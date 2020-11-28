using System;
using System.IO;

using Xenial.Delicious.Beer.Recipes;

using static SimpleExec.Command;
using static Bullseye.Targets;
using static Xenial.Delicious.Beer.Recipes.IISRecipe;
using static Xenial.Delicious.Beer.Json.PokeJson;

const string PleaseSet = "PLEASE SET BEFORE USE";

var projectName = "Xenial.Licensing";
var sln = $"{projectName}.sln";

Target("restore:npm", () => RunAsync("cmd.exe", $"/C npm ci"));
Target("restore:dotnet", () => RunAsync("dotnet", $"restore {sln}"));
Target("restore", DependsOn("restore:npm", "restore:dotnet"));

Target("build:npm", () => RunAsync("cmd.exe", $"/C npm run build"));
Target("build:dotnet", DependsOn("restore:dotnet"), () => RunAsync("dotnet", $"build {sln} --no-restore"));
Target("build", DependsOn("restore", "build:npm", "build:dotnet"));

BuildAndDeployIISProject(new IISDeployOptions("Xenial.Licensing.Blazor.Server", "admin.licensing.xenial.io")
{
    PrepareTask = async () =>
    {
        var settingsPath = "./src/Xenial.Licensing.Blazor.Server/appsettings.json";

        var serverSettings = await File.ReadAllTextAsync(settingsPath);

        serverSettings = serverSettings
            .AddOrUpdateJsonValue(
                "ConnectionStrings:DefaultConnection",
                Environment.GetEnvironmentVariable("XENIAL_DEFAULTCONNECTIONSTRING") ?? PleaseSet
            )
            .AddOrUpdateJsonValue(
                "Authentication:Xenial:ClientSecret",
                Environment.GetEnvironmentVariable("ADMIN_AUTHENTICATION_XENIAL_CLIENTSECRET") ?? PleaseSet
            )
        ;

        await File.WriteAllTextAsync(settingsPath, serverSettings);
    }
}, "admin");

BuildAndDeployIISProject(new IISDeployOptions("Xenial.Licensing.Api", "api.licensing.xenial.io")
{
    PrepareTask = async () =>
    {
        var settingsPath = "./src/Xenial.Licensing.Api/appsettings.json";

        var serverSettings = await File.ReadAllTextAsync(settingsPath);

        serverSettings = serverSettings
            .AddOrUpdateJsonValue(
                "ConnectionStrings:DefaultConnection",
                Environment.GetEnvironmentVariable("XENIAL_DEFAULTCONNECTIONSTRING") ?? PleaseSet
            )
            .AddOrUpdateJsonValue(
                "Authentication:Xenial:ApiSecret",
                Environment.GetEnvironmentVariable("API_AUTHENTICATION_XENIAL_APISECRET") ?? PleaseSet
            )
            .AddOrUpdateJsonValue(
                "Authentication:Xenial.Swagger:ClientSecret",
                Environment.GetEnvironmentVariable("API_AUTHENTICATION_XENIAL_SWAGGER_CLIENTSECRET") ?? PleaseSet
            )
        ;

        await File.WriteAllTextAsync(settingsPath, serverSettings);
    }
}, "api");

Target("publish", DependsOn("publish:admin", "publish:api"));

Target("default", DependsOn("build", "publish"));

await RunTargetsAndExitAsync(args);
