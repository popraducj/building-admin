using BuildingAdmin.Mvc.Controllers;
using MongoDbGenericRepository;
using Moq;

namespace BuildingAdmin.Tests.Controllers
{
    public class ApartmentControllerTests
    {
        private Mock<IBaseMongoRepository> _repository;

        public ApartmentControllerTests(){
            _repository = new Mock<IBaseMongoRepository>();
        }
        private ApartmentController CreateController(){
            return new ApartmentController(_repository.Object);
        }
    }
}