using BuildingAdmin.Mvc.Controllers;
using MongoDbGenericRepository;
using Moq;

namespace BuildingAdmin.Tests.Controllers
{
    public class BuildingControllerTests
    {
        private Mock<IBaseMongoRepository> _repository;

        public BuildingControllerTests(){
            _repository = new Mock<IBaseMongoRepository>();
        }

        private BuildingController CreateController(){
            return new BuildingController(_repository.Object);
        }
    }
}