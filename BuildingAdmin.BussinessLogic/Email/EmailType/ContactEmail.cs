using MimeKit;

namespace BuildingAdmin.BussinessLogic.Email.EmailType
{ 
    public class ContactEmail : EmailBody
    {
        public ContactEmail(string email, string name, string body) : base(null) {
            FromAdress = new MailboxAddress(
                "Contact building Admin",
                "no-replay@building-admin.com"
            );
            Template = string.Format("Name: {0} <br/ > Email: {1} <br/> Message: {2} ",name, email, body);
            Subject = "New feedback";
        }
        public override void SetEmailTemplate() { }
    }
}