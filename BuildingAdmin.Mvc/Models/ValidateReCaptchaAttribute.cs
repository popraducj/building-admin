using BuildingAdmin.BussinessLogic;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BuildingAdmin.Mvc.Models
{
    public class ValidateReCaptchaAttribute : ActionFilterAttribute
    {
        public const string ReCaptchaModelErrorKey = "ReCaptcha";
        private const string RecaptchaResponseTokenKey = "g-recaptcha-response";
        private const string ApiVerificationEndpoint = "https://www.google.com/recaptcha/api/siteverify";
        private readonly IConfiguration m_configuration;
        private readonly Lazy<string> m_reCaptchaSecret;
        public ValidateReCaptchaAttribute(IConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }
            this.m_configuration = configuration;
            this.m_reCaptchaSecret = new Lazy<string>(() => m_configuration["ReCaptcha:SecretKey"]);
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await DoReCaptchaValidation(context);
            await base.OnActionExecutionAsync(context, next);
        }
        private async Task DoReCaptchaValidation(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.HasFormContentType)
            {
                AddModelError(context, Constant.InvalidCaptcha);
                return;
            }
            string token = context.HttpContext.Request.Form[RecaptchaResponseTokenKey];
            if (string.IsNullOrWhiteSpace(token))
            {
                AddModelError(context, Constant.InvalidCaptcha);
            }
            else
            {
                await ValidateRecaptcha(context, token);
            }
        }
        private static void AddModelError(ActionExecutingContext context, string error)
        {
            context.ModelState.AddModelError(ReCaptchaModelErrorKey, error.ToString());
        }
        private async Task ValidateRecaptcha(ActionExecutingContext context, string token)
        {
            using (var webClient = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                        new KeyValuePair<string, string>("secret", this.m_reCaptchaSecret.Value),
                        new KeyValuePair<string, string>("response", token)
                    });
                HttpResponseMessage response = await webClient.PostAsync(ApiVerificationEndpoint, content);
                var json = await response.Content.ReadAsStringAsync();
                var reCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(json);
                if (reCaptchaResponse == null)
                {
                    AddModelError(context, Constant.InvalidCaptcha);
                }
                else if (!reCaptchaResponse.success)
                {
                    AddModelError(context, Constant.InvalidCaptcha);
                }
            }
        }
    }
    public class ReCaptchaResponse
    {
        public bool success { get; set; }
        public string challenge_ts { get; set; }
        public string hostname { get; set; }
        public string errorcodes { get; set; }
    }
}