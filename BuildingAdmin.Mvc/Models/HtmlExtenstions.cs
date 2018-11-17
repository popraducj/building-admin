using System;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BuildingAdmin.Mvc.Models
{
    public static class HtmlExtentions
    {
        public static IConfiguration Configuration {get;set;}
        public static string AbsoluteAction(this IUrlHelper url, string action, string controller, object routeValues = null){
            
            var urlAbsolute= url.Action( new UrlActionContext(){ 
                    Action = action, 
                    Controller = controller,
                    Values= routeValues
                });

            string absoluteAction = string.Format(
                "{0}{1}{2}",
                Configuration["URL"],
                urlAbsolute.Length < 4 || urlAbsolute[3].Equals('/') ? string.Empty: $"/{CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower()}",
                urlAbsolute);

            return absoluteAction;
        }
        public static string ActiveMenuItem(this IHtmlHelper htmlHelper, string action, string controller)
        {
            var currentAction = (string)htmlHelper.ViewContext.RouteData.Values["action"];
            var currentController = (string)htmlHelper.ViewContext.RouteData.Values["controller"];
            if (string.Equals(currentAction, action, StringComparison.CurrentCultureIgnoreCase)
                && string.Equals(currentController, controller, StringComparison.CurrentCultureIgnoreCase))
            {
                return "active";
            }
            return string.Empty;
        }
    }
}