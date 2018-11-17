using Xunit;
using Moq;
using BuildingAdmin.Mvc.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace BuildingAdmin.Tests.Controllers
{
    public class HomeControllerTests : IDisposable
    {

        private Mock<IStringLocalizer<HomeController>> mockLocalizer;
        private Mock<ILogger<HomeController>> mockLogger;

        public HomeControllerTests(){
            mockLocalizer = new Mock<IStringLocalizer<HomeController>>();   
            mockLogger = new Mock<ILogger<HomeController>>();
        }

        [Fact]
        public void Index_ReturnsAView(){
            var controller = new HomeController(mockLocalizer.Object, mockLogger.Object);
            var result = controller.Index();
            Assert.IsType<ViewResult>(result);            
        }

        [Fact]
        public void About_ReturnsAView(){
            var controller = new HomeController(mockLocalizer.Object, mockLogger.Object);
            var result = controller.About();
            Assert.IsType<ViewResult>(result);            
        }

       

        [Fact]
        public void Forbbiden_ReturnsAView(){
            var controller = new HomeController(mockLocalizer.Object, mockLogger.Object);
            var result = controller.Forbbiden();
            Assert.IsType<ViewResult>(result);            
        }

        [Fact]
        public void Pricing_ReturnsAView(){
            var controller = new HomeController(mockLocalizer.Object, mockLogger.Object);
            var result = controller.Pricing();
            Assert.IsType<ViewResult>(result);            
        }

/*
        [Fact]
        public void Error_ReturnsAView_404(){
            var controller = new HomeController(mockLocalizer.Object, mockLogger.Object);
            var result = controller.Error(404) as ViewResult;
            Assert.IsType<ViewResult>(result);
            Assert.Equal("404", result.ViewName);            
        }

        [Fact]
        public void Error_ReturnsAView_404_ForNull(){
            var controller = new HomeController(mockLocalizer.Object, mockLogger.Object);
            var result = controller.Error(null) as ViewResult;
            Assert.IsType<ViewResult>(result);
            Assert.Equal("404", result.ViewName);
        }

        [Fact]
        public void Error_ReturnsAView_500(){
            var controller = new HomeController(mockLocalizer.Object, mockLogger.Object);
            var result = controller.Error(500) as ViewResult;
            Assert.IsType<ViewResult>(result);
            Assert.Equal("500", result.ViewName);
        }

        [Theory]
        [InlineData(302)]
        [InlineData(502)]
        [InlineData(400)]
        [InlineData(202)]
        public void Error_ReturnsAView_500_ForEverythingElseThenNullAnd404(int value){
            var controller = new HomeController(mockLocalizer.Object, mockLogger.Object);
            var result = controller.Error(value) as ViewResult;
            Assert.IsType<ViewResult>(result);
            Assert.Equal("500", result.ViewName);
        }*/
        public void Dispose(){
            mockLocalizer = null;
            mockLogger = null;
        }
    }
}