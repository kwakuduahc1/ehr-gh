using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using ShimsServer.Controllers;
using ShimsServer.Data.Repositories;
using ShimsServer.Models.Schemes;
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
            var mockRepository = new Mock<ISchemeServiceRepository>();
            var mockLogger = new Mock<ILogger<SchemeServicesController>>();
            var cancellationToken = new CancellationToken();

            mockRepository
                .Setup(r => r.GetConnectionAsync(cancellationToken))
                .ThrowsAsync(new Exception("Stop execution"));

            var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            try
            {
                await controller.GetServicesByScheme(schemeId);
            }
            catch
            {
                // Expected - cannot fully execute due to Dapper mocking limitations
            }

            // Assert
            mockRepository.Verify(r => r.GetConnectionAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetservicesByScheme_ConnectionThrowsNpgsqlException_PropagatesException()
        {
            // Arrange
            var schemeId = Guid.NewGuid();
            var mockRepository = new Mock<ISchemeServiceRepository>();
            var mockLogger = new Mock<ILogger<SchemeServicesController>>();
            var cancellationToken = new CancellationToken();
            var npgsqlException = new NpgsqlException("Connection failed");

            mockRepository
                .Setup(r => r.GetConnectionAsync(cancellationToken))
                .ThrowsAsync(npgsqlException);

            var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object, cancellationToken);

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
            var mockRepository = new Mock<ISchemeServiceRepository>();
            var mockLogger = new Mock<ILogger<SchemeServicesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.GetConnectionAsync(cancellationToken))
                .ThrowsAsync(new Exception("Stop"));

            var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            try
            {
                await controller.GetServicesByScheme(schemeId);
            }
            catch { }

            // Assert
            mockRepository.Verify(r => r.GetConnectionAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task AddSchemeService_RepositoryThrowsNpgsqlException_ReturnsBadRequestAndLogsError()
        {
            // Arrange
            var mockRepository = new Mock<ISchemeServiceRepository>();
            var mockLogger = new Mock<ILogger<SchemeServicesController>>();
            var cancellationToken = CancellationToken.None;
            var npgsqlException = new NpgsqlException("Database error");

            mockRepository
                .Setup(r => r.GetConnectionAsync(cancellationToken))
                .ThrowsAsync(npgsqlException);

            var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object, cancellationToken);
            var dto = new AddSchemeServiceDto(
                SchemesID: Guid.NewGuid(),
                ServicesID: Guid.NewGuid(),
                GDRG: "GDRG001",
                Narration: "Test",
                AllowedTiers: new[] { "Tier1" },
                Price: 100.00m);

            // Act
            var result = await controller.AddSchemeService(dto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Guid>>(result);
            var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.NotNull(badRequest.Value);

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Database error occurred")),
                    npgsqlException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task AddSchemeService_RepositoryThrowsGenericException_ReturnsBadRequestAndLogsError()
        {
            // Arrange
            var mockRepository = new Mock<ISchemeServiceRepository>();
            var mockLogger = new Mock<ILogger<SchemeServicesController>>();
            var cancellationToken = CancellationToken.None;
            var exception = new Exception("Unexpected error");

            mockRepository
                .Setup(r => r.GetConnectionAsync(cancellationToken))
                .ThrowsAsync(exception);

            var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object, cancellationToken);
            var dto = new AddSchemeServiceDto(
                SchemesID: Guid.NewGuid(),
                ServicesID: Guid.NewGuid(),
                GDRG: "GDRG001",
                Narration: "Test",
                AllowedTiers: new[] { "Tier1" },
                Price: 100.00m);

            // Act
            var result = await controller.AddSchemeService(dto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Guid>>(result);
            var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.NotNull(badRequest.Value);

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Unexpected error occurred")),
                    exception,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task AddSchemeService_CallsRepositoryWithCorrectToken()
        {
            // Arrange
            var mockRepository = new Mock<ISchemeServiceRepository>();
            var mockLogger = new Mock<ILogger<SchemeServicesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.GetConnectionAsync(cancellationToken))
                .ThrowsAsync(new Exception("Stop"));

            var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object, cancellationToken);
            var dto = new AddSchemeServiceDto(
                SchemesID: Guid.NewGuid(),
                ServicesID: Guid.NewGuid(),
                GDRG: "GDRG001",
                Narration: "Test",
                AllowedTiers: new[] { "Tier1" },
                Price: 100.00m);

            // Act
            try
            {
                await controller.AddSchemeService(dto);
            }
            catch { }

            // Assert
            mockRepository.Verify(r => r.GetConnectionAsync(cancellationToken), Times.Once);
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(100.00)]
        [InlineData(99999.99)]
        public async Task AddSchemeService_VariousPrices_CallsRepository(decimal price)
        {
            // Arrange
            var mockRepository = new Mock<ISchemeServiceRepository>();
            var mockLogger = new Mock<ILogger<SchemeServicesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.GetConnectionAsync(cancellationToken))
                .ThrowsAsync(new Exception("Stop"));

            var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object, cancellationToken);
            var dto = new AddSchemeServiceDto(
                SchemesID: Guid.NewGuid(),
                ServicesID: Guid.NewGuid(),
                GDRG: "GDRG001",
                Narration: "Test",
                AllowedTiers: new[] { "Tier1" },
                Price: price);

            // Act
            try
            {
                await controller.AddSchemeService(dto);
            }
            catch { }

            // Assert
            mockRepository.Verify(r => r.GetConnectionAsync(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task AddSchemeService_EmptyAllowedTiers_CallsRepository()
        {
            // Arrange
            var mockRepository = new Mock<ISchemeServiceRepository>();
            var mockLogger = new Mock<ILogger<SchemeServicesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.GetConnectionAsync(cancellationToken))
                .ThrowsAsync(new Exception("Stop"));

            var controller = new SchemeServicesController(mockRepository.Object, mockLogger.Object, cancellationToken);
            var dto = new AddSchemeServiceDto(
                SchemesID: Guid.NewGuid(),
                ServicesID: Guid.NewGuid(),
                GDRG: "GDRG001",
                Narration: "Test",
                AllowedTiers: [],
                Price: 100.00m);

            // Act
            try
            {
                await controller.AddSchemeService(dto);
            }
            catch { }

            // Assert
            mockRepository.Verify(r => r.GetConnectionAsync(cancellationToken), Times.Once);
        }
    }
}
