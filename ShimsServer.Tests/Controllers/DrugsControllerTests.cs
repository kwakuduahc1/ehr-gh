using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using ShimsServer.Controllers;
using ShimsServer.Models.Drugs;
using ShimsServer.Repositories;
using Xunit;

namespace ShimsServer.Tests.Controllers
{
    public class DrugsControllerTests
    {
        [Fact]
        public async Task GetDrugs_CallsRepositoryWithCorrectToken()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = new CancellationToken();
            mockRepository.Setup(r => r.Drugs(cancellationToken)).ReturnsAsync(Array.Empty<DrugsDTO>());
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            await controller.GetDrugs();

            // Assert
            mockRepository.Verify(r => r.Drugs(cancellationToken), Times.Once);
        }

        [Fact]
        public async Task GetDrugs_ReturnsEnumerableOfDrugsDTO()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugs = new[]
            {
                new DrugsDTO(DrugsID: Guid.NewGuid(), Drug: "Aspirin", Tags: "pain", Description: "Pain relief", DateAdded: DateTime.Now),
                new DrugsDTO(DrugsID: Guid.NewGuid(), Drug: "Paracetamol", Tags: "fever", Description: "Fever relief", DateAdded: DateTime.Now)
            };
            mockRepository.Setup(r => r.Drugs(cancellationToken)).ReturnsAsync(drugs);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.GetDrugs();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Aspirin", result.First().Drug);
        }

        [Theory]
        [InlineData("Aspirin")]
        [InlineData("Paracetamol")]
        [InlineData("Ibuprofen")]
        public async Task DrugExists_DrugExistsInRepository_ReturnsOk(string drugName)
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            mockRepository.Setup(r => r.DrugExists(drugName, cancellationToken)).ReturnsAsync(true);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.DrugExists(drugName);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            mockRepository.Verify(r => r.DrugExists(drugName, cancellationToken), Times.Once);
        }

        [Theory]
        [InlineData("NonExistentDrug")]
        [InlineData("UnknownMedicine")]
        public async Task DrugExists_DrugNotInRepository_ReturnsNotFound(string drugName)
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            mockRepository.Setup(r => r.DrugExists(drugName, cancellationToken)).ReturnsAsync(false);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.DrugExists(drugName);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            mockRepository.Verify(r => r.DrugExists(drugName, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task AddDrug_RepositoryReturnsOne_ReturnsOkWithId()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugDto = new AddDrugDto(Drug: "Aspirin", Description: "Pain relief", Tags: "pain");
            mockRepository.Setup(r => r.AddDrug(It.IsAny<Guid>(), drugDto, cancellationToken)).ReturnsAsync(1);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.AddDrug(drugDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            Assert.IsType<Guid>(okResult.Value);
            mockRepository.Verify(r => r.AddDrug(It.IsAny<Guid>(), drugDto, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task AddDrug_RepositoryReturnsZero_ReturnsBadRequest()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugDto = new AddDrugDto(Drug: "Aspirin", Description: "Pain relief", Tags: "pain");
            mockRepository.Setup(r => r.AddDrug(It.IsAny<Guid>(), drugDto, cancellationToken)).ReturnsAsync(0);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.AddDrug(drugDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task AddDrug_RepositoryThrowsPostgresException_ReturnsBadRequest()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugDto = new AddDrugDto(Drug: "Aspirin", Description: "Pain relief", Tags: "pain");
            var postgresException = new PostgresException("Duplicate key", null, null, null);
            mockRepository.Setup(r => r.AddDrug(It.IsAny<Guid>(), drugDto, cancellationToken)).ThrowsAsync(postgresException);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.AddDrug(drugDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            mockLogger.Verify(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task AddDrug_RepositoryThrowsGenericException_ReturnsInternalServerError()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugDto = new AddDrugDto(Drug: "Aspirin", Description: "Pain relief", Tags: "pain");
            var exception = new Exception("Unexpected error");
            mockRepository.Setup(r => r.AddDrug(It.IsAny<Guid>(), drugDto, cancellationToken)).ThrowsAsync(exception);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.AddDrug(drugDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
            mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), exception, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(100.00)]
        [InlineData(99999.99)]
        public async Task AddDrug_VariousPrices_CallsRepository(decimal price)
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugDto = new AddDrugDto(Drug: "Aspirin", Description: "Pain relief", Tags: "pain");
            mockRepository.Setup(r => r.AddDrug(It.IsAny<Guid>(), drugDto, cancellationToken)).ReturnsAsync(1);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.AddDrug(drugDto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            mockRepository.Verify(r => r.AddDrug(It.IsAny<Guid>(), drugDto, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task UpdateDrug_DrugExists_ReturnsOk()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugId = Guid.NewGuid();
            var drugDto = new UpdateDrugDto(DrugsID: drugId, Drug: "Aspirin Updated", Description: "Pain relief", Tags: "pain");
            mockRepository.Setup(r => r.DrugExists(drugId, cancellationToken)).ReturnsAsync(true);
            mockRepository.Setup(r => r.EditDrug(drugDto, cancellationToken)).ReturnsAsync(1);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.UpdateDrug(drugDto);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            mockRepository.Verify(r => r.DrugExists(drugId, cancellationToken), Times.Once);
            mockRepository.Verify(r => r.EditDrug(drugDto, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task UpdateDrug_DrugNotFound_ReturnsBadRequest()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugId = Guid.NewGuid();
            var drugDto = new UpdateDrugDto(DrugsID: drugId, Drug: "Aspirin", Description: "Pain relief", Tags: "pain");
            mockRepository.Setup(r => r.DrugExists(drugId, cancellationToken)).ReturnsAsync(false);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.UpdateDrug(drugDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            mockRepository.Verify(r => r.DrugExists(drugId, cancellationToken), Times.Once);
            mockRepository.Verify(r => r.EditDrug(It.IsAny<UpdateDrugDto>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task UpdateDrug_EditReturnsZero_ReturnsInternalServerError()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugId = Guid.NewGuid();
            var drugDto = new UpdateDrugDto(DrugsID: drugId, Drug: "Aspirin", Description: "Pain relief", Tags: "pain");
            mockRepository.Setup(r => r.DrugExists(drugId, cancellationToken)).ReturnsAsync(true);
            mockRepository.Setup(r => r.EditDrug(drugDto, cancellationToken)).ReturnsAsync(0);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.UpdateDrug(drugDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
        }

        [Fact]
        public async Task UpdateDrug_RepositoryThrowsPostgresException_ReturnsInternalServerError()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugId = Guid.NewGuid();
            var drugDto = new UpdateDrugDto(DrugsID: drugId, Drug: "Aspirin", Description: "Pain relief", Tags: "pain");
            var postgresException = new PostgresException("Update failed", null, null, null);
            mockRepository.Setup(r => r.DrugExists(drugId, cancellationToken)).ReturnsAsync(true);
            mockRepository.Setup(r => r.EditDrug(drugDto, cancellationToken)).ThrowsAsync(postgresException);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.UpdateDrug(drugDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
            mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), postgresException, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task UpdateDrug_RepositoryThrowsGenericException_ReturnsInternalServerError()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugId = Guid.NewGuid();
            var drugDto = new UpdateDrugDto(DrugsID: drugId, Drug: "Aspirin", Description: "Pain relief", Tags: "pain");
            var exception = new Exception("Unexpected error");
            mockRepository.Setup(r => r.DrugExists(drugId, cancellationToken)).ReturnsAsync(true);
            mockRepository.Setup(r => r.EditDrug(drugDto, cancellationToken)).ThrowsAsync(exception);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.UpdateDrug(drugDto);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
            mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), exception, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDrug_DrugExists_ReturnsOk()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugId = Guid.NewGuid();
            mockRepository.Setup(r => r.DrugExists(drugId, cancellationToken)).ReturnsAsync(true);
            mockRepository.Setup(r => r.DeleteDrug(drugId, cancellationToken)).ReturnsAsync(1);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.DeleteDrug(drugId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            mockRepository.Verify(r => r.DrugExists(drugId, cancellationToken), Times.Once);
            mockRepository.Verify(r => r.DeleteDrug(drugId, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task DeleteDrug_DrugNotFound_ReturnsBadRequest()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugId = Guid.NewGuid();
            mockRepository.Setup(r => r.DrugExists(drugId, cancellationToken)).ReturnsAsync(false);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.DeleteDrug(drugId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            mockRepository.Verify(r => r.DrugExists(drugId, cancellationToken), Times.Once);
            mockRepository.Verify(r => r.DeleteDrug(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task DeleteDrug_DeleteReturnsZero_ReturnsBadRequest()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugId = Guid.NewGuid();
            mockRepository.Setup(r => r.DrugExists(drugId, cancellationToken)).ReturnsAsync(true);
            mockRepository.Setup(r => r.DeleteDrug(drugId, cancellationToken)).ReturnsAsync(0);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.DeleteDrug(drugId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task DeleteDrug_RepositoryThrowsPostgresException_ReturnsInternalServerError()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugId = Guid.NewGuid();
            var postgresException = new PostgresException("Delete failed", null, null, null);
            mockRepository.Setup(r => r.DrugExists(drugId, cancellationToken)).ReturnsAsync(true);
            mockRepository.Setup(r => r.DeleteDrug(drugId, cancellationToken)).ThrowsAsync(postgresException);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.DeleteDrug(drugId);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
            mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), postgresException, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDrug_RepositoryThrowsGenericException_ReturnsInternalServerError()
        {
            // Arrange
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            var drugId = Guid.NewGuid();
            var exception = new Exception("Unexpected error");
            mockRepository.Setup(r => r.DrugExists(drugId, cancellationToken)).ReturnsAsync(true);
            mockRepository.Setup(r => r.DeleteDrug(drugId, cancellationToken)).ThrowsAsync(exception);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            var result = await controller.DeleteDrug(drugId);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusResult.StatusCode);
            mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), exception, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000001")]
        [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
        public async Task DeleteDrug_VariousGuids_CallsRepository(string guidString)
        {
            // Arrange
            var drugId = Guid.Parse(guidString);
            var mockRepository = new Mock<IDrugsRepository>();
            var mockLogger = new Mock<ILogger<DrugsController>>();
            var cancellationToken = CancellationToken.None;
            mockRepository.Setup(r => r.DrugExists(drugId, cancellationToken)).ReturnsAsync(true);
            mockRepository.Setup(r => r.DeleteDrug(drugId, cancellationToken)).ReturnsAsync(1);
            var controller = new DrugsController(mockRepository.Object, mockLogger.Object);
            controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            controller.HttpContext.RequestAborted = cancellationToken;

            // Act
            await controller.DeleteDrug(drugId);

            // Assert
            mockRepository.Verify(r => r.DrugExists(drugId, cancellationToken), Times.Once);
            mockRepository.Verify(r => r.DeleteDrug(drugId, cancellationToken), Times.Once);
        }
    }
}
