using System;
using System.Collections.Generic;
using System.Linq;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text;
using BuildingAdmin.BussinessLogic.Email.EmailType;
using BuildingAdmin.BussinessLogic.Email.Contracts;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace BuildingAdmin.BussinessLogic.Email.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
    
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            //TO-DO sa verific cate mailuri au fost trimite intr-o zi/luna si sa schimb configurarile
        }
    
        public async Task Send(IEmailMessage emailMessage)
        {
            var message = new MimeMessage();
            emailMessage.EmailContent.SetEmailTemplate();
            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.From.Add(emailMessage.EmailContent.FromAdress);
        
            message.Subject = emailMessage.EmailContent.Subject;
            //We will say we are sending HTML. But there are options for plaintext etc. 
            message.Body = new TextPart("html")
            {
                Text = emailMessage.EmailContent.Template
            };
        
            using (var emailClient = new SmtpClient())
            {
                //The last parameter here is to use SSL 
                emailClient.Connect(_configuration["EmailConfiguration:SmtpServer"], int.Parse(_configuration["EmailConfiguration:SmtpPort"]), false);
        
                //Remove any OAuth functionality as we won't be using it. 
                emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
        
                emailClient.Authenticate(_configuration["EmailConfiguration:SmtpUsername"], _configuration["EmailConfiguration:SmtpPassword"]);
                emailClient.Send(message);
                emailClient.Disconnect(true);
            }
        }
    }
}
