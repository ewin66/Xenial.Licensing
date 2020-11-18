using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DevExpress.Xpo;

using Shouldly;

using Standard.Licensing.Validation;

using Xenial.Licensing.Domain.Commands;
using Xenial.Licensing.Model;

using static Xenial.Tasty;

namespace Xenial.Licensing.Tests.Domain
{
    public static class TrialRequestCommandHandlerTests
    {
        public static void Tests(string name, string connectionString) => Describe($"{nameof(TrialRequestCommandHandler)} using {name}", () =>
        {
            var dataLayer = XpoDefault.GetDataLayer(connectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
            using var unitOfWork = new UnitOfWork(dataLayer);
            var settings = CreateDefaultSettings(unitOfWork);

            unitOfWork.UpdateSchema();
            unitOfWork.Save(settings);
            unitOfWork.CommitChanges();

            async Task<TrialRequestResult> ExecuteCommand(TrialRequestCommand command)
            {
                using var uow = new UnitOfWork(dataLayer);
                return await new TrialRequestCommandHandler(uow).ExecuteAsync(command);
            }

            Describe("can request a trial", () =>
            {
                It("With normal user", async () =>
                {
                    var trial = await ExecuteCommand(new TrialRequestCommand("ExistingUser", "Machine1", null, null));

                    trial.ShouldSatisfyAllConditions(
                        () => trial.ShouldNotBeNull(),
                        () => trial.Id.ShouldNotBe(default),
                        () => trial.ExpiresAt.ShouldNotBeNull(),
                        () => trial.ExpiresAt.Value.Date.Date.ShouldBe(DateTime.Today.AddDays(30)),
                        () => trial.License.ShouldNotBeNull(),
                        () => Standard.Licensing.License.Load(trial.License)
                                .Validate()
                                .ExpirationDate()
                                .And()
                                .Signature(settings.DefaultLicensingKey.PublicKey)
                                .AssertValidLicense()
                                .ToList().Any().ShouldBe(false, "License is not valid")
                    );
                });
            });
        });

        private static LicenseSettings CreateDefaultSettings(UnitOfWork unitOfWork)
        {
            var key = new LicensingKey(unitOfWork);
            key.PassPhrase = "This is the passphrase";
            var keyGenerator = Standard.Licensing.Security.Cryptography.KeyGenerator.Create();
            var keyPair = keyGenerator.GenerateKeyPair();
            key.PrivateKey = keyPair.ToEncryptedPrivateKeyString(key.PassPhrase);
            key.PublicKey = keyPair.ToPublicKeyString();

            var settings = new LicenseSettings(unitOfWork)
            {
                DefaultLicensingKey = key
            };
            return settings;
        }
    }
}
