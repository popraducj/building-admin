using BuildingAdmin.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;
using MongoDbGenericRepository;
using Moq;
using Xunit;

namespace BuildingAdmin.Tests.Controllers
{
    public class BillsControllerTests
    {
        private Mock<IBaseMongoRepository> _repository;
        public BillsControllerTests(){
            _repository = new Mock<IBaseMongoRepository>();
        }

        [Fact]
        public void GetShould_ReturnGetView(){
            var controller = GetController();
            var result = controller.Get();

            Assert.NotNull(result);
            var viewResult = result as ViewResult;
            Assert.Equal("Get", viewResult.ViewName);
        }

        [Fact]
        public void AddShould_ReturnAddView(){
            var controller = GetController();
            var result = controller.Add();

            Assert.NotNull(result);
            var viewResult = result as ViewResult;
            Assert.Equal("Add", viewResult.ViewName);
        }
        private BillController GetController(){
            return new BillController(_repository.Object);
        }
    }
}