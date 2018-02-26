using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Xunit.Abstractions;
using Dapper;
using DapperUniversity.Data;
using DapperUniversity.Controllers;
using DapperUniversity.Models;
using DapperUniversity.Tests;

namespace ContosoUniversity.Web.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly ITestOutputHelper _output;
        private HomeController _sut;
        
        public HomeControllerTests(ITestOutputHelper output)
        {
            _output = output;
            _sut = new HomeController(mockUnitOfWork.Object);
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

        [Fact]
        public void Error_ReturnsAViewResult()
        {
            var result = _sut.Error();

            Assert.IsType<ViewResult>(result);
        }

    }
}
