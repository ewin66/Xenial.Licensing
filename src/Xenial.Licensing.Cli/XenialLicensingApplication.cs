using System;
using System.Threading.Tasks;

using static System.Console;
using Xenial.Licensing.Cli.Services.Commands.Default;

namespace Xenial.Licensing.Cli
{
    public class XenialLicensingApplication
    {
        private readonly DefaultWelcomeCommand welcomeCommand;

        public XenialLicensingApplication(DefaultWelcomeCommand welcomeCommand)
            => this.welcomeCommand = welcomeCommand;

        public async Task<int> RunAsync(string[] args)
        {
            try
            {
                var result = await welcomeCommand.Execute();
                WriteLine();
                WriteLine($"Done {result}");

                ReadLine();
                return result;
            }
            catch (Exception ex)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine(ex.Message);
                ResetColor();
                return 1;
            }
        }
    }
}
