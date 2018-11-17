using System.Security.Claims;
using System.Threading.Tasks;
using BuildingAdmin.BussinessLogic.Cryptography;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.Mvc.Controllers;
using BuildingAdmin.Mvc.Models;
using BuildingAdmin.Tests.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MongoDbGenericRepository;
using Moq;
using Xunit;

namespace BuildingAdmin.Tests.Controllers
{
    public class SettingsControllerTests
    {
        private Mock<IBaseMongoRepository> _repository;
        private Mock<IStringLocalizer<SettingsController>> _localizer;
        private Mock<IArgonHash> _hash;
        private Mock<IOptions<HashingConfig>> _hashingConfig;
        private ClaimsPrincipal _user;

        public SettingsControllerTests(){
            _repository = new Mock<IBaseMongoRepository>();
            _localizer = new Mock<IStringLocalizer<SettingsController>>();
            _hash = new Mock<IArgonHash>();
            _hashingConfig = new Mock<IOptions<HashingConfig>>();
            _user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
            }));
        }

        #region index
        [Fact]
        public void Index_ReturnAView(){
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
        #endregion;
        #region  edit post
        [Fact]
        public async Task Edit_Post_ReturnsAView(){
            var key = "The changes have been saved successfully";
            _localizer.Setup(_ => _[key]).Returns(new LocalizedString(key,key));
            var controller = SetUpBasicUserValidation("1");
            var result = await controller.Edit(new SettingsModel());
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async Task Edit_Post_ReturnsError_IfInvalidModel(){
            var key = "The changes have been saved successfully";
            _localizer.Setup(_ => _[key]).Returns(new LocalizedString(key,key));
            var controller = SetUpBasicUserValidation("1");
            controller.ModelState.AddModelError("", "last name");
            var result = await controller.Edit(new SettingsModel());
            Assert.IsType<ViewResult>(result);
            _repository.Verify(repo => repo.GetById<UserModel, string>("1", null), Times.Never());
        }

        [Fact]
        public async Task Edit_Post_ReturnsNotFound_IfLoggedUserIsNotFound(){
            var controller = SetUpBasicUserValidation("0");
            var result = await controller.Edit(new SettingsModel());
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, (int)statusResult.StatusCode);
        }

        [Fact]
        public async Task Edit_Post_ReturnsNotFound_IfUserIsNotActive(){
            var controller = SetUpBasicUserValidation("1", UserStatesEnum.NotConfirmed);
            var result = await controller.Edit(new SettingsModel());
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, (int)statusResult.StatusCode);
        }
        #endregion;

        #region changePassword
        [Fact]
        public async Task ChangePassword_Post_ReturnsAView(){
            var user = HelperObjects.GetUser();
            _hash.Setup(hash => hash.Encoder(user.Password, _hashingConfig.Object.Value, user.Salt)).Returns(GetEncodingResult());
            _hash.Setup(hash => hash.Encoder("encodedPassword", _hashingConfig.Object.Value)).Returns(GetEncodingResult());
            var controller = SetUpBasicUserValidation("1");
            var result = await controller.ChangePassword(new SettingsChangePassword() { OldPassword = "encodedPassword", Password = "encodedPassword"});
            Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async Task ChangePassword_Post_ReturnsError_IfCurrentPasswordDoesNotMatch(){
            string oldPassword = "encodedPassword1";
            var user = HelperObjects.GetUser();
            _hash.Setup(hash => hash.Encoder(oldPassword, _hashingConfig.Object.Value, user.Salt)).Returns(GetWrongEncodingResult());
            var controller = SetUpBasicUserValidation("1");
            var result = await controller.ChangePassword(new SettingsChangePassword() { OldPassword = oldPassword});
            Assert.IsType<ViewResult>(result);
            _repository.Verify(repo => repo.UpdateOneAsync<UserModel, string>(user, null), Times.Never());
        }

        [Fact]
        public async Task ChangePassword_Post_ReturnsError_IfInvalidModel(){
            var key = "The changes have been saved successfully";
            _localizer.Setup(_ => _[key]).Returns(new LocalizedString(key,key));
            var controller = SetUpBasicUserValidation("1");
            controller.ModelState.AddModelError("", "last name");
            var result = await controller.ChangePassword(new SettingsChangePassword());
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ChangePassword_Post_ReturnsNotFound_IfLoggedUserIsNotFound(){
            var key = "The changes could not be saved";
            _localizer.Setup(_ => _[key]).Returns(new LocalizedString(key,key));
            var controller = SetUpBasicUserValidation("0");
            var result = await controller.ChangePassword(new SettingsChangePassword());
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, (int)statusResult.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_Post_ReturnsNotFound_IfUserIsNotActive(){
            var key = "The changes could not be saved";
            _localizer.Setup(_ => _[key]).Returns(new LocalizedString(key,key));
            var controller = SetUpBasicUserValidation("1", UserStatesEnum.NotConfirmed);
            var result = await controller.ChangePassword(new SettingsChangePassword());
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(404, (int)statusResult.StatusCode);
        }
        #endregion;
        public SettingsController CreateController(){
            return new SettingsController(_repository.Object, _localizer.Object, _hash.Object, _hashingConfig.Object);
        }
        private SettingsController SetUpBasicUserValidation(string name, UserStatesEnum state = UserStatesEnum.Active){
            _repository.Setup(repo => repo.GetById<UserModel, string>(name, null)).Returns(HelperObjects.GetUser(state));
            var controller = CreateController();
            controller.ControllerContext = new ControllerContext(){
                HttpContext = new DefaultHttpContext() {User = _user}
            };
            return controller;
        }
        private EncodingResult GetEncodingResult()
        {
            return new EncodingResult(){
                Hash = "encodedPassword",
                Salt = "salt"
            };
        }
        private EncodingResult GetWrongEncodingResult()
        {
            return new EncodingResult(){
                Hash = "encodedPassword wrong",
                Salt = "salt"
            };
        }
    }
}