using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BuildingAdmin.BussinessLogic.Email.EmailType;

namespace BuildingAdmin.BussinessLogic.Email.Contracts
{
    public interface IEmailService
    {
        Task Send(IEmailMessage emailMessage);
    } 
}