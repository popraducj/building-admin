using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BuildingAdmin.BussinessLogic.Email.Contracts;
using BuildingAdmin.BussinessLogic.Email.EmailType;
using BuildingAdmin.BussinessLogic.Email.Implementation;
using BuildingAdmin.DataLayer.Models;
using BuildingAdmin.DataLayer.RepositoryContracts;
using BuildingAdmin.Mvc.Controllers;
using BuildingAdmin.Tests.Helper;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDbGenericRepository;
using Moq;
using Xunit;

namespace BuildingAdmin.Tests.Controllers
{
   public class EmailControllerTests
   {
        private Mock<IConfiguration> _config;
        private Mock<MongoDbGenericRepository.IBaseMongoRepository> _repository;
        private Mock<IEmailService> _emailService;
        private Mock<IEmailMessage> _message;
        private Mock<EmailAddress> _emailAddress;
        private Mock<DataLayer.RepositoryContracts.IBaseMongoRepository> _emailTemplate;
        private Mock<IDataProtectionProvider> _dataProvider;
        private Mock<EmailConfirmation> _confirmationEmail;
        private Mock<Registration> _registrationEmail;
        private Mock<ForgotPassword> _forgotPassword;
        public EmailControllerTests(){
            _repository = new Mock<MongoDbGenericRepository.IBaseMongoRepository>();
            _emailTemplate = new Mock<DataLayer.RepositoryContracts.IBaseMongoRepository>();
            _emailService = new Mock<IEmailService>();
            _message = new Mock<IEmailMessage>();
            _emailAddress = new Mock<EmailAddress>();
            _dataProvider = new Mock<IDataProtectionProvider>();
            _config = new Mock<IConfiguration>();
            var mockReadRepository = new Mock<DataLayer.RepositoryContracts.IBaseMongoRepository>();
            _confirmationEmail = new Mock<EmailConfirmation>(mockReadRepository.Object);
            _registrationEmail = new Mock<Registration>(mockReadRepository.Object);
            _forgotPassword = new Mock<ForgotPassword>(mockReadRepository.Object);

            var mockedProtector = new Mock<IDataProtector>();
            mockedProtector.Setup(protector => protector.Unprotect(It.IsAny<byte[]>())).Returns(Encoding.UTF8.GetBytes(HelperConstants.TestEmailAddress));
            _dataProvider.Setup(s => s.CreateProtector(It.IsAny<string>())).Returns(mockedProtector.Object);
        }

        [Fact]
        public async Task Confirm_WrongEmailAddress(){
            var mockedProtector = new Mock<IDataProtector>();
            mockedProtector.Setup(protector => protector.Unprotect(It.IsAny<byte[]>())).Returns(Encoding.UTF8.GetBytes("wrong@test.com"));
            _dataProvider.Setup(s => s.CreateProtector(It.IsAny<string>())).Returns(mockedProtector.Object);
            _repository.Setup(repo => repo.GetById<UserModel, string>(HelperConstants.TestEmailAddress, null)).Returns(HelperObjects.GetUser(UserStatesEnum.Active));

            var controller = new EmailController(_emailService.Object, _message.Object, _dataProvider.Object, _config.Object, 
                                _emailAddress.Object, _repository.Object, _emailTemplate.Object, _confirmationEmail.Object, _registrationEmail.Object, _forgotPassword.Object);
            var result = await controller.Confirm(string.Empty) as StatusCodeResult;
            Assert.Equal(404, (int)result.StatusCode);
        } 

        [Fact]
        public async Task Confirm_UserIsActive(){
            _repository.Setup(repo => repo.GetById<UserModel, string>(HelperConstants.TestEmailAddress, null)).Returns(HelperObjects.GetUser(UserStatesEnum.Active));

            var controller = new EmailController(_emailService.Object, _message.Object, _dataProvider.Object, _config.Object, 
                                 _emailAddress.Object, _repository.Object, _emailTemplate.Object, _confirmationEmail.Object, _registrationEmail.Object, _forgotPassword.Object);
            var result = await controller.Confirm(string.Empty) as StatusCodeResult;
            Assert.Equal(404, (int)result.StatusCode);
        } 

        [Fact]
        public async Task Confirm_Success(){

            _repository.Setup(repo => repo.GetById<UserModel, string>(HelperConstants.TestEmailAddress, null)).Returns(HelperObjects.GetUser(UserStatesEnum.NotConfirmed));
            _emailService.Setup(email => email.Send(_message.Object));
            _message.SetupProperty(m => m.ToAddresses);
            _message.Object.ToAddresses =new List<EmailAddress>();
            var controller = new EmailController(_emailService.Object, _message.Object, _dataProvider.Object, _config.Object,
                                 _emailAddress.Object, _repository.Object, _emailTemplate.Object, _confirmationEmail.Object, _registrationEmail.Object, _forgotPassword.Object);
            var result = await controller.Confirm(string.Empty);
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void NewAccount_WrongEmailAddress(){
            var mockedProtector = new Mock<IDataProtector>();
            mockedProtector.Setup(protector => protector.Unprotect(It.IsAny<byte[]>())).Returns(Encoding.UTF8.GetBytes("wrong@test.com"));
            _dataProvider.Setup(s => s.CreateProtector(It.IsAny<string>())).Returns(mockedProtector.Object);
            _repository.Setup(repo => repo.GetById<UserModel, string>(HelperConstants.TestEmailAddress, null)).Returns(HelperObjects.GetUser(UserStatesEnum.Active));

            var controller = new EmailController(_emailService.Object, _message.Object, _dataProvider.Object, _config.Object, 
                                _emailAddress.Object, _repository.Object, _emailTemplate.Object, _confirmationEmail.Object, _registrationEmail.Object, _forgotPassword.Object);
            var result = controller.NewAccount(string.Empty) as StatusCodeResult;
            Assert.Equal(404, (int)result.StatusCode);
        } 

        [Fact]
        public void NewAccount_UserIsActive(){
            _repository.Setup(repo => repo.GetById<UserModel, string>(HelperConstants.TestEmailAddress, null)).Returns(HelperObjects.GetUser(UserStatesEnum.Active));

            var controller = new EmailController(_emailService.Object, _message.Object, _dataProvider.Object, _config.Object, 
                                 _emailAddress.Object, _repository.Object, _emailTemplate.Object, _confirmationEmail.Object, _registrationEmail.Object, _forgotPassword.Object);
            var result = controller.NewAccount(string.Empty) as StatusCodeResult;
            Assert.Equal(404, (int)result.StatusCode);
        } 
        
        [Fact]
        public void NewAccount_Success(){

            _repository.Setup(repo => repo.GetById<UserModel, string>(HelperConstants.TestEmailAddress, null)).Returns(HelperObjects.GetUser(UserStatesEnum.NotConfirmed));
            _emailService.Setup(email => email.Send(_message.Object));
            _message.SetupProperty(m => m.ToAddresses);
            _message.Object.ToAddresses =new List<EmailAddress>();
            var controller = new EmailController(_emailService.Object, _message.Object, _dataProvider.Object, _config.Object,
                                 _emailAddress.Object, _repository.Object, _emailTemplate.Object, _confirmationEmail.Object, _registrationEmail.Object, _forgotPassword.Object);
            var result = controller.NewAccount(string.Empty);
            var redirectToAction = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Confirm", redirectToAction.ActionName);
            Assert.Null(redirectToAction.ControllerName);
        }

        [Fact]
        public async Task ForgetPassword_WrongEmailAddress(){
            var mockedProtector = new Mock<IDataProtector>();
            mockedProtector.Setup(protector => protector.Unprotect(It.IsAny<byte[]>())).Returns(Encoding.UTF8.GetBytes("wrong@test.com"));
            _dataProvider.Setup(s => s.CreateProtector(It.IsAny<string>())).Returns(mockedProtector.Object);
            _repository.Setup(repo => repo.GetById<UserModel, string>(HelperConstants.TestEmailAddress, null)).Returns(HelperObjects.GetUser(UserStatesEnum.NotConfirmed));

            var controller = new EmailController(_emailService.Object, _message.Object, _dataProvider.Object, _config.Object, 
                                _emailAddress.Object, _repository.Object, _emailTemplate.Object, _confirmationEmail.Object, _registrationEmail.Object, _forgotPassword.Object);
            var result = await controller.ForgetPassword(string.Empty) as StatusCodeResult;
            Assert.Equal(404, (int)result.StatusCode);
        } 

        [Fact]
        public async Task ForgetPassword_UserIsActive(){
            _repository.Setup(repo => repo.GetById<UserModel, string>(HelperConstants.TestEmailAddress, null)).Returns(HelperObjects.GetUser(UserStatesEnum.NotConfirmed));

            var controller = new EmailController(_emailService.Object, _message.Object, _dataProvider.Object, _config.Object, 
                                 _emailAddress.Object, _repository.Object, _emailTemplate.Object, _confirmationEmail.Object, _registrationEmail.Object, _forgotPassword.Object);
            var result = await controller.ForgetPassword(HelperConstants.TestEmailAddress) as StatusCodeResult;
            Assert.Equal(404, (int)result.StatusCode);
        } 

        [Fact]
        public async Task ForgetPassword_Success(){

            _repository.Setup(repo => repo.GetById<UserModel, string>(HelperConstants.TestEmailAddress, null)).Returns(HelperObjects.GetUser(UserStatesEnum.Active));
            _emailService.Setup(email => email.Send(_message.Object));
            _message.SetupProperty(m => m.ToAddresses);
            _message.Object.ToAddresses =new List<EmailAddress>();
            var controller = new EmailController(_emailService.Object, _message.Object, _dataProvider.Object, _config.Object,
                                 _emailAddress.Object, _repository.Object, _emailTemplate.Object, _confirmationEmail.Object, _registrationEmail.Object, _forgotPassword.Object);
            var result = await controller.ForgetPassword(HelperConstants.TestEmailAddress);
            Assert.IsType<ViewResult>(result);
        }
   }
}