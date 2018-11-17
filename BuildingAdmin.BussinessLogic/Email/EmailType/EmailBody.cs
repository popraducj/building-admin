using System;
using MimeKit;
using BuildingAdmin.DataLayer.Models;
using MongoDbGenericRepository;

namespace BuildingAdmin.BussinessLogic.Email.EmailType
{
    public abstract class EmailBody
    {
        private IBaseMongoRepository _repository;
        public EmailBody(IBaseMongoRepository repository){
            _repository = repository;
        }
        public string Name {get;set;}

        public string Subject {get; protected set;}

        public MailboxAddress FromAdress {get; protected set;}

        public abstract void SetEmailTemplate();

        public string Language { get; set; }

        public string Template {get;set; }

        protected void SetTemplate(EmailTypeEnum type)
        {
            var body = _repository.GetOne<EmailTemplate>(x => x.Language.Equals(Language) && x.EmailType.Equals(type));
            Subject = body.Subject;
            FromAdress = new MailboxAddress(
                "Building-admin.com",
                body.Address
            );
            Template = body.Template;
        }
    }
    
}