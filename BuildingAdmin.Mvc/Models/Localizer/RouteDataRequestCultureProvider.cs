using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Internal;

namespace BuildingAdmin.Mvc.Models.Localizer
{
    /// <summary>
    /// Determines the culture information for a request via values in the route data.
    /// </summary>
    public class RouteDataRequestCultureProvider : RequestCultureProvider
    {
        /// <summary>
        /// The key that contains the culture name.
        /// Defaults to "culture".
        /// </summary>
        public string RouteDataStringKey { get; set; } = "culture";

        /// <summary>
        /// The key that contains the UI culture name. If not specified or no value is found,
        /// <see cref="RouteDataStringKey"/> will be used.
        /// Defaults to "ui-culture".
        /// </summary>
        public string UIRouteDataStringKey { get; set; } = "ui-culture";

        /// <inheritdoc />
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            string culture = null;
            string uiCulture = null;

            uiCulture =  culture = httpContext.Request.Path.Value.Split('/')[1]?.ToString();
            if (string.IsNullOrEmpty(culture))
            {
                uiCulture = culture = "en";
            }
            var providerResultCulture = new ProviderCultureResult(culture, uiCulture);

            return Task.FromResult(providerResultCulture);
        }
    }
}