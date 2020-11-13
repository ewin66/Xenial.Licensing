using System;
using System.Net.Http;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;

using Xenial.Licensing.Cli;
using Xenial.Licensing.Cli.Services;
using Xenial.Licensing.Cli.Services.Commands.Default;
using Xenial.Licensing.Cli.Services.Default;
using Xenial.Licensing.Cli.Services.Queries;
using Xenial.Licensing.Cli.Services.Queries.Default;
using Xenial.Licensing.Cli.XenialLicenseApi;

var host = CreateHostBuilder(args).Build();

using var serviceScope = host.Services.CreateScope();

var services = serviceScope.ServiceProvider;

try
{
    var myService = services.GetRequiredService<XenialLicensingApplication>();
    return await myService.RunAsync(args);
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Error Occured {0}", ex);
    Console.ResetColor();
    return -1;
}

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddTransient<IUserProfileProvider, DefaultUserProfileProvider>();
            services.AddTransient<IDeviceIdProvider, DefaultDeviceIdProvider>();

            services.AddHttpClient<DefaultTokenProvider>();
            services.AddHttpClient(nameof(LicenseClient));
            services.AddTransient<ITokenProvider, DefaultTokenProvider>();
            services.AddTransient<IApiClientConfiguration, DefaultApiClientConfiguration>();

            services.AddTransient<ILicenseClient, LicenseClient>(
                s => new LicenseClient(
                    s.GetRequiredService<IApiClientConfiguration>(),
                    s.GetRequiredService<IHttpClientFactory>().CreateClient(nameof(LicenseClient))
            ));

            services.AddTransient<ILicenseQuery, DefaultLicenseQuery>();
            services.AddTransient<DefaultWelcomeCommand>();

            services.AddSingleton<XenialLicensingApplication>();
        });
