using System;
using System.Collections.Generic;

namespace BuildingAdmin.BussinessLogic.Email.Contracts
{	
    public interface IEmailConfiguration
    {
        string SmtpServer {set; get; }
        int SmtpPort { set;get; }
        string SmtpUsername { get; set; }
        string SmtpPassword { get; set; }    
    }
}