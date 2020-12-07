using System;
using System.Linq;
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
            var settings = unitOfWork.CreateDefaultSettings();

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
                    var trial = await ExecuteCommand(new TrialRequestCommand(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, null));

                    trial.ShouldSatisfyAllConditions(
                        () => trial.ShouldNotBeNull(),
                        () => trial.Id.ShouldNotBe(default),
                        () => trial.ExpiresAt.Date.ShouldBe(DateTime.Today.AddDays(settings.DefaultTrialPeriod)),
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

                It("With existing valid trial license gets the same", async () =>
                {
                    var machine = Guid.NewGuid().ToString();
                    var user = Guid.NewGuid().ToString();
                    var trial1 = await ExecuteCommand(new TrialRequestCommand(user, machine, null, null));
                    var trial2 = await ExecuteCommand(new TrialRequestCommand(user, machine, null, null));

                    trial1.ShouldBeEquivalentTo(trial2);
                });

                It("With existing old trial license gets a new one", async () =>
                {
                    using var uow = new UnitOfWork(dataLayer);
                    await uow.SaveAsync(new TrialRequest(uow)
                    {
                        MachineKey = "Machine1",
                        UserId = "ExistingUser",
                        RequestDate = DateTime.Today.AddDays((settings.DefaultTrialCooldown * -1) + 1)
                    });

                    await uow.CommitChangesAsync();

                    var trial = await ExecuteCommand(new TrialRequestCommand(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, null));

                    trial.ShouldSatisfyAllConditions(
                        () => trial.ShouldNotBeNull(),
                        () => trial.Id.ShouldNotBe(default),
                        () => trial.ExpiresAt.Date.ShouldBe(DateTime.Today.AddDays(settings.DefaultTrialPeriod)),
                        () => trial.License.ShouldNotBeNull(),
                        () => Standard.Licensing.License.Load(trial.License)
                                .IsLicenseValid(settings)
                                .ShouldBe(true, "License should be valid, but is invalid")
                    );
                });

                It("old trial license is invalid", async () =>
                {
                    var trial = await ExecuteCommand(new TrialRequestCommand(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null, null));
                    using var uow = new UnitOfWork(dataLayer);
                    var persistentTrial = await uow.GetObjectByKeyAsync<GrantedLicense>(trial.Id);
                    persistentTrial.ExpiresAt = DateTime.Now.AddDays(-1);
                    await uow.SaveAsync(persistentTrial);
                    await uow.CommitChangesAsync();

                    persistentTrial.ShouldSatisfyAllConditions(
                        () => persistentTrial.GeneratedLicense.ShouldNotBeNull(),
                        () => persistentTrial.GeneratedLicense
                                .IsLicenseValid(settings)
                                .ShouldBe(false, "License is valid, but should be invalid")
                    );
                });

                It("second trial with same machine key should be valid (user 'extends' trial with new email)", async () =>
                {
                    var machine = Guid.NewGuid().ToString();
                    var trial1 = await ExecuteCommand(new TrialRequestCommand(Guid.NewGuid().ToString(), machine, null, null));
                    var trial2 = await ExecuteCommand(new TrialRequestCommand(Guid.NewGuid().ToString(), machine, null, null));

                    (trial1 == trial2).ShouldBeFalse("Second request should be fine");
                });

                It("third trial with same machine key should be null", async () =>
                {
                    var machine = Guid.NewGuid().ToString();
                    var trial1 = await ExecuteCommand(new TrialRequestCommand(Guid.NewGuid().ToString(), machine, null, null));
                    var trial2 = await ExecuteCommand(new TrialRequestCommand(Guid.NewGuid().ToString(), machine, null, null));

                    await Should.ThrowAsync<InvalidOperationException>(
                        () => ExecuteCommand(new TrialRequestCommand(Guid.NewGuid().ToString(), machine, null, null))
                    );
                });

                It("third trial with same machine key after cooldown should be valid", async () =>
                {
                    var machine = Guid.NewGuid().ToString();
                    await ExecuteCommand(new TrialRequestCommand(Guid.NewGuid().ToString(), machine, null, null));
                    await ExecuteCommand(new TrialRequestCommand(Guid.NewGuid().ToString(), machine, null, null));

                    using var uow = new UnitOfWork(dataLayer);
                    foreach (var trialRequest in await uow.Query<TrialRequest>().Where(t => t.MachineKey == machine).ToListAsync())
                    {
                        trialRequest.RequestDate = DateTime.UtcNow.AddDays((settings.DefaultTrialCooldown * -1) - 1);
                        await uow.SaveAsync(trialRequest);
                    }

                    await uow.CommitChangesAsync();

                    var trial = await ExecuteCommand(new TrialRequestCommand(Guid.NewGuid().ToString(), machine, null, null));

                    trial.ShouldSatisfyAllConditions(
                        () => trial.ShouldNotBeNull(),
                        () => Standard.Licensing.License.Load(trial.License)
                                .IsLicenseValid(settings)
                                .ShouldBeTrue("License should be valid, but was not")
                    );
                });

            });
        });

        private static bool IsLicenseValid(this Standard.Licensing.License license, LicenseSettings settings)
            => !license.Validate()
                .ExpirationDate()
                .And()
                .Signature(settings.DefaultLicensingKey.PublicKey)
                .AssertValidLicense()
                .ToList().Any();

        private static LicenseSettings CreateDefaultSettings(this UnitOfWork unitOfWork)
        {
            var key = new LicensingKey(unitOfWork)
            {
                PassPhrase = "This is the passphrase"
            };

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
