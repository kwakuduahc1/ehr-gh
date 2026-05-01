using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using ShimsServer.Controllers;
using ShimsServer.Models.Schemes;
using ShimsServer.Repositories;
using Xunit;

namespace ShimsServer.Tests.Controllers
{
    public class SchemeServicesControllerTests
    {
        [Fact]
        public async Task GetservicesByScheme_CallsRepositoryWithCorrectToken()
        {
            // Arrange
            var schemeId = Guid.NewGuid();
            var mockRepository = new Mock<ISchemeServicePricingRepository>();
            var mockLogger = new Mock<ILogger<SchemeServicesController>>();
            var cancellationToken = new CancellationToken();
            mockRepository.Setup(r => r.GetServicesBySchemeAsync(schemeId, cancellationToken)).ReturnsAsync(Array.Empty<SchemeServiceDTO>());
            var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.RequestAborted = cancellationToken;
            // Act
            await controller.GetServicesByScheme(schemeId);
            // Assert
            mockRepository.Verify(r => r.GetServicesBySchemeAsync(schemeId, cancellationToken), Times.Once);
        }

[Fact]
public async Task GetservicesByScheme_ConnectionThrowsNpgsqlException_PropagatesException()
{
    // Arrange
    var schemeId = Guid.NewGuid();
    var mockRepository = new Mock<ISchemeServicePricingRepository>();
    var mockLogger = new Mock<ILogger<SchemeServicesController>>();
    var cancellationToken = CancellationToken.None;
    var npgsqlException = new NpgsqlException("Connection failed");
    mockRepository.Setup(r => r.GetServicesBySchemeAsync(schemeId, cancellationToken)).ThrowsAsync(npgsqlException);
    var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object);
    controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext() };
    // Act & Assert
    var exception = await Assert.ThrowsAsync<NpgsqlException>(() => controller.GetServicesByScheme(schemeId));
    Assert.Equal("Connection failed", exception.Message);
}

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000001")]
        [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
        public async Task GetservicesByScheme_VariousGuids_CallsRepository(string guidString)
        {
            // Arrange
            var schemeId = Guid.Parse(guidString);
            var mockRepository = new Mock<ISchemeServicePricingRepository>();
            var mockLogger = new Mock<ILogger<SchemeServicesController>>();
            var cancellationToken = CancellationToken.None;
            mockRepository.Setup(r => r.GetServicesBySchemeAsync(schemeId, cancellationToken)).ReturnsAsync([]);
            var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            controller.HttpContext.RequestAborted = cancellationToken;
            // Act
            await controller.GetServicesByScheme(schemeId);
            // Assert
            mockRepository.Verify(r => r.GetServicesBySchemeAsync(schemeId, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task AddSchemeService_RepositoryThrowsGenericException_ReturnsBadRequestAndLogsError()
        {
            // Arrange
            var mockRepository = new Mock<ISchemeServicePricingRepository>();
            var mockLogger = new Mock<ILogger<SchemeServicesController>>();
            var cancellationToken = CancellationToken.None;
            var exception = new Exception("Unexpected error");
            mockRepository.Setup(r => r.AddSchemeServiceAsync(It.IsAny<AddSchemeServiceDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(exception);
            var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
            {
                HttpContext = new Microsoft.AspNetCore.Http.DefaultHttpContext()
            };
            var dto = new AddSchemeServiceDto(SchemesID: Guid.NewGuid(), ServicesID: Guid.NewGuid(), GDRG: "GDRG001", Narration: "Test", AllowedTiers: ["Tier1"], Price: 100.00m);
            // Act
            var result = await controller.AddSchemeService(dto);
            // Assert
            var actionResult = Assert.IsType<ActionResult<Guid>>(result);
            var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.NotNull(badRequest.Value);
            mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unexpected error occurred")), exception, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task AddSchemeService_CallsRepositoryWithCorrectToken()
        {
            // Arrange
            var mockRepository = new Mock<ISchemeServicePricingRepository>();
            var mockLogger = new Mock<ILogger<SchemeServicesController>>();
            var cancellationToken = CancellationToken.None;
            mockRepository.Setup(r => r.AddSchemeServiceAsync(It.IsAny<AddSchemeServiceDto>(), It.IsAny<string>(), cancellationToken)).ReturnsAsync(Guid.NewGuid());
            var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            var dto = new AddSchemeServiceDto(SchemesID: Guid.NewGuid(), ServicesID: Guid.NewGuid(), GDRG: "GDRG001", Narration: "Test", AllowedTiers: ["Tier1"], Price: 100.00m);
            // Act
            await controller.AddSchemeService(dto);
            // Assert
            mockRepository.Verify(r => r.AddSchemeServiceAsync(It.IsAny<AddSchemeServiceDto>(), It.IsAny<string>(), cancellationToken), Times.Once);
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(100.00)]
        [InlineData(99999.99)]
        public async Task AddSchemeService_VariousPrices_CallsRepository(decimal price)
        {
            // Arrange
            var mockRepository = new Mock<ISchemeServicePricingRepository>();
            var mockLogger = new Mock<ILogger<SchemeServicesController>>();
            var cancellationToken = CancellationToken.None;
            mockRepository.Setup(r => r.AddSchemeServiceAsync(It.IsAny<AddSchemeServiceDto>(), It.IsAny<string>(), cancellationToken)).ReturnsAsync(Guid.NewGuid());
            var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            var dto = new AddSchemeServiceDto(SchemesID: Guid.NewGuid(), ServicesID: Guid.NewGuid(), GDRG: "GDRG001", Narration: "Test", AllowedTiers: ["Tier1"], Price: price);
            // Act
            await controller.AddSchemeService(dto);
            // Assert
            mockRepository.Verify(r => r.AddSchemeServiceAsync(It.IsAny<AddSchemeServiceDto>(), It.IsAny<string>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task AddSchemeService_EmptyAllowedTiers_CallsRepository()
        {
            // Arrange
            var mockRepository = new Mock<ISchemeServicePricingRepository>();
            var mockLogger = new Mock<ILogger<SchemeServicesController>>();
            var cancellationToken = CancellationToken.None;
            mockRepository.Setup(r => r.AddSchemeServiceAsync(It.IsAny<AddSchemeServiceDto>(), It.IsAny<string>(), cancellationToken)).ReturnsAsync(Guid.NewGuid());
            var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            var dto = new AddSchemeServiceDto(SchemesID: Guid.NewGuid(), ServicesID: Guid.NewGuid(), GDRG: "GDRG001", Narration: "Test", AllowedTiers: Array.Empty<string>(), Price: 100.00m);
            // Act
            await controller.AddSchemeService(dto);
            // Assert
            mockRepository.Verify(r => r.AddSchemeServiceAsync(It.IsAny<AddSchemeServiceDto>(), It.IsAny<string>(), cancellationToken), Times.Once);
        }
    }
}