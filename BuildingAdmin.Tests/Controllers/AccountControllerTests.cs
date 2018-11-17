using System;
using System.Threading.Tasks;
using System.Linq;
using BuildingAdmin.BussinessLogic.Cryptography;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.Mvc.Controllers;
using BuildingAdmin.Mvc.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MongoDbGenericRepository;
using Moq;
using Xunit;
using BuildingAdmin.Tests.Helper;

namespace BuildingAdmin.Tests.Controllers{
    public class AccountControllerTests{
        private Mock<IConfiguration> _config;
        private Mock<IBaseMongoRepository> _repository;
        private Mock<IOptions<HashingConfig>> _hashingConfig;
        private Mock<IDataProtectionProvider> _protector;
        private Mock<IStringLocalizer<string>> _localizer;
        private Mock<IArgonHash> _argonHash;
        public AccountControllerTests(){
            _config = new Mock<IConfiguration>();
            _repository = new Mock<IBaseMongoRepository>();
            _hashingConfig = new Mock<IOptions<HashingConfig>>();
            _protector = new Mock<IDataProtectionProvider>();
            _localizer = new Mock<IStringLocalizer<string>>();
            _argonHash = new Mock<IArgonHash>();
        }

        [Fact]
        public void Login_ReturnsAView(){
            var controller = new AccountController(_hashingConfig.Object, _repository.Object, _protector.Object, _config.Object, _localizer.Object, _argonHash.Object);
            var result = controller.Login();
            Assert.IsType<ViewResult>(result);            
        }

        [Fact]
        public async Task Login_Post_ReturnsAnError_IfUserNotFound(){
            _repository.Setup(repo => repo.GetById<UserModel, string>("", null)).Returns(HelperObjects.GetUser());
            var key = "Invalid email or wrong password";
            _localizer.Setup(_ => _[key]).Returns(new LocalizedString(key, key));
            var controller = new AccountController(_hashingConfig.Object, _repository.Object, _protector.Object, _config.Object, _localizer.Object, _argonHash.Object);
            var result = await controller.Login(GetUserViewModel());
            Assert.IsType<ViewResult>(result);
            Assert.Equal(1, controller.ModelState.ErrorCount);
        }

        [Fact]
        public async Task Login_Post_ReturnsAnError_IfModelIsNotValid(){            
            var controller = new AccountController(_hashingConfig.Object, _repository.Object, _protector.Object, _config.Object, _localizer.Object, _argonHash.Object);
            controller.ModelState.AddModelError("", "Required");
            var result = await controller.Login(GetUserViewModel());
            Assert.IsType<ViewResult>(result);
            Assert.Equal(1, controller.ModelState.ErrorCount);
        }

        [Fact]
        public async Task Login_Post_ReturnsAnError_IfWrongPassword(){
            var userViewModel = GetUserViewModel();
            var dbUser = HelperObjects.GetUser();
            var tupleToReturn = Tuple.Create<string, string>(userViewModel.Password, dbUser.Salt);
            _repository.Setup(repo => repo.GetById<UserModel, string>(userViewModel.Email, null)).Returns(dbUser);
            _argonHash.Setup(hash => hash.Encoder(userViewModel.Password, _hashingConfig.Object.Value, dbUser.Salt)).Returns(GetEncodingResult());
            var key = "Invalid email or wrong password";
            _localizer.Setup(_ => _[key]).Returns(new LocalizedString(key, key));

            var controller = new AccountController(_hashingConfig.Object, _repository.Object, _protector.Object, _config.Object, _localizer.Object, _argonHash.Object);
            var result = await controller.Login(userViewModel);
            Assert.IsType<ViewResult>(result);
            Assert.Equal(1, controller.ModelState.ErrorCount);
            Assert.Equal(key, controller.ModelState.Values.First().Errors.First().ErrorMessage);
        }

        [Fact]
        public void Register_ReturnsAView(){
            var controller = new AccountController(_hashingConfig.Object, _repository.Object, _protector.Object, _config.Object, _localizer.Object, _argonHash.Object);
            var result = controller.Register();
            Assert.IsType<ViewResult>(result);            
        }
        
         [Fact]
        public void ForgetPassword_ReturnsAView(){
            var controller = new AccountController(_hashingConfig.Object, _repository.Object, _protector.Object, _config.Object, _localizer.Object, _argonHash.Object);
            var result = controller.ForgetPassword();
            Assert.IsType<ViewResult>(result);            
        }

        private UserViewModel GetUserViewModel()
        {
            return new UserViewModel(){
                Email = HelperConstants.TestEmailAddress,
                Password = "test"
            };
        }

        private EncodingResult GetEncodingResult()
        {
            return new EncodingResult(){
                Hash = "encodedPassword1",
                Salt = "salt"
            };
        }
    }
}