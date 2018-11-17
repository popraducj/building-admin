using System;
using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;

namespace BuildingAdmin.Mvc.Models.Localizer
{
    public static class LocalizerErrors
    {
        public static IApplicationBuilder UseStatusCodePagesWithReExecuteMultilanguage(
            this IApplicationBuilder app,
            string pathFormat,
            string queryFormat = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseStatusCodePages(async context =>
            {
                var culture = context.HttpContext.Request.Path.Value.Split('/')[1]?.ToString();
                
                var newPath = new PathString(
                    string.Format(pathFormat, culture, context.HttpContext.Response.StatusCode));
                var formatedQueryString = queryFormat == null ? null :
                    string.Format(queryFormat, context.HttpContext.Response.StatusCode);
                var newQueryString = queryFormat == null ? QueryString.Empty : new QueryString(formatedQueryString);

                var originalPath = context.HttpContext.Request.Path;
                var originalQueryString = context.HttpContext.Request.QueryString;
                // Store the original paths so the app can check it.
                context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(new StatusCodeReExecuteFeature()
                {
                    OriginalPathBase = context.HttpContext.Request.PathBase.Value,
                    OriginalPath = originalPath.Value,
                    OriginalQueryString = originalQueryString.HasValue ? originalQueryString.Value : null,
                });

                context.HttpContext.Request.Path = newPath;
                context.HttpContext.Request.QueryString = newQueryString;
                try
                {
                    await context.Next(context.HttpContext);
                }
                finally
                {
                    context.HttpContext.Request.QueryString = originalQueryString;
                    context.HttpContext.Request.Path = originalPath;
                    context.HttpContext.Features.Set<IStatusCodeReExecuteFeature>(null);
                }
            });
        }
        public static string GetRawTarget(this HttpRequest request)	{	
            var httpRequestFeature = request.HttpContext.Features.Get<IHttpRequestFeature>();
            return httpRequestFeature.RawTarget;
        }

        public static IApplicationBuilder UseExceptionHandlerMultilanguage(this IApplicationBuilder app, string errorHandlingPath)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
           
            return app.UseExceptionHandler(new ExceptionHandlerOptions
            {
                ExceptionHandlingPath = new PathString(errorHandlingPath)
            });
        }
    }
}