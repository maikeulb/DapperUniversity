using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperUniversity.Data;
using DapperUniversity.Models;
using DapperUniversity.ViewModels;
using DapperUniversity.Controllers;
using DapperUniversity.Tests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace DapperUniversity.Tests.Controllers
{
    public class HomeControllerTests
    {
        HomeController _sut;
        
        public HomeControllerTests()
        {
            var mockILogger = new Mock<ILogger<HomeController>>();
            var mockConnectionString = new Mock<IConfiguration>();
            _sut = new HomeController(mockILogger.Object, mockConnectionString.Object);
        }

        [Fact]
        public void Index_ReturnsAViewResult()
        {
            var result = _sut.Index();

            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public void Contact_ReturnsAViewResult()
        {
            var result = _sut.Contact();

            Assert.IsType<ViewResult>(result);
        }

    }
}
