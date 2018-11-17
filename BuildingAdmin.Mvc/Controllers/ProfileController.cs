using System.Security.Claims;
using System.Threading.Tasks;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.Mvc.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDbGenericRepository;

namespace BuildingAdmin.Mvc.Controllers
{
    public class ProfileController : Controller{

        private IBaseMongoRepository _repository;
        private IStringLocalizer _localizer;
        public ProfileController(IBaseMongoRepository repository, IStringLocalizer<ProfileController> localizer){
            _repository = repository;
            _localizer = localizer;
        }

        [Authorize]
        public IActionResult Index(){

            var user = _repository.GetById<UserModel, string>(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if(user == null || !user.State.Equals(BuildingAdmin.DataLayer.Models.UserStatesEnum.Active)){
                return StatusCode(404);
            }
            return View(new ProfileModel(user));
        }
        [Authorize]
        public IActionResult Edit(){

            var user = _repository.GetById<UserModel, string>(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if(user == null || !user.State.Equals(BuildingAdmin.DataLayer.Models.UserStatesEnum.Active)){
                return StatusCode(404);
            }
            return View(new ProfileModel(user));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(ProfileModel model){

            if(!ModelState.IsValid){
                return View(model);
            }
            var user = _repository.GetById<UserModel, string>(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if(user == null || !user.State.Equals(BuildingAdmin.DataLayer.Models.UserStatesEnum.Active)){
                return StatusCode(404);
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Description = model.Description;
            user.Phone = model.Phone;
            user.Address = model.Address;
            await _repository.UpdateOneAsync<UserModel, string>(user);
            return RedirectToAction(nameof(Index));
        }
    }
}