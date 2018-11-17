using Microsoft.AspNetCore.Mvc;
using BuildingAdmin.BussinessLogic.Email.Contracts;
using BuildingAdmin.BussinessLogic.Email.Implementation;
using BuildingAdmin.Mvc.Models;
using Microsoft.AspNetCore.DataProtection;
using BuildingAdmin.BussinessLogic.Email.EmailType;
using BuildingAdmin.DataLayer.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using BuildingAdmin.BussinessLogic;
using System.Threading;
using MongoDbGenericRepository;

namespace BuildingAdmin.Mvc.Controllers
{
    public class EmailController : Controller
    {
        private IConfiguration _config;
        private MongoDbGenericRepository.IBaseMongoRepository _repository;
        private IEmailService _emailService;
        private IEmailMessage _message;
        private EmailAddress _emailAddress;
        private IDataProtector _protector;
        private EmailConfirmation _emailConfirmation;
        private Registration _registration;
        private ForgotPassword _forgotPassword;
        public EmailController(IEmailService emailService, IEmailMessage message, IDataProtectionProvider provider, IConfiguration config,
                               EmailAddress address, IBaseMongoRepository repository, EmailConfirmation emailConfirmation,
                               Registration registration, ForgotPassword forgotPassword){
            _repository = repository;
            _emailService = emailService;
            _message = message;
            _emailAddress = address;
            _protector = provider.CreateProtector("email");
            _config = config;
            _registration = registration;
            _emailConfirmation = emailConfirmation;
            _forgotPassword = forgotPassword;
        }

        public async Task<ActionResult> Confirm(string id){
            var user = _repository.GetById<UserModel, string>(_protector.Unprotect(id));
            if(user == null || !user.State.Equals(BuildingAdmin.DataLayer.Models.UserStatesEnum.NotConfirmed)){
               
               return StatusCode(404);
            }

            var token = await UpdateToken(60*24*30, user);
            
            _emailConfirmation.Name = user.FirstName;
            _emailConfirmation.Link = $"{_config["URL"]}/account/activate?id={token}";
            _emailConfirmation.Language = "ro";
            _message.EmailContent = _emailConfirmation;

            SendMessage(user);
            
            ViewBag.Email = user.Id;
            ViewBag.Resend = id;
            return View();
        }

        public ActionResult NewAccount(string id)
        {
            var user = _repository.GetById<UserModel, string>(_protector.Unprotect(id));
            if(user == null || !user.State.Equals(BuildingAdmin.DataLayer.Models.UserStatesEnum.NotConfirmed))
            {
                return StatusCode(404);
            }
            _registration.Name = user.FirstName;
            _registration.Language = user.Language;
            _message.EmailContent = _registration;
            SendMessage(user);
            return RedirectToAction(nameof(Confirm), new{ id = id});
        }

        public async Task<IActionResult> ForgetPassword(string id){

            var user = _repository.GetById<UserModel, string>(id);
            if(user == null || user.State.Equals(BuildingAdmin.DataLayer.Models.UserStatesEnum.NotConfirmed)){
                return StatusCode(404);
            }
            var token = await UpdateToken(60*24, user);

            _forgotPassword.Language = "ro";
            _forgotPassword.Name = user.FirstName;
            _forgotPassword.Link = $"{_config["URL"]}/Account/ChangePassword?id={token}";
            _message.EmailContent = _forgotPassword;

            SendMessage(user);

            ViewBag.Email = user.Id;
            return View();
        }

        private void SendMessage(UserModel user){
            _emailAddress.Address = user.Id;
            _emailAddress.Name = $"{user.FirstName} {user.LastName}";
            _message.ToAddresses.Add(_emailAddress);
                
            _emailService.Send(_message);
        }

        private async Task<string> UpdateToken(int delayInMinutes, UserModel user){

            user.ExpireDate = DateTime.Now.AddMinutes(delayInMinutes);
            var token = _protector.Protect(user.ExpireDate.GetValueOrDefault().ToString(Constant.DateFormat) + user.Id);
            user.Token = token;
            await _repository.UpdateOneAsync<UserModel, string>(user);
            return token;
        }
    }
}