using System;
using System.IO;
using MimeKit;
using BuildingAdmin.DataLayer.Models;
using MongoDbGenericRepository;

namespace BuildingAdmin.BussinessLogic.Email.EmailType
{
    public class EmailConfirmation : EmailBody
    {
        public EmailConfirmation(IBaseMongoRepository repository) : base(repository) {}
        public string Link {get;set;}

        public override void SetEmailTemplate()
        {
            SetTemplate(EmailTypeEnum.EmailConfirmation);
            Template = Template.Replace("{{user}}", Name);
            Template = Template.Replace("{{emailConfirmationLink}}", Link);
            Template = Template.Replace("\\\"", "\"");
        }
    }
}