using System;
using System.IO;
using MimeKit;
using BuildingAdmin.DataLayer.Models;
using MongoDbGenericRepository;

namespace BuildingAdmin.BussinessLogic.Email.EmailType
{
    public class Registration : EmailBody
    {
        public Registration(IBaseMongoRepository repository): base(repository) { }

        public override void SetEmailTemplate()
        {
            SetTemplate(EmailTypeEnum.Registration);
            Template = Template.Replace("{{user}}", Name);
        }
    }
}