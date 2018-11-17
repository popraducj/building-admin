using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Linq;
using System;
using Microsoft.Extensions.Logging;
using BuildingAdmin.Mvc.Models;
using MongoDbGenericRepository;
using BuildingAdmin.BussinessLogic.Email.Contracts;
using BuildingAdmin.BussinessLogic.Email.Implementation;
using Microsoft.AspNetCore.DataProtection;
using BuildingAdmin.BussinessLogic.Email.EmailType;

namespace BuildingAdmin.Mvc.Controllers
{
    public class HomeController : Controller
    {      
        private IStringLocalizer _localizer;
        private readonly ILogger<HomeController> _logger;
        public HomeController (IStringLocalizer<HomeController> localizer, ILogger<HomeController> logger){
            _localizer = localizer;
            _logger = logger;
        }
        
        public IActionResult About() => View();
        public IActionResult Index() => View();
        public IActionResult Forbbiden() => View();

        public IActionResult Pricing() => View();
        public IActionResult Error(int? id)
        {
            if(id == null || id == 404 ){
                HttpContext.Response.StatusCode = 404;
                return View("404");
            }
            HttpContext.Response.StatusCode = 500;
            return View("500");
        }

        public IActionResult RedirectToDefaultLanguage()
        {
            return RedirectToAction("Index", new { lang = CurrentLanguage });
        }

        private string _currentLanaguage;
        private string CurrentLanguage
        {
            get{
                if(!string.IsNullOrEmpty(_currentLanaguage)){
                    return _currentLanaguage;
                }
                if(RouteData.Values.ContainsKey("lang")){
                    _currentLanaguage = RouteData.Values["lang"].ToString().ToLower();
                }
                if(string.IsNullOrEmpty(_currentLanaguage)){
                    var feature  = HttpContext.Features.Get<IRequestCultureFeature>();
                    _currentLanaguage = feature.RequestCulture.Culture.TwoLetterISOLanguageName.ToLower();
                }
                return _currentLanaguage;
            }
        }
    }
}
