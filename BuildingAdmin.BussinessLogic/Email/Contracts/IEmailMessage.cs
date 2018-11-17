using System;
using System.Collections.Generic;
using BuildingAdmin.BussinessLogic.Email.EmailType;
using BuildingAdmin.BussinessLogic.Email.Implementation;

namespace BuildingAdmin.BussinessLogic.Email.Contracts
{
    public interface IEmailMessage
    {
        List<EmailAddress> ToAddresses {get;set;}
        EmailBody EmailContent { get; set; }
    }
}