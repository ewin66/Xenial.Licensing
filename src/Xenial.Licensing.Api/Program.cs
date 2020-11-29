using System;

using DevExpress.Xpo;
using DevExpress.Xpo.DB;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

using Xenial.Licensing.Api;
using Xenial.Licensing.Api.Controllers;

SQLiteConnectionProvider.Register();
MySqlConnectionProvider.Register();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.File(
        @"C:\logs\licensing.xenial.io\api.licensing.xenial.io.log",
        fileSizeLimitBytes: 1_000_000,
        rollOnFileSizeLimit: true,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1))
    //#endif
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
    .CreateLogger();

try
{
    Log.Information("Starting host...");

    var host = CreateHostBuilder(args).Build();

    var serviceCollection = new ServiceCollection();
    serviceCollection
        .AddXpo(host.Services.GetRequiredService<IConfiguration>(), AutoCreateOption.DatabaseAndSchema)
        .AddXpoDefaultUnitOfWork();

    using (var provider = serviceCollection.BuildServiceProvider())
    using (var unitOfWork = provider.GetRequiredService<UnitOfWork>())
    {
        unitOfWork.UpdateSchema();
    }

    Log.Information("Update Done");

    host.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
