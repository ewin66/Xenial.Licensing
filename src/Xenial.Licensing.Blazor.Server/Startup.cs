using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.ExpressApp.Security;
using DevExpress.Blazor.Reporting;
using DevExpress.ExpressApp.ReportsV2.Blazor;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.Persistent.Base;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xenial.Licensing.Blazor.Server.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using IdentityModel.Client;
using System.Net.Http;
using System.Security.Claims;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.ExpressApp;
using System.Security.Principal;
using DevExpress.Data.Filtering;
using Xenial.Licensing.Blazor.Server.Infrastructure;
using Xenial.Licensing.Model;

namespace Xenial.Licensing.Blazor.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
            => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHttpContextAccessor();
            services.AddSingleton<XpoDataStoreProviderAccessor>();
            services.AddScoped<CircuitHandler, CircuitHandlerProxy>();
            services.AddXaf<LicensingBlazorApplication>(Configuration);
            services.AddXafReporting();
            services.AddXafSecurity(options =>
            {
                options.RoleType = typeof(PermissionPolicyRole);
                options.UserType = typeof(PermissionPolicyUser);
                options.Events.OnSecurityStrategyCreated = securityStrategy => ((SecurityStrategy)securityStrategy).RegisterXPOAdapterProviders();
                options.SupportNavigationPermissionsForTypes = false;
            }).AddExternalAuthentication<HttpContextPrincipalProvider>(
                options =>
                {
                    options.Events.Authenticate = (objectSpace, externalUser) =>
                    {
                        var autoCreateUserByExternalProviderInfo = true;
                        return ProcessExternalLogin(objectSpace, externalUser, autoCreateUserByExternalProviderInfo);

                        PermissionPolicyUser ProcessExternalLogin(IObjectSpace os, IPrincipal externalUser, bool autoCreateUser)
                        {
                            var userIdClaim = ((ClaimsPrincipal)externalUser).FindFirst("sub") ??
                                ((ClaimsPrincipal)externalUser).FindFirst(ClaimTypes.NameIdentifier) ??
                                throw new Exception("Unknown user id");

                            var providerUserId = userIdClaim.Value;
                            var userLoginInfo = os.FindObject<UserLoginInfo>(CriteriaOperator.And(
                                    new BinaryOperator(nameof(UserLoginInfo.LoginProviderName), externalUser.Identity.AuthenticationType),
                                    new BinaryOperator(nameof(UserLoginInfo.ProviderUserKey), providerUserId)
                            ));

                            if (userLoginInfo != null)
                            {
                                return UpdateUser(os, userLoginInfo.User, externalUser);
                            }
                            else
                            {
                                if (autoCreateUser)
                                {
                                    var user = CreatePermissionPolicyUser(os, externalUser);
                                    if (user != null)
                                    {
                                        user.CreateUserLoginInfo(os, externalUser.Identity.AuthenticationType, providerUserId);
                                    }
                                    return user;
                                }
                            }
                            return null;
                        }

                        PermissionPolicyUser CreatePermissionPolicyUser(IObjectSpace os, IPrincipal externalUser)
                        {
                            var user = os.CreateObject<PermissionPolicyUser>();
                            return UpdateUser(os, user, externalUser);
                        }

                        PermissionPolicyUser UpdateUser(IObjectSpace os, PermissionPolicyUser user, IPrincipal externalUser)
                        {
                            user.UserName = externalUser.Identity.Name;

                            foreach (var role in user.Roles.ToList())
                            {
                                user.Roles.Remove(role);
                            }
                            if (externalUser.Identity is ClaimsIdentity identity)
                            {
                                var roles = identity.Claims.Where(c => c.Type == identity.RoleClaimType || c.Type == ClaimTypes.Role).ToList();
                                foreach (var role in roles)
                                {
                                    user.Roles.Add(os.FindObject<PermissionPolicyRole>(new BinaryOperator(nameof(PermissionPolicyRole.Name), role.Value)));
                                }
                            }

                            user.Roles.Add(os.FindObject<PermissionPolicyRole>(new BinaryOperator(nameof(PermissionPolicyRole.Name), "User")));
                            user.Roles.Add(os.FindObject<PermissionPolicyRole>(new BinaryOperator(nameof(PermissionPolicyRole.Name), "Default")));

                            os.CommitChanges();

                            return user;
                        }
                    };
                })
            .AddAuthenticationStandard(options =>
            {
                options.IsSupportChangePassword = false;
            });

            services.Configure<OpenIdConnectOptions>("Xenial",
                options =>
                {
                    Configuration.Bind("Authentication:Xenial", options);
                    if (options.GetClaimsFromUserInfoEndpoint)
                    {
                        var standardClaims = new[]
                        {
                            "name",
                            "given_name",
                            "family_name",
                            "middle_name",
                            "nickname",
                            "preferred_username",
                            "profile",
                            "picture",
                            "website",
                            "gender",
                            "birthdate",
                            "zoneinfo",
                            "locale",
                            "address",
                            "updated_at",
                            "email",
                            "email_verified",
                            "phone",
                            "phone_verified",
                            "sub",
                            "openid",
                        };
                        foreach (var standardClaim in standardClaims)
                        {
                            options.ClaimActions.MapUniqueJsonKey(standardClaim, standardClaim);
                        }
                        options.ClaimActions.MapJsonKey("role", "role");
                        options.ClaimActions.MapUniqueJsonKey("xenial", "xenial");
                        options.ClaimActions.MapUniqueJsonKey("xenial_forecolor", "xenial_forecolor");
                        options.ClaimActions.MapUniqueJsonKey("xenial_backcolor", "xenial_backcolor");
                        options.ClaimActions.MapUniqueJsonKey("xenial_initials", "xenial_initials");
                    }
                });

            services.AddHttpClient("identity");

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => options.LoginPath = "/LoginPage")
                .AddOpenIdConnect("Xenial", "Xenial", options => { });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    //var xafApplicationProvider = context.RequestServices.GetService<IXafApplicationProvider>();
                    //var application = xafApplicationProvider.GetApplication();
                    //application.Setup();

                    //var xafApplicationLogonService = context.RequestServices.GetService<IXafApplicationLogonService>();
                    await context.ChallengeAsync("Xenial");
                }
                else
                {
                    await next();
                }
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/api/signOut"))
                {
                    if (context.Request.Query.ContainsKey("state") && context.Request.Query["state"] == "loggedOut")
                    {
                        await next();
                        return;
                    }

                    var token = await context.GetTokenAsync("id_token");
                    if (!string.IsNullOrEmpty(token))
                    {
                        var configuration = context.RequestServices.GetService<IConfiguration>();
                        var httpClient = context.RequestServices.GetService<IHttpClientFactory>().CreateClient("identity");

                        var authority = configuration.GetSection("Authentication:Xenial").GetValue<string>("Authority");

                        var disco = await httpClient.GetDiscoveryDocumentAsync(authority);
                        var fullUrl = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetEncodedUrl(context.Request);
                        if (!string.IsNullOrEmpty(disco.EndSessionEndpoint))
                        {
                            var url = new RequestUrl(disco.EndSessionEndpoint).CreateEndSessionUrl(token, fullUrl, "loggedOut");

                            context.Response.Redirect(url);

                            return;
                        }
                    }
                }

                await next();
            });
            app.UseXaf();
            app.UseDevExpressBlazorReporting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapControllers();
            });
        }
    }
}
