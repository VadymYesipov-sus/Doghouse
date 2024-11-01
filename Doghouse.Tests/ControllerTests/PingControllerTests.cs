using Doghouse.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doghouse.Tests.ControllerTests
{
    public class PingControllerTests
    {
        private readonly PingController _pingController;
        public PingControllerTests()
        {
            _pingController = new PingController();
        }

        [Fact]
        public void Ping_ReturnsOkResult_WithVersionString()
        {
            //Arrange 
            //we have nothing to arrange since Ping() takes 0 parameters

            // Act
            var result = _pingController.Ping();

            // Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);
            Assert.Equal("Dogshouseservice.Version1.0.1", returnValue);
        }

    }
}
