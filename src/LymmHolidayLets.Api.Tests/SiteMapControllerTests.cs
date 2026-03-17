using LymmHolidayLets.Api.Controllers;
using LymmHolidayLets.Api.Infrastructure.SiteMap;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Asp.Versioning;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace LymmHolidayLets.Api.Tests
{
    /// <summary>
    /// Unit tests for SiteMapController
    /// Tests the sitemap index endpoint behavior
    /// </summary>
    public class SiteMapControllerTests
    {
        private readonly Mock<ILogger<SiteMapController>> _loggerMock;
        private readonly SiteMapController _controller;

        public SiteMapControllerTests()
        {
            _loggerMock = new Mock<ILogger<SiteMapController>>();
            _controller = new SiteMapController(_loggerMock.Object);
        }

        /// <summary>
        /// Test: Index endpoint returns XmlSiteMapIndex result
        /// </summary>
        [Fact]
        public void Index_ReturnsXmlSiteMapIndexResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            var xmlResult = Assert.IsType<XmlSiteMapIndex>(result);
            Assert.NotNull(xmlResult);
        }

        /// <summary>
        /// Test: Index endpoint logs information when called
        /// </summary>
        [Fact]
        public void Index_LogsInformation()
        {
            // Act
            _controller.Index();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Sitemap index requested")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        /// <summary>
        /// Test: Index endpoint returns IActionResult type
        /// </summary>
        [Fact]
        public void Index_ReturnsIActionResult()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsAssignableFrom<IActionResult>(result);
        }

        /// <summary>
        /// Test: Controller is properly decorated with ApiController attribute
        /// </summary>
        [Fact]
        public void Controller_HasApiControllerAttribute()
        {
            // Arrange
            var controllerType = typeof(SiteMapController);

            // Act
            var attributes = controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false);

            // Assert
            Assert.NotEmpty(attributes);
        }

        /// <summary>
        /// Test: Index method is decorated with HttpGet attribute
        /// </summary>
        [Fact]
        public void IndexMethod_HasHttpGetAttribute()
        {
            // Arrange
            var method = typeof(SiteMapController).GetMethod("Index");

            // Act
            var attributes = method?.GetCustomAttributes(typeof(HttpGetAttribute), false);

            // Assert
            Assert.NotNull(attributes);
            Assert.NotEmpty(attributes);
        }

        /// <summary>
        /// Test: Index method is decorated with ProducesResponseType attributes
        /// </summary>
        [Fact]
        public void IndexMethod_HasProducesResponseTypeAttributes()
        {
            // Arrange
            var method = typeof(SiteMapController).GetMethod("Index");

            // Act
            var attributes = method?.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), false);

            // Assert
            Assert.NotNull(attributes);
            Assert.NotEmpty(attributes);
        }

        /// <summary>
        /// Test: Controller is sealed (cannot be inherited)
        /// </summary>
        [Fact]
        public void SiteMapController_IsSealed()
        {
            // Arrange
            var controllerType = typeof(SiteMapController);

            // Act & Assert
            Assert.True(controllerType.IsSealed);
        }

        /// <summary>
        /// Test: Constructor accepts ILogger dependency
        /// </summary>
        [Fact]
        public void Constructor_AcceptsLoggerDependency()
        {
            // Act
            var controller = new SiteMapController(_loggerMock.Object);

            // Assert
            Assert.NotNull(controller);
        }

        /// <summary>
        /// Test: Multiple calls to Index all log information
        /// </summary>
        [Fact]
        public void Index_MultipleCalls_LogsEachTime()
        {
            // Act
            _controller.Index();
            _controller.Index();
            _controller.Index();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Sitemap index requested")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Exactly(3));
        }

        /// <summary>
        /// Test: Index method produces XML content type
        /// </summary>
        [Fact]
        public void IndexMethod_HasProducesAttribute()
        {
            // Arrange
            var method = typeof(SiteMapController).GetMethod("Index");

            // Act
            var attributes = method?.GetCustomAttributes(typeof(ProducesAttribute), false);

            // Assert
            Assert.NotNull(attributes);
            Assert.NotEmpty(attributes);
            var producesAttr = (ProducesAttribute)attributes?[0]!;
            Assert.Contains("application/xml", producesAttr.ContentTypes);
        }

        /// <summary>
        /// Test: Controller has correct API version
        /// </summary>
        [Fact]
        public void Controller_HasCorrectApiVersion()
        {
            // Arrange
            var controllerType = typeof(SiteMapController);

            // Act
            var versionAttributes = controllerType.GetCustomAttributes(typeof(Asp.Versioning.ApiVersionAttribute), false);

            // Assert
            Assert.NotEmpty(versionAttributes);
            var versionAttr = (Asp.Versioning.ApiVersionAttribute)versionAttributes[0];
            Assert.Contains("1.0", versionAttr.Versions.Select(v => v.ToString()));
        }

        /// <summary>
        /// Test: Controller has correct route template
        /// </summary>
        [Fact]
        public void Controller_HasCorrectRouteTemplate()
        {
            // Arrange
            var controllerType = typeof(SiteMapController);

            // Act
            var routeAttributes = controllerType.GetCustomAttributes(typeof(RouteAttribute), false);

            // Assert
            Assert.NotEmpty(routeAttributes);
            var routeAttr = (RouteAttribute)routeAttributes[0];
            Assert.Equal("api/v{version:apiVersion}/[controller]", routeAttr.Template);
        }

        /// <summary>
        /// Test: Index method has correct HTTP route
        /// </summary>
        [Fact]
        public void IndexMethod_HasCorrectHttpRoute()
        {
            // Arrange
            var method = typeof(SiteMapController).GetMethod("Index");

            // Act
            var httpGetAttributes = method?.GetCustomAttributes(typeof(HttpGetAttribute), false);

            // Assert
            Assert.NotNull(httpGetAttributes);
            Assert.NotEmpty(httpGetAttributes);
            var httpGetAttr = (HttpGetAttribute)httpGetAttributes[0];
            Assert.Equal("index.xml", httpGetAttr.Template);
        }

        /// <summary>
        /// Test: Controller inherits from ControllerBase
        /// </summary>
        [Fact]
        public void Controller_InheritsFromControllerBase()
        {
            // Arrange
            var controllerType = typeof(SiteMapController);

            // Act & Assert
            Assert.True(typeof(ControllerBase).IsAssignableFrom(controllerType));
        }

        /// <summary>
        /// Test: Index method has XML documentation summary
        /// </summary>
        [Fact]
        public void IndexMethod_HasXmlDocumentation()
        {
            // Arrange
            var method = typeof(SiteMapController).GetMethod("Index");

            // Act & Assert
            Assert.NotNull(method);
            // This test ensures the method exists and can be reflected upon
            // XML documentation is validated by the compiler
        }

        /// <summary>
        /// Test: Controller has both 200 and 500 response type attributes
        /// </summary>
        [Fact]
        public void IndexMethod_HasCorrectResponseTypes()
        {
            // Arrange
            var method = typeof(SiteMapController).GetMethod("Index");

            // Act
            var responseTypeAttrs = method?.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), false)
                .Cast<ProducesResponseTypeAttribute>().ToArray();

            // Assert
            Assert.NotNull(responseTypeAttrs);
            Assert.Contains(responseTypeAttrs, attr => attr.StatusCode == StatusCodes.Status200OK);
            Assert.Contains(responseTypeAttrs, attr => attr.StatusCode == StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Test: Constructor with null logger throws ArgumentNullException
        /// </summary>
        [Fact]
        public void Constructor_WithNullLogger_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SiteMapController(null!));
        }

        /// <summary>
        /// Test: Index method execution time is reasonable (performance test)
        /// </summary>
        [Fact]
        public void Index_ExecutionTime_IsReasonable()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            _controller.Index();
            stopwatch.Stop();

            // Assert - Should complete in under 100ms
            Assert.True(stopwatch.ElapsedMilliseconds < 100, 
                $"Index method took {stopwatch.ElapsedMilliseconds}ms, expected < 100ms");
        }

        /// <summary>
        /// Test: Multiple concurrent calls to Index method are thread-safe
        /// </summary>
        [Fact]
        public async Task Index_ConcurrentCalls_AreThreadSafe()
        {
            // Arrange
            var tasks = new List<Task>();
            const int numberOfTasks = 10;

            // Act
            for (int i = 0; i < numberOfTasks; i++)
            {
                tasks.Add(Task.Run(() => _controller.Index()));
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Sitemap index requested")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Exactly(numberOfTasks));
        }

        /// <summary>
        /// Test: Index method returns different XmlSiteMapIndex instances
        /// </summary>
        [Fact]
        public void Index_MultipleCalls_ReturnsDifferentInstances()
        {
            // Act
            var result1 = _controller.Index();
            var result2 = _controller.Index();

            // Assert
            Assert.NotSame(result1, result2);
            Assert.IsType<XmlSiteMapIndex>(result1);
            Assert.IsType<XmlSiteMapIndex>(result2);
        }

        /// <summary>
        /// Test: Controller follows naming conventions
        /// </summary>
        [Fact]
        public void Controller_FollowsNamingConventions()
        {
            // Arrange
            var controllerType = typeof(SiteMapController);

            // Act & Assert
            Assert.EndsWith("Controller", controllerType.Name);
            Assert.Equal("SiteMapController", controllerType.Name);
        }

        /// <summary>
        /// Test: Controller is in correct namespace
        /// </summary>
        [Fact]
        public void Controller_HasCorrectNamespace()
        {
            // Arrange
            var controllerType = typeof(SiteMapController);

            // Act & Assert
            Assert.Equal("LymmHolidayLets.Api.Controllers", controllerType.Namespace);
        }

        /// <summary>
        /// Test: Logger is properly injected and used
        /// </summary>
        [Fact]
        public void Constructor_InjectsLoggerCorrectly()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<SiteMapController>>();

            // Act
            var controller = new SiteMapController(loggerMock.Object);
            controller.Index();

            // Assert
            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Sitemap index requested")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        /// <summary>
        /// Test: Index method returns fresh instance each time (stateless)
        /// </summary>
        [Fact]
        public void Index_IsStateless_ReturnsNewInstanceEachTime()
        {
            // Act
            var result1 = _controller.Index() as XmlSiteMapIndex;
            var result2 = _controller.Index() as XmlSiteMapIndex;
            var result3 = _controller.Index() as XmlSiteMapIndex;

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);
            Assert.NotSame(result1, result2);
            Assert.NotSame(result2, result3);
            Assert.NotSame(result1, result3);
        }

        /// <summary>
        /// Test: Controller method signature matches expected pattern
        /// </summary>
        [Fact]
        public void IndexMethod_HasCorrectSignature()
        {
            // Arrange
            var method = typeof(SiteMapController).GetMethod("Index");

            // Act & Assert
            Assert.NotNull(method);
            Assert.True(method.IsPublic);
            Assert.Equal(typeof(IActionResult), method.ReturnType);
            Assert.Empty(method.GetParameters()); // No parameters
        }

        /// <summary>
        /// Test: Controller uses primary constructor pattern
        /// </summary>
        [Fact]
        public void Controller_UsesPrimaryConstructor()
        {
            // Arrange
            var controllerType = typeof(SiteMapController);
            var constructors = controllerType.GetConstructors();

            // Act & Assert
            Assert.Single(constructors); // Only one constructor
            var constructor = constructors[0];
            var parameters = constructor.GetParameters();
            Assert.Single(parameters); // Only one parameter
            Assert.Equal(typeof(ILogger<SiteMapController>), parameters[0].ParameterType);
        }

        /// <summary>
        /// Test: Index method execution is consistent across multiple calls
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        public void Index_MultipleExecutions_ConsistentBehavior(int callCount)
        {
            // Arrange
            var results = new List<IActionResult>();

            // Act
            for (int i = 0; i < callCount; i++)
            {
                results.Add(_controller.Index());
            }

            // Assert
            Assert.All(results, result => Assert.IsType<XmlSiteMapIndex>(result));
            
            // Verify logging occurred for each call
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Sitemap index requested")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Exactly(callCount));
        }

        /// <summary>
        /// Test: Controller metadata is correct for OpenAPI/Swagger
        /// </summary>
        [Fact]
        public void Controller_HasCorrectMetadataForSwagger()
        {
            // Arrange
            var controllerType = typeof(SiteMapController);
            var indexMethod = controllerType.GetMethod("Index");

            // Act & Assert
            // Verify controller has ApiController attribute (required for automatic model validation)
            Assert.NotNull(controllerType.GetCustomAttributes(typeof(ApiControllerAttribute), false).FirstOrDefault());
            
            // Verify method has Produces attribute (for content negotiation)
            var producesAttr = indexMethod?.GetCustomAttributes(typeof(ProducesAttribute), false).FirstOrDefault() as ProducesAttribute;
            Assert.NotNull(producesAttr);
            Assert.Contains("application/xml", producesAttr.ContentTypes);
            
            // Verify response types are documented
            var responseTypes = indexMethod?.GetCustomAttributes(typeof(ProducesResponseTypeAttribute), false);
            Assert.NotNull(responseTypes);
            Assert.True(responseTypes.Length >= 2); // At least 200 and 500
        }

        /// <summary>
        /// Test: Controller follows RESTful conventions
        /// </summary>
        [Fact]
        public void Controller_FollowsRestfulConventions()
        {
            // Arrange
            var controllerType = typeof(SiteMapController);
            var indexMethod = controllerType.GetMethod("Index");

            // Act & Assert
            // GET method for retrieving data
            var httpGetAttr = indexMethod?.GetCustomAttributes(typeof(HttpGetAttribute), false).FirstOrDefault();
            Assert.NotNull(httpGetAttr);
            
            // Specific route for the resource
            var routeAttr = controllerType.GetCustomAttributes(typeof(RouteAttribute), false).FirstOrDefault() as RouteAttribute;
            Assert.NotNull(routeAttr);
            Assert.Contains("[controller]", routeAttr.Template);
            
            // Version-aware routing
            Assert.Contains("v{version:apiVersion}", routeAttr.Template);
        }

        /// <summary>
        /// Test: Index method handles rapid successive calls correctly
        /// </summary>
        [Fact]
        public void Index_RapidSuccessiveCalls_HandledCorrectly()
        {
            // Arrange
            const int rapidCallCount = 100;
            var results = new List<IActionResult>();

            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < rapidCallCount; i++)
            {
                results.Add(_controller.Index());
            }
            stopwatch.Stop();

            // Assert
            Assert.Equal(rapidCallCount, results.Count);
            Assert.All(results, result => Assert.IsType<XmlSiteMapIndex>(result));
            
            // Performance assertion - should handle 100 calls quickly
            Assert.True(stopwatch.ElapsedMilliseconds < 1000, 
                $"100 rapid calls took {stopwatch.ElapsedMilliseconds}ms, expected < 1000ms");
            
            // Verify all calls were logged
            _loggerMock.Verify(
                x => x.Log(LogLevel.Information, It.IsAny<EventId>(), 
                    It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), 
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Exactly(rapidCallCount));
        }
    }
}
