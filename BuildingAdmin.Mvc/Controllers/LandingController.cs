using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDbGenericRepository;

namespace BuildingAdmin.Mvc.Controllers
{
    public class LandingController: Controller
    {
        public IBaseMongoRepository _repository;

        public LandingController(IBaseMongoRepository repository){
            _repository = repository;
        }

        [Authorize(Policy ="admin")]
        public ActionResult Index(){
            return View();
        }
        
    }
}