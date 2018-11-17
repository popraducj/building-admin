using System;
using System.Collections.Generic;
using System.Globalization;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using BuildingAdmin.BussinessLogic;
using BuildingAdmin.BussinessLogic.Cryptography;
using BuildingAdmin.BussinessLogic.Email.Contracts;
using BuildingAdmin.BussinessLogic.Email.Implementation;
using BuildingAdmin.DataLayer;
using BuildingAdmin.Mvc.Models;
using BuildingAdmin.Mvc.Models.Localizer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.HttpOverrides;
using BuildingAdmin.Mvc.Controllers;
using MongoDbGenericRepository;

namespace BuildingAdmin.Mvc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            HtmlExtentions.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region authenctication and authorization
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.AccessDeniedPath = "/home/forbbiden";
                options.LoginPath = "/account/login";
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.SlidingExpiration = true;
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy => policy.RequireRole("admin"));
                options.AddPolicy("badmin", policy => policy.RequireRole("buildingAdmin"));
                options.AddPolicy("owner", policy => policy.RequireRole("propertyOwner"));
                options.AddPolicy("both", policy => policy.RequireRole("buildingAdmin" ,"propertyOwner"));
            });
            #endregion;

            #region localization
            services.AddJsonLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("ro"),
                    new CultureInfo("en")
                };
                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                
                var provider = new RouteDataRequestCultureProvider();
                provider.UIRouteDataStringKey = "lang";
                provider.RouteDataStringKey = "lang";
                provider.Options = options;
                options.RequestCultureProviders = new[] { provider };
            });

            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("lang", typeof(LanguageRouteConstraint));
            });
            #endregion;

            services.AddDataProtection();
            services.AddEmailService();
            services.AddDependencies();

            #region logging    
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Error));

            var serviceProvider = services.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            //configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            NLog.LogManager.LoadConfiguration("Configs/nlog.config");
            #endregion;


            #region configuration
            services.Configure<HashingConfig>(Configuration.GetSection("Hashing"));
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddScoped<ValidateReCaptchaAttribute>();
            services.AddTransient<ApartmentController>();
            #endregion;            
            
            if(Configuration["InitDb"].Equals("true"))
            {
                var repository = serviceProvider.GetService<IBaseMongoRepository>();
                var hashing = serviceProvider.GetService<IArgonHash>();
                MongoDb.Initilize(repository, hashing, Configuration.GetSection("Hashing").Get<HashingConfig>());
            }

            // url helper
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                    .AddScoped<IUrlHelper>(x => x .GetRequiredService<IUrlHelperFactory>()
                    .GetUrlHelper(x.GetRequiredService<IActionContextAccessor>().ActionContext));
            services.AddMvc(options =>{
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                //options.Filters.Add(new RequireHttpsAttribute());
            }).AddViewLocalization().AddDataAnnotationsLocalization();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStatusCodePagesWithReExecuteMultilanguage("/{0}/Home/Error/{1}");
            app.UseExceptionHandler("/home/error/500");

            app.UseForwardedHeaders(new ForwardedHeadersOptions{
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });           

            app.UseHsts(opts => opts.MaxAge(days: 365).IncludeSubdomains());
            app.UseXContentTypeOptions();
            app.UseReferrerPolicy(opts => opts.StrictOriginWhenCrossOrigin());
            app.UseXXssProtection(opts => opts.EnabledWithBlockMode());
            app.UseXfo(opts => opts.Deny());
            app.UseCsp(opts => opts
                .BlockAllMixedContent()
                .StyleSources(s => s.CustomSources("https://fonts.gstatic.com", "https://fonts.googleapis.com", "https://maxcdn.bootstrapcdn.com", "http://localhost:5000"))
                .StyleSources(s => s.Self())
                .StyleSources(s => s.UnsafeInline())
                .FontSources(s => s.Self())
                .FontSources(s => s.CustomSources("https://fonts.gstatic.com", "https://fonts.googleapis.com", "https://maxcdn.bootstrapcdn.com", "http://localhost:5000"))
                .FormActions(s => s.Self())
                .FrameAncestors(s => s.Self())
                .ScriptSources(s => s.Self())
                .ScriptSources(s => s.UnsafeInline())
                .ScriptSources(s => s.CustomSources("https://www.google.com","https://www.gstatic.com", "https://maps.googleapis.com", "https://www.googletagmanager.com/",
                                                    "https://www.google-analytics.com", "http://localhost:5000"))
            );

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // .Append requires the following import:
                    // using Microsoft.AspNetCore.Http;
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=600");
                    ctx.Context.Response.Headers.Add("Expires", new[] { DateTime.UtcNow.AddSeconds(500).ToString("R") });
                }
            });

             app.UseAuthentication();

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "LocalizedDefault",                    
                    template: "{lang:lang}/{controller=Home}/{action=Index}/{id?}"
                );
                routes.MapRoute(
                    name: "bills",                    
                    template: "Bills/Get/{month?}"
                );
                routes.MapRoute(
                    name: "default",                    
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "RedirectToDefaultLanguage" }
                );
                routes.MapRoute(
                    name: "catchall",
                    template: "{*catchall}",
                    defaults: new { controller = "Home", action = "Error" });
            });
        }
    }
}
