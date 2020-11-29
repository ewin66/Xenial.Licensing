
using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using Xenial.Licensing.Api.Controllers;
using Xenial.Licensing.Api.Infrastructure;

namespace Xenial.Licensing.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddXpo(Configuration);
            services.AddXpoDefaultUnitOfWork();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = Configuration.GetSection("Authentication:Xenial").GetValue<string>("ClientId"), Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri($"{Configuration.GetSection("Authentication:Xenial").GetValue<string>("Authority")}/connect/authorize"),
                            TokenUrl = new Uri($"{Configuration.GetSection("Authentication:Xenial").GetValue<string>("Authority")}/connect/token"),
                            Scopes = new Dictionary<string, string>
                            {
                                [Configuration.GetSection("Authentication:Xenial").GetValue<string>("Audience")] = Configuration.GetSection("Authentication:Xenial").GetValue<string>("Audience"),
                            }
                        },
                    }
                });

                c.OperationFilter<AuthorizeCheckOperationFilter>(Configuration);
            });
            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication("Bearer", (options) =>
                {
                    Configuration.Bind("Authentication:Xenial", options);
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{Configuration.GetSection("Authentication:Xenial").GetValue<string>("ClientId")} v1");
                c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);

                c.OAuthClientId(Configuration.GetSection("Authentication:Xenial.Swagger").GetValue<string>("ClientId"));
                c.OAuthAppName(Configuration.GetSection("Authentication:Xenial.Swagger").GetValue<string>("ClientId"));
                c.OAuthClientSecret(Configuration.GetSection("Authentication:Xenial.Swagger").GetValue<string>("ClientSecret"));
                c.OAuthUsePkce();
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
