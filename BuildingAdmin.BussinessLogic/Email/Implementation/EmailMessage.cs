using System;
using System.Collections.Generic;
using BuildingAdmin.BussinessLogic.Email.EmailType;
using BuildingAdmin.BussinessLogic.Email.Contracts;

namespace BuildingAdmin.BussinessLogic.Email.Implementation
{
    public class EmailMessage : IEmailMessage
    {
        public List<EmailAddress> ToAddresses {get;set;} = new List<EmailAddress>();
        public EmailBody EmailContent { get; set; }
    }
}