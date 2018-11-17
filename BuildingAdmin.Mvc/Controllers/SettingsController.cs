using System.Security.Claims;
using System.Threading.Tasks;
using BuildingAdmin.BussinessLogic.Cryptography;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.Mvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MongoDbGenericRepository;

namespace BuildingAdmin.Mvc.Controllers
{
    public class SettingsController: Controller
    {
        private IBaseMongoRepository _repository;
        private IStringLocalizer<SettingsController> _localizer;
        private IArgonHash _hash;
        private IOptions<HashingConfig> _hashingConfig;

        public SettingsController(IBaseMongoRepository repository, IStringLocalizer<SettingsController> localizer, IArgonHash hash, IOptions<HashingConfig> hashingConfig){
            _repository = repository;
            _localizer =  localizer;
            _hash = hash;
            _hashingConfig = hashingConfig;
        }

        public ActionResult Index(){
            var user = _repository.GetById<UserModel, string>(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if(user == null || !user.State.Equals(BuildingAdmin.DataLayer.Models.UserStatesEnum.Active)){
                return StatusCode(404);
            }
            return View(new SettingsModel(user));
        }

        [HttpPost]
         public async Task<ActionResult> Edit(SettingsModel settings){

            if (!ModelState.IsValid){
                return View("Index", settings);
            }

            var user = _repository.GetById<UserModel, string>(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if(user == null || !user.State.Equals(BuildingAdmin.DataLayer.Models.UserStatesEnum.Active)){
                return StatusCode(404);
            }
            user.FirstName = settings.FirstName;
            user.LastName = settings.LastName;
            
            await _repository.UpdateOneAsync<UserModel, string>(user);
            ViewBag.Success = _localizer["The changes have been saved successfully"];
            return View("Index", new SettingsModel(user));
        }

        [HttpPost]
         public async Task<ActionResult> ChangePassword(SettingsChangePassword settings){


            var user = _repository.GetById<UserModel, string>(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if(user == null || !user.State.Equals(BuildingAdmin.DataLayer.Models.UserStatesEnum.Active)){
                return StatusCode(404);
            }

            if (!ModelState.IsValid){
                ViewBag.Error = _localizer["The changes could not be saved"];
                return View("Index", new SettingsModel(user));
            }

            if(!_hash.Encoder(settings.OldPassword, _hashingConfig.Value, user.Salt).Hash.Equals(user.Password)){
                ViewBag.Error = _localizer["The changes could not be saved"];
                return View("Index", new SettingsModel(user));
            }
            
            var hash = _hash.Encoder(settings.Password, _hashingConfig.Value);
            user.Password = hash.Hash;
            user.Salt = hash.Salt;
            
            await _repository.UpdateOneAsync<UserModel, string>(user);
            ViewBag.Success = _localizer["The changes have been saved successfully"];
            return View("Index", new SettingsModel(user));
        }
    }
}