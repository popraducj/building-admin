using Microsoft.Extensions.DependencyInjection;
using BuildingAdmin.BussinessLogic.Email;
using BuildingAdmin.BussinessLogic.Email.Contracts;
using BuildingAdmin.BussinessLogic.Email.Implementation;
using BuildingAdmin.BussinessLogic.Cryptography;
using BuildingAdmin.BussinessLogic.Email.EmailType;

namespace BuildingAdmin.BussinessLogic
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddEmailService(this IServiceCollection service){

            service.AddTransient<IEmailService, EmailService>();
            service.AddTransient<IEmailMessage, EmailMessage>();
            service.AddTransient<IArgonHash, ArgonHash>();
            service.AddTransient<EmailAddress>();
            service.AddTransient<EmailConfirmation>();
            service.AddTransient<Registration>();
            service.AddTransient<ForgotPassword>();
            return service;
        }
    }
}