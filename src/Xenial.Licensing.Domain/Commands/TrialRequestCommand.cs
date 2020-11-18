using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.Xpo;

using Xenial.Licensing.Model;
using Xenial.Licensing.Model.Infrastructure;

namespace Xenial.Licensing.Domain.Commands
{
    public record TrialRequestCommand(string UserId, string MachineKey, int? DefaultTrialCooldown, int? DefaultTrialPeriod);
    public record TrialRequestResult(Guid Id, string License, DateTime? ExpiresAt);

    public class TrialRequestCommandHandler
    {
        private readonly UnitOfWork unitOfWork;

        public TrialRequestCommandHandler(UnitOfWork unitOfWork)
            => this.unitOfWork = unitOfWork;

        public async Task<TrialRequestResult> ExecuteAsync(TrialRequestCommand command)
        {
            var settings = (await unitOfWork.GetSingletonAsync<LicenseSettings>()) ?? throw new ArgumentNullException(nameof(LicenseSettings));

            if (!command.DefaultTrialCooldown.HasValue)
            {
                command = command with { DefaultTrialCooldown = settings.DefaultTrialCooldown };
            }
            if (!command.DefaultTrialPeriod.HasValue)
            {
                command = command with { DefaultTrialPeriod = settings.DefaultTrialPeriod };
            }

            var licenses = await unitOfWork.Query<License>()
                .Where(l =>
                    l.User != null
                    && l.User.Id == command.UserId
                    && l.Type == License.LicenseType.Trial
                    && (l.ExpiresAt.HasValue || l.ExpiresNever)
                )
                .ToListAsync();

            if (licenses.Any(l => l.ExpiresNever))
            {
                var neverExpireTrial = licenses.First(l => l.ExpiresNever);
                return new TrialRequestResult(neverExpireTrial.Id, neverExpireTrial.GeneratedLicense.ToString(), DateTime.MaxValue);
            }
            else
            {
                var expireTrial = licenses
                    .Where(l => l.ExpiresAt.HasValue)
                    .OrderBy(l => l.ExpiresAt.Value)
                    .FirstOrDefault();

                if (expireTrial != null)
                {
                    return new TrialRequestResult(expireTrial.Id, expireTrial.GeneratedLicense.ToString(), expireTrial.ExpiresAt.Value);
                }
            }

            var trialRequest = await unitOfWork.Query<TrialRequest>()
                   .Where(trial => trial.UserId == command.UserId)
                   .OrderByDescending(trial => trial.RequestDate)
                   .FirstOrDefaultAsync();

            if (trialRequest != null)
            {
                if ((trialRequest.RequestDate - DateTime.UtcNow.Date).TotalDays < settings.DefaultTrialCooldown)
                {
                    //Last trial request is older than 1 year / DefaultTrialCooldown
                }

                //Need to lockout from new trial
            }

            var trialRequests = await unitOfWork.Query<TrialRequest>()
                .Where(trial => trial.MachineKey == command.MachineKey)
                .OrderByDescending(trial => trial.RequestDate)
                .Take(2)
                .ToListAsync();

            if (trialRequests.Count <= 2) //We allow a second trial with a different email
            {

            }
            else //We have a second trial request on the same machine with a different email
            {

            }

            trialRequest = new TrialRequest(unitOfWork)
            {
                MachineKey = command.MachineKey,
                UserId = command.UserId
            };

            await unitOfWork.SaveAsync(trialRequest);
            await unitOfWork.CommitChangesAsync();

            var user = await unitOfWork.GetObjectByKeyAsync<CompanyUser>(command.UserId);
            if (user == null)
            {
                user = new CompanyUser(unitOfWork)
                {
                    Id = command.UserId
                };
            }

            var license = new License(unitOfWork)
            {
                User = user,
            };

            await unitOfWork.SaveAsync(license);
            await unitOfWork.CommitChangesAsync();

            if (!license.ExpiresAt.HasValue)
            {
                throw new ArgumentException($"License must have {nameof(license.ExpiresAt)} set for a trial request.");
            }

            return new TrialRequestResult(license.Id, license.GeneratedLicense.ToString(), license.ExpiresAt.Value);
        }
    }
}
