using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.Mvc.Controllers;
using BuildingAdmin.Mvc.Models;
using BuildingAdmin.Tests.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MongoDbGenericRepository;
using Moq;
using Xunit;

namespace BuildingAdmin.Tests.Controllers
{
    public class  ProfileControllerTests
    {
        private Mock<IBaseMongoRepository> _repository;
        private Mock<IStringLocalizer<ProfileController>> _localizer;
        private ClaimsPrincipal _user;
        public ProfileControllerTests()
        {
            _repository = new Mock<IBaseMongoRepository>();
            _localizer = new Mock<IStringLocalizer<ProfileController>>();
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
            }));
        }

        [Fact]
        public void Index_ReturnsAView(){
            var controller = SetUpBasicUserValidation("1");
            var result = controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Index_ReturnsNotFound_IfLoggedUserIsNotFound(){
            var controller = SetUpBasicUserValidation("0");
            var result = controller.Index();
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, (int)statusResult.StatusCode);
        }

        [Fact]
        public void Index_ReturnsNotFound_IfUserIsNotActive(){
            var controller = SetUpBasicUserValidation("0", UserStatesEnum.NotConfirmed);
            var result = controller.Index();
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, (int)statusResult.StatusCode);
        }
        
        [Fact]
        public void Edit_ReturnsAView(){
            var controller = SetUpBasicUserValidation("1");
            var result = controller.Edit();
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public void Edit_ReturnsNotFound_IfLoggedUserIsNotFound(){
            var controller = SetUpBasicUserValidation("0");
            var result = controller.Edit();
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, (int)statusResult.StatusCode);
        }

        [Fact]
        public void Edit_ReturnsNotFound_IfUserIsNotActive(){
            var controller = SetUpBasicUserValidation("0", UserStatesEnum.NotConfirmed);
            var result = controller.Edit();
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, (int)statusResult.StatusCode);
        }

        [Fact]
        public async Task Edit_Post_ReturnsAView(){
            var controller = SetUpBasicUserValidation("1");
            var result = await controller.Edit(new ProfileModel());
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToAction.ActionName);
            Assert.Null(redirectToAction.ControllerName);
        }

        [Fact]
        public async Task Edit_Post_ReturnsNotFound_IfLoggedUserIsNotFound(){
            var controller = SetUpBasicUserValidation("0");
            var result = await controller.Edit(new ProfileModel());
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, (int)statusResult.StatusCode);
        }

        [Fact]
        public async Task Edit_Post_ReturnsNotFound_IfUserIsNotActive(){
            var controller = SetUpBasicUserValidation("1", UserStatesEnum.NotConfirmed);
            var result = await controller.Edit(new ProfileModel());
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, (int)statusResult.StatusCode);
        }

        [Fact]
        public async Task Edit_Post_ReturnsModelState_IfInvalid(){
         
            var controller = CreateController();
            controller.ModelState.AddModelError("Password", "Wrong Password");
            var result = await controller.Edit(new ProfileModel());
            Assert.IsType<ViewResult>(result);
            _repository.Verify(repo => repo.GetById<UserModel, string>("", null), Times.Never());
        }

        private ProfileController CreateController(){
            return new ProfileController(_repository.Object, _localizer.Object);
        }
        private ProfileController SetUpBasicUserValidation(string name, UserStatesEnum state = UserStatesEnum.Active){
            _repository.Setup(repo => repo.GetById<UserModel, string>(name, null)).Returns(HelperObjects.GetUser(state));
            var controller = CreateController();
            controller.ControllerContext = new ControllerContext(){
                HttpContext = new DefaultHttpContext() {User = _user}
            };
            return controller;
        }
    }
}