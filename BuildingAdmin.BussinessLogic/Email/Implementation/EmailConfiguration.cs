using System;
using System.Collections.Generic;
using BuildingAdmin.BussinessLogic.Email.Contracts;

namespace BuildingAdmin.BussinessLogic.Email.Implementation
{	
    public class EmailConfiguration :IEmailConfiguration
    {
        public string SmtpServer { set; get; }
        public int SmtpPort {set; get; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
    }
}