using System;
using System.IO;

using Xenial.Delicious.Beer.Recipes;

using static SimpleExec.Command;
using static Bullseye.Targets;
using static Xenial.Delicious.Beer.Recipes.IISRecipe;

var projectName = "Xenial.Licensing";
var sln = $"{projectName}.sln";

Target("restore:npm", () => RunAsync("cmd.exe", $"/C npm ci"));
Target("restore:dotnet", () => RunAsync("dotnet", $"restore {sln}"));
Target("restore", DependsOn("restore:npm", "restore:dotnet"));

Target("build:npm", () => RunAsync("cmd.exe", $"/C npm run build"));
Target("build:dotnet", DependsOn("restore:dotnet"), () => RunAsync("dotnet", $"build {sln} --no-restore"));
Target("build", DependsOn("restore", "build:npm", "build:dotnet"));

BuildAndDeployIISProject(new IISDeployOptions("Xenial.Licensing.Blazor.Server", "admin.licensing.xenial.io"), "admin");
BuildAndDeployIISProject(new IISDeployOptions("Xenial.Licensing.Api", "api.licensing.xenial.io"), "api");

Target("publish", DependsOn("publish:admin", "publish:api"));

Target("default", DependsOn("build", "publish"));

await RunTargetsAndExitAsync(args);
