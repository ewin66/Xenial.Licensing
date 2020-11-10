using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Xenial.Licensing.Cli;

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
            services.AddSingleton<XenialLicensingApplication>();
        });
