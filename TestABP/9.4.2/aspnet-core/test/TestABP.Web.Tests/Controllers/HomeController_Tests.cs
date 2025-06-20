﻿using System.Threading.Tasks;
using TestABP.Models.TokenAuth;
using TestABP.Web.Controllers;
using Shouldly;
using Xunit;

namespace TestABP.Web.Tests.Controllers
{
    public class HomeController_Tests: TestABPWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}