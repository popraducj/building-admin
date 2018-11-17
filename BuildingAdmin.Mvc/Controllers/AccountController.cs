using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using BuildingAdmin.Mvc.Models;
using BuildingAdmin.DataLayer.Repository;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.BussinessLogic.Email;
using BuildingAdmin.BussinessLogic.Email.EmailType;
using Microsoft.Extensions.Options;
using BuildingAdmin.BussinessLogic.Email.Contracts;
using BuildingAdmin.BussinessLogic.Email.Implementation;
using BuildingAdmin.BussinessLogic.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using BuildingAdmin.BussinessLogic;
using Microsoft.Extensions.Localization;
using MongoDbGenericRepository;

namespace BuildingAdmin.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private IConfiguration _config;
        private IBaseMongoRepository _repository;
        private IOptions<HashingConfig> _hashingConfig;
        private IDataProtector _protector;
        private IStringLocalizer _localizer;
        private IArgonHash _argonHash;
        public AccountController(IOptions<HashingConfig> hashingConfig, IBaseMongoRepository repository, IDataProtectionProvider provider,
                                 IConfiguration config, IStringLocalizer<string> localizer, IArgonHash argonHash)
        {
            _repository = repository;
            _hashingConfig = hashingConfig;
            _protector = provider.CreateProtector("email");
            _config = config;
            _localizer = localizer;
            _argonHash = argonHash;
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        public async Task<IActionResult> Login(UserViewModel model)
        {
            SetCaptchaErrors();
            if(!ModelState.IsValid){
                return View(model);
            }
            var user = _repository.GetById<UserModel, string>(model.Email);
            if(user == null && user.State.Equals(UserStatesEnum.Active)){
                ModelState.AddModelError("", _localizer["Invalid email or wrong password"]);
                return View(model);
            }

            if(user != null && _argonHash.Encoder(model.Password, _hashingConfig.Value, user.Salt).Hash.Equals(user.Password)){               
                var identity = new ClaimsIdentity(new []{
                    new Claim(ClaimTypes.Surname, $"{user.FirstName} {user.LastName}"),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                foreach (var role in user.Roles){
                    identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }

                var principal = new ClaimsPrincipal(identity);                

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties{
                    IsPersistent = model.RememberMe
                });

                return RedirectToAction("Index","Landing");
            }
            ModelState.AddModelError("", _localizer["Invalid email or wrong password"]);
            return View(model);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        public async Task<IActionResult> Register(UserRegistration viewModel){
            
            SetCaptchaErrors("Captcha");
            if(!viewModel.Terms){
                ModelState.AddModelError("Terms", _localizer["The terms and conditions must be accepted"]);
            }

            if(!ModelState.IsValid){
                return View(viewModel);
            }
            if(_repository.GetById<UserModel, string>(viewModel.Email) != null){
                  ModelState.AddModelError("Email", _localizer["The email address already exists in our database"]);
            }
            if(!ModelState.IsValid){
                return View(viewModel);
            }

            var encodingResult = _argonHash.Encoder(viewModel.Password, _hashingConfig.Value);

            
            // save user into db
            await _repository.AddOneAsync<UserModel, string>(new UserModel(){
                Id = viewModel.Email,
                Password = encodingResult.Hash,
                Salt = encodingResult.Salt,
                CreatedAt = DateTime.Now,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                State = UserStatesEnum.NotConfirmed,
                Roles = new List<string>(){ "admin"}
            });

            return RedirectToAction("NewAccount", "Email", new { id = _protector.Protect(viewModel.Email)});
        }

        public async Task<IActionResult> Logout(){

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        public IActionResult ForgetPassword(ForgotPasswordModel model){

            var user = _repository.GetById<UserModel, string>(model.Email);
            if(user == null){
                ModelState.AddModelError("", _localizer["The email address was not found"]);
                return View();
            }
            else if(user.State.Equals(UserStatesEnum.NotConfirmed))
            {
                ModelState.AddModelError("", _localizer["The email address was not confirmed"]);
                return View();
            }
            return RedirectToAction("ForgetPassword","Email", new { id =user.Id });
        }

        public async Task<ActionResult> Activate(string id){
            var user = _repository.GetById<UserModel, string>(_protector.Unprotect(id).Substring(8));
            if (user == null){
                return StatusCode(404);
            }
            user.State = UserStatesEnum.Active;
            user.Token = null;
            user.ExpireDate = null;
            await _repository.UpdateOneAsync<UserModel, string>(user);
            return View(); 
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidateReCaptchaAttribute))]
        public async Task<IActionResult> ChangePassword(ChangePassword model)
        { 
            if(!ModelState.IsValid){
                return StatusCode(404);
            }
            var user = _repository.GetById<UserModel, string>(_protector.Unprotect(model.Token).Substring(8));
            if (user == null){
                return StatusCode(404);
            }
            var encoding = _argonHash.Encoder(model.Password, _hashingConfig.Value);
            user.Password = encoding.Hash;
            user.Salt = encoding.Salt;
            user.Token = null;
            
            await _repository.UpdateOneAsync<UserModel, string>(user);
            return View("ChangePasswordConfirmation");
        }

        public async Task<IActionResult> ChangePassword(string id){

            var user = _repository.GetById<UserModel, string>(_protector.Unprotect(id).Substring(8));
            if(user == null || user.ExpireDate == null){
                return StatusCode(404);
            }

            if(user.ExpireDate < DateTime.Now.ToUniversalTime()){
                user.State = UserStatesEnum.NotConfirmed;
                user.Token = null;
                await _repository.UpdateOneAsync<UserModel, string>(user);
                return StatusCode(404);
            }            

            return View(new ChangePassword(){Token = id});
        }

        public IActionResult Register() => View(new UserRegistration());
        public IActionResult Login() =>View();
        public IActionResult ForgetPassword() => View();

        private void SetCaptchaErrors(string field = ""){

            foreach(var modelstate in ModelState.Values){
                foreach(var error in modelstate.Errors){
                    
                    if( error.ErrorMessage.Equals(Constant.InvalidCaptcha)){
                        ModelState.AddModelError(field, error.ErrorMessage);
                        modelstate.Errors.Clear();
                        break;
                    } 
                }
            }
        }
    }
}