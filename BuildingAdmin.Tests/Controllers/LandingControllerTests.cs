using BuildingAdmin.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using MongoDbGenericRepository;
using Moq;
using Xunit;

namespace BuildingAdmin.Tests.Controllers
{
    public class LandingControllerTests
    {
        private Mock<IBaseMongoRepository> _repository;

        public LandingControllerTests(){
            _repository = new Mock<IBaseMongoRepository>();
        }

        [Fact]
        public void Index_ReturnAView(){
            var controller = CreateController();
            var result = controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        private LandingController CreateController (){
            return new LandingController(_repository.Object);
        }
    }
}