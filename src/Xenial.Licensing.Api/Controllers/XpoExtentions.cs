using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DevExpress.Xpo.DB;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xenial.Licensing.Model;

namespace Xenial.Licensing.Api.Controllers
{
    public static class XpoExtentions
    {
        public static IServiceCollection AddXpo(
            this IServiceCollection services,
            IConfiguration configuration,
            AutoCreateOption autoCreateOption = AutoCreateOption.None
        )
            => services.AddXpoDefaultDataLayer(ServiceLifetime.Singleton, dl => dl
                .UseConnectionString(configuration.GetConnectionString("DefaultConnection"))
                .UseThreadSafeDataLayer(autoCreateOption == AutoCreateOption.None)
                .UseConnectionPool(autoCreateOption == AutoCreateOption.None)
                .UseAutoCreationOption(autoCreateOption)
                .UseEntityTypes(
                    new Type[0]
                        .UseLicensingPersistentModels()
                    .ToArray()
                )
            );
    }
}
