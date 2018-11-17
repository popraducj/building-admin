using BuildingAdmin.BussinessLogic.Email.Contracts;
using BuildingAdmin.BussinessLogic.Email.EmailType;
using BuildingAdmin.BussinessLogic.Email.Implementation;
using BuildingAdmin.Mvc.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using MongoDbGenericRepository;

namespace BuildingAdmin.Mvc.Controllers
{
    public class ContactController : Controller
    {
        private IBaseMongoRepository _repository;
        private IEmailService _emailService;
        private IEmailMessage _message;
        private EmailAddress _emailAddress;
        private IDataProtector _protector;
        public ContactController (IBaseMongoRepository repository,
             IEmailService emailService, IEmailMessage emailMessage, EmailAddress emailAddress, IDataProtectionProvider provider){

            _repository = repository;
            _emailService = emailService;
            _message = emailMessage;
            _emailAddress = emailAddress;            
            _protector = provider.CreateProtector("email");
        }
         public IActionResult Index() => View();
        [HttpPost]
        public IActionResult Index(ContactModel model){
            
            _message.EmailContent = new ContactEmail(model.Email, model.Name, model.Message);
            
            _emailAddress.Address = "radupopcj@gmail.com";
            _emailAddress.Name = "Radu Pop";
            _message.ToAddresses.Add(_emailAddress);
                
            _emailService.Send(_message);
            return RedirectToAction("Index","Thankyou");
        }
    }
}