using System;
using System.IO;
using MimeKit;
using BuildingAdmin.DataLayer.Models;
using MongoDbGenericRepository;

namespace BuildingAdmin.BussinessLogic.Email.EmailType
{
    public class ForgotPassword : EmailBody
    {
        public ForgotPassword(IBaseMongoRepository repository) : base(repository) { }

        public string Link {get;set;}
        public override void SetEmailTemplate()
        {
            SetTemplate(EmailTypeEnum.ForgotPassword);
            Template = Template.Replace("{{user}}", Name);
            Template = Template.Replace("{{emailConfirmationLink}}", Link);
        }
    }
}