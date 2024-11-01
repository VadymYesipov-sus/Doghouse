using Doghouse.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Doghouse.Tests.HelperTests
{
    public class RateLimiterTests
    {
        private readonly RateLimiter _rateLimiter;
        private readonly Mock<RequestDelegate> _next;

        public RateLimiterTests()
        {
            _next = new Mock<RequestDelegate>();
            _next.Setup(n => n(It.IsAny<HttpContext>())).Returns(Task.CompletedTask);

            var inMemorySettings = new Dictionary<string, string>
            {
                { "RateLimiting:RequestsPerSecond", "10" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _rateLimiter = new RateLimiter(_next.Object, configuration);
        }


        [Fact]
        public async Task InvokeAsync_ExceedsLimit_Returns429()
        {
            // Arrange
            var context = new DefaultHttpContext
            {
                Connection = { RemoteIpAddress = IPAddress.Parse("192.168.1.1") }
            };

            using var responseStream = new MemoryStream();
            context.Response.Body = responseStream; 

            for (int i = 0; i < 10; i++) 
            {
                await _rateLimiter.InvokeAsync(context);
            }

            // Act 
            await _rateLimiter.InvokeAsync(context);

            responseStream.Position = 0;

            using var reader = new StreamReader(responseStream);
            var responseBody = await reader.ReadToEndAsync();

            // Assert
            Assert.Equal(StatusCodes.Status429TooManyRequests, context.Response.StatusCode);
            Assert.Equal("Too many requests. Please try again later.", responseBody);
            _next.Verify(n => n(context), Times.Exactly(10));
        }
    }
}
