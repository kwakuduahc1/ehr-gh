using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using ShimsServer.Controllers;
using ShimsServer.Models.DTOs;
using ShimsServer.Models.Schemes;
using ShimsServer.Repositories;
using Xunit;

namespace ShimsServer.Tests.Controllers
{
    public class SchemesControllerTests
    {
        /// <summary>
        /// Tests that GetSchemes calls the repository with the correct cancellation token.
        /// Verifies the controller passes through the injected cancellation token to the repository method.
        /// </summary>
        [Fact]
        public async Task GetSchemes_Always_CallsRepositoryWithCorrectToken()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = new CancellationToken();
            var expectedSchemes = new List<SchemesDTO>();

            mockRepository
                .Setup(r => r.GetAllSchemesAsync(cancellationToken))
                .ReturnsAsync(expectedSchemes);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            await controller.GetSchemes();

            // Assert
            mockRepository.Verify(r => r.GetAllSchemesAsync(cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that GetSchemes handles a cancelled cancellation token properly.
        /// Validates the controller correctly propagates cancellation to the repository.
        /// Expected to throw OperationCanceledException when cancellation is requested.
        /// </summary>
        [Fact]
        public async Task GetSchemes_WithCancelledToken_ThrowsOperationCanceledException()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var cancellationToken = cancellationTokenSource.Token;

            mockRepository
                .Setup(r => r.GetAllSchemesAsync(cancellationToken))
                .ThrowsAsync(new OperationCanceledException(cancellationToken));

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await controller.GetSchemes());
        }

        /// <summary>
        /// Tests that GetSchemeById returns OkObjectResult with the scheme when a valid ID exists in the repository.
        /// Input: A valid Guid that exists in the repository.
        /// Expected: Returns OkObjectResult containing the SchemesDTO object.
        /// </summary>
        [Fact]
        public async Task GetSchemeById_ExistingId_ReturnsOkResultWithScheme()
        {
            // Arrange
            var schemeId = Guid.NewGuid();
            var expectedScheme = new SchemesDTO(schemeId, "Test Scheme", "Full", 1000.00m, 500.00m);
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.GetSchemeByIdAsync(schemeId, cancellationToken))
                .ReturnsAsync(expectedScheme);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.GetSchemeById(schemeId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SchemesDTO>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedScheme = Assert.IsType<SchemesDTO>(okResult.Value);
            Assert.Equal(expectedScheme, returnedScheme);
            mockRepository.Verify(r => r.GetSchemeByIdAsync(schemeId, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that GetSchemeById returns NotFoundResult when the ID does not exist in the repository.
        /// Input: A valid Guid that does not exist in the repository (repository returns null).
        /// Expected: Returns NotFoundResult (HTTP 404).
        /// </summary>
        [Fact]
        public async Task GetSchemeById_NonExistingId_ReturnsNotFoundResult()
        {
            // Arrange
            var schemeId = Guid.NewGuid();
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.GetSchemeByIdAsync(schemeId, cancellationToken))
                .ReturnsAsync((SchemesDTO?)null);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.GetSchemeById(schemeId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SchemesDTO>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
            mockRepository.Verify(r => r.GetSchemeByIdAsync(schemeId, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that GetSchemeById handles Guid.Empty correctly by calling the repository.
        /// Input: Guid.Empty.
        /// Expected: Calls repository and returns NotFoundResult when repository returns null.
        /// </summary>
        [Fact]
        public async Task GetSchemeById_EmptyGuid_ReturnsNotFoundWhenRepositoryReturnsNull()
        {
            // Arrange
            var emptyGuid = Guid.Empty;
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.GetSchemeByIdAsync(emptyGuid, cancellationToken))
                .ReturnsAsync((SchemesDTO?)null);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.GetSchemeById(emptyGuid);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SchemesDTO>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
            mockRepository.Verify(r => r.GetSchemeByIdAsync(emptyGuid, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that GetSchemeById passes the cancellation token to the repository correctly.
        /// Input: A valid Guid and a cancellation token.
        /// Expected: Repository method is called with the correct cancellation token.
        /// </summary>
        [Fact]
        public async Task GetSchemeById_WithCancellationToken_PassesTokenToRepository()
        {
            // Arrange
            var schemeId = Guid.NewGuid();
            var expectedScheme = new SchemesDTO(schemeId, "Test Scheme", "Relative", 2000.00m, 1000.00m);
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            mockRepository
                .Setup(r => r.GetSchemeByIdAsync(schemeId, cancellationToken))
                .ReturnsAsync(expectedScheme);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.GetSchemeById(schemeId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SchemesDTO>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            mockRepository.Verify(r => r.GetSchemeByIdAsync(schemeId, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that GetSchemeById with multiple different valid Guids handles each correctly.
        /// Input: Various valid Guid values that exist in the repository.
        /// Expected: Returns OkObjectResult with the corresponding scheme for each Guid.
        /// </summary>
        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012")]
        [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
        [InlineData("00000000-0000-0000-0000-000000000001")]
        public async Task GetSchemeById_VariousValidGuids_ReturnsOkResultWithScheme(string guidString)
        {
            // Arrange
            var schemeId = Guid.Parse(guidString);
            var expectedScheme = new SchemesDTO(schemeId, "Test Scheme", "Fixed", 5000.00m, 2500.00m);
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.GetSchemeByIdAsync(schemeId, cancellationToken))
                .ReturnsAsync(expectedScheme);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.GetSchemeById(schemeId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SchemesDTO>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedScheme = Assert.IsType<SchemesDTO>(okResult.Value);
            Assert.Equal(expectedScheme.SchemesID, returnedScheme.SchemesID);
        }

        /// <summary>
        /// Tests that GetSchemeById with multiple different Guids returns NotFoundResult when they don't exist.
        /// Input: Various valid Guid values that do not exist in the repository.
        /// Expected: Returns NotFoundResult for each non-existent Guid.
        /// </summary>
        [Theory]
        [InlineData("11111111-1111-1111-1111-111111111111")]
        [InlineData("22222222-2222-2222-2222-222222222222")]
        [InlineData("33333333-3333-3333-3333-333333333333")]
        public async Task GetSchemeById_VariousNonExistingGuids_ReturnsNotFoundResult(string guidString)
        {
            // Arrange
            var schemeId = Guid.Parse(guidString);
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.GetSchemeByIdAsync(schemeId, cancellationToken))
                .ReturnsAsync((SchemesDTO?)null);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.GetSchemeById(schemeId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<SchemesDTO>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        /// <summary>
        /// Tests that AddScheme returns OkObjectResult with a Guid when the scheme is successfully added.
        /// Input: Valid AddSchemeDto with unique scheme name.
        /// Expected: Returns OkObjectResult with Guid value.
        /// </summary>
        [Fact]
        public async Task AddScheme_ValidSchemeWithUniqueName_ReturnsOkWithGuid()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedGuid = Guid.NewGuid();

            var schemeDto = new AddSchemeDto(
                SchemeName: "Test Scheme",
                Coverage: "Full",
                MaxPayable: 1000.50m,
                Recovery: 50.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.AddScheme(schemeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedGuid, okResult.Value);
        }

        /// <summary>
        /// Tests that AddScheme returns ConflictObjectResult when a scheme with the same name already exists.
        /// Input: AddSchemeDto with a scheme name that already exists.
        /// Expected: Returns ConflictObjectResult with appropriate message.
        /// </summary>
        [Fact]
        public async Task AddScheme_SchemeNameAlreadyExists_ReturnsConflict()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new AddSchemeDto(
                SchemeName: "Duplicate Scheme",
                Coverage: "Relative",
                MaxPayable: 500.00m,
                Recovery: 25.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.AddScheme(schemeDto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            var value = conflictResult.Value;
            var messageProperty = value?.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);
            var message = messageProperty.GetValue(value) as string;
            Assert.Equal($"Scheme with name {schemeDto.SchemeName} already exists.", message);
        }

        /// <summary>
        /// Tests that AddScheme returns BadRequestObjectResult when PostgresException is thrown.
        /// Input: Valid AddSchemeDto, but repository throws PostgresException.
        /// Expected: Returns BadRequestObjectResult and logs the error.
        /// </summary>
        [Fact]
        public async Task AddScheme_PostgresExceptionThrown_ReturnsBadRequestAndLogsError()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new AddSchemeDto(
                SchemeName: "Test Scheme",
                Coverage: "Fixed",
                MaxPayable: 750.00m,
                Recovery: 30.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            var postgresException = new PostgresException("Database error", "severity", "invariantSeverity", "23505");

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ThrowsAsync(postgresException);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.AddScheme(schemeDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var value = badRequestResult.Value;
            var messageProperty = value?.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);
            var message = messageProperty.GetValue(value) as string;
            Assert.Equal("There was a database level error", message);

            mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Error inserting scheme {schemeDto.SchemeName}")),
                    postgresException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        /// <summary>
        /// Tests that AddScheme correctly maps DTO properties to Schemes entity.
        /// Input: AddSchemeDto with various property values.
        /// Expected: Schemes entity passed to repository has correct property values from DTO.
        /// </summary>
        [Theory]
        [InlineData("Full Coverage Scheme", "Full", 1000.00, 50.0)]
        [InlineData("Relative Coverage Scheme", "Relative", 2500.50, 75.5)]
        [InlineData("Fixed Coverage Scheme", "Fixed", 0.01, 0.0)]
        public async Task AddScheme_VariousValidInputs_MapsPropertiesCorrectly(
            string schemeName,
            string coverage,
            decimal maxPayable,
            decimal recovery)
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedGuid = Guid.NewGuid();

            var schemeDto = new AddSchemeDto(
                SchemeName: schemeName,
                Coverage: coverage,
                MaxPayable: maxPayable,
                Recovery: recovery
            );

            Schemes? capturedScheme = null;
            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .Callback<Schemes, CancellationToken>((scheme, token) => capturedScheme = scheme)
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            await controller.AddScheme(schemeDto);

            // Assert
            Assert.NotNull(capturedScheme);
            Assert.NotEqual(Guid.Empty, capturedScheme.SchemesID);
            Assert.Equal(schemeName, capturedScheme.SchemeName);
            Assert.Equal(coverage, capturedScheme.Coverage);
            Assert.Equal(maxPayable, capturedScheme.MaxPayable);
            Assert.Equal(recovery, capturedScheme.Recovery);
        }

        /// <summary>
        /// Tests that AddScheme passes the correct cancellation token to repository methods.
        /// Input: Valid AddSchemeDto with a specific cancellation token.
        /// Expected: Repository methods are called with the correct cancellation token.
        /// </summary>
        [Fact]
        public async Task AddScheme_ValidScheme_PassesCancellationTokenToRepository()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            var expectedGuid = Guid.NewGuid();

            var schemeDto = new AddSchemeDto(
                SchemeName: "Test Scheme",
                Coverage: "Full",
                MaxPayable: 1000.00m,
                Recovery: 50.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            await controller.AddScheme(schemeDto);

            // Assert
            mockRepository.Verify(
                r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken),
                Times.Once);

            mockRepository.Verify(
                r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken),
                Times.Once);
        }

        /// <summary>
        /// Tests that AddScheme handles minimum valid MaxPayable value correctly.
        /// Input: AddSchemeDto with MaxPayable set to minimum valid value (0.01).
        /// Expected: Returns OkObjectResult with Guid.
        /// </summary>
        [Fact]
        public async Task AddScheme_MinimumMaxPayableValue_ReturnsOk()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedGuid = Guid.NewGuid();

            var schemeDto = new AddSchemeDto(
                SchemeName: "Minimum MaxPayable Scheme",
                Coverage: "Full",
                MaxPayable: 0.01m,
                Recovery: 10.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.AddScheme(schemeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedGuid, okResult.Value);
        }

        /// <summary>
        /// Tests that AddScheme handles maximum decimal value for MaxPayable correctly.
        /// Input: AddSchemeDto with MaxPayable set to decimal.MaxValue.
        /// Expected: Returns OkObjectResult with Guid.
        /// </summary>
        [Fact]
        public async Task AddScheme_MaximumMaxPayableValue_ReturnsOk()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedGuid = Guid.NewGuid();

            var schemeDto = new AddSchemeDto(
                SchemeName: "Maximum MaxPayable Scheme",
                Coverage: "Relative",
                MaxPayable: decimal.MaxValue,
                Recovery: 100.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.AddScheme(schemeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedGuid, okResult.Value);
        }

        /// <summary>
        /// Tests that AddScheme handles zero Recovery value correctly.
        /// Input: AddSchemeDto with Recovery set to 0.
        /// Expected: Returns OkObjectResult with Guid.
        /// </summary>
        [Fact]
        public async Task AddScheme_ZeroRecoveryValue_ReturnsOk()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedGuid = Guid.NewGuid();

            var schemeDto = new AddSchemeDto(
                SchemeName: "Zero Recovery Scheme",
                Coverage: "Fixed",
                MaxPayable: 500.00m,
                Recovery: 0.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.AddScheme(schemeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedGuid, okResult.Value);
        }

        /// <summary>
        /// Tests that AddScheme handles scheme names with special characters correctly.
        /// Input: AddSchemeDto with scheme name containing special characters.
        /// Expected: Returns OkObjectResult with Guid.
        /// </summary>
        [Theory]
        [InlineData("Scheme@2024")]
        [InlineData("Scheme-With-Dashes")]
        [InlineData("Scheme_With_Underscores")]
        [InlineData("Scheme (With Parentheses)")]
        [InlineData("Scheme's Name")]
        public async Task AddScheme_SchemeNameWithSpecialCharacters_ReturnsOk(string schemeName)
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedGuid = Guid.NewGuid();

            var schemeDto = new AddSchemeDto(
                SchemeName: schemeName,
                Coverage: "Full",
                MaxPayable: 1000.00m,
                Recovery: 50.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.AddScheme(schemeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedGuid, okResult.Value);
        }

        /// <summary>
        /// Tests that AddScheme handles very long scheme names correctly.
        /// Input: AddSchemeDto with a 50-character scheme name (maximum length).
        /// Expected: Returns OkObjectResult with Guid.
        /// </summary>
        [Fact]
        public async Task AddScheme_VeryLongSchemeName_ReturnsOk()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedGuid = Guid.NewGuid();

            var schemeDto = new AddSchemeDto(
                SchemeName: new string('A', 50),
                Coverage: "Full",
                MaxPayable: 1000.00m,
                Recovery: 50.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.AddScheme(schemeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedGuid, okResult.Value);
        }

        /// <summary>
        /// Tests that AddScheme does not call AddSchemeAsync when scheme name already exists.
        /// Input: AddSchemeDto with a scheme name that already exists.
        /// Expected: AddSchemeAsync is never called.
        /// </summary>
        [Fact]
        public async Task AddScheme_SchemeNameAlreadyExists_DoesNotCallAddSchemeAsync()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new AddSchemeDto(
                SchemeName: "Duplicate Scheme",
                Coverage: "Full",
                MaxPayable: 1000.00m,
                Recovery: 50.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            await controller.AddScheme(schemeDto);

            // Assert
            mockRepository.Verify(
                r => r.AddSchemeAsync(It.IsAny<Schemes>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        /// <summary>
        /// Tests that AddScheme generates a non-empty GUID for SchemesID.
        /// Input: Valid AddSchemeDto.
        /// Expected: Schemes entity passed to repository has non-empty SchemesID.
        /// </summary>
        [Fact]
        public async Task AddScheme_ValidScheme_GeneratesNonEmptyGuid()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedGuid = Guid.NewGuid();

            var schemeDto = new AddSchemeDto(
                SchemeName: "Test Scheme",
                Coverage: "Full",
                MaxPayable: 1000.00m,
                Recovery: 50.0m
            );

            Schemes? capturedScheme = null;
            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .Callback<Schemes, CancellationToken>((scheme, token) => capturedScheme = scheme)
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            await controller.AddScheme(schemeDto);

            // Assert
            Assert.NotNull(capturedScheme);
            Assert.NotEqual(Guid.Empty, capturedScheme.SchemesID);
        }

        /// <summary>
        /// Tests that DeleteScheme returns Ok when the scheme is successfully deleted.
        /// </summary>
        [Fact]
        public async Task DeleteScheme_ExistingScheme_ReturnsOk()
        {
            // Arrange
            var schemeId = Guid.NewGuid();
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.DeleteSchemeAsync(schemeId, cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.DeleteScheme(schemeId);

            // Assert
            Assert.IsType<OkResult>(result);
            mockRepository.Verify(r => r.DeleteSchemeAsync(schemeId, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that DeleteScheme returns NotFound when the scheme does not exist.
        /// </summary>
        [Fact]
        public async Task DeleteScheme_NonExistentScheme_ReturnsNotFound()
        {
            // Arrange
            var schemeId = Guid.NewGuid();
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.DeleteSchemeAsync(schemeId, cancellationToken))
                .ReturnsAsync(false);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.DeleteScheme(schemeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            mockRepository.Verify(r => r.DeleteSchemeAsync(schemeId, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that DeleteScheme handles empty Guid correctly and returns NotFound when scheme is not found.
        /// </summary>
        [Fact]
        public async Task DeleteScheme_EmptyGuid_ReturnsNotFound()
        {
            // Arrange
            var schemeId = Guid.Empty;
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.DeleteSchemeAsync(schemeId, cancellationToken))
                .ReturnsAsync(false);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.DeleteScheme(schemeId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            mockRepository.Verify(r => r.DeleteSchemeAsync(schemeId, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that DeleteScheme passes the correct cancellation token to the repository.
        /// </summary>
        [Fact]
        public async Task DeleteScheme_UsesCancellationToken_PassesTokenToRepository()
        {
            // Arrange
            var schemeId = Guid.NewGuid();
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            mockRepository
                .Setup(r => r.DeleteSchemeAsync(schemeId, cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.DeleteScheme(schemeId);

            // Assert
            Assert.IsType<OkResult>(result);
            mockRepository.Verify(r => r.DeleteSchemeAsync(schemeId, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that UpdateScheme returns AcceptedResult when the scheme is successfully updated.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_ValidSchemeDtoAndSuccessfulUpdate_ReturnsAcceptedResult()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "Test Scheme",
                "Full",
                100.50m,
                50.25m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            Assert.IsType<AcceptedResult>(result);
        }

        /// <summary>
        /// Tests that UpdateScheme returns BadRequestObjectResult with appropriate message when scheme does not exist.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_SchemeDoesNotExist_ReturnsBadRequestWithMessage()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "Nonexistent Scheme",
                "Relative",
                200.00m,
                75.00m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(false);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
            var message = badRequestResult.Value.GetType().GetProperty("Message")?.GetValue(badRequestResult.Value);
            Assert.Equal($"Scheme {schemeDto.SchemeName} does not exist", message);
        }

        /// <summary>
        /// Tests that UpdateScheme logs error and returns BadRequest when PostgresException is thrown.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_PostgresExceptionThrown_LogsErrorAndReturnsBadRequest()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "Test Scheme",
                "Fixed",
                150.00m,
                25.00m
            );

            var postgresException = new PostgresException("Test error", "Error", "Error", "23505");

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ThrowsAsync(postgresException);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
            var message = badRequestResult.Value.GetType().GetProperty("message")?.GetValue(badRequestResult.Value);
            Assert.Equal("There was a database level error", message);

            mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Error updating scheme")),
                    postgresException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        /// <summary>
        /// Tests that UpdateScheme correctly maps UpdateSchemeDto properties to Schemes entity.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_ValidSchemeDto_CallsRepositoryWithCorrectlyMappedScheme()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeId = Guid.NewGuid();
            var schemeDto = new UpdateSchemeDto(
                schemeId,
                "Mapped Scheme",
                "Full",
                300.75m,
                80.50m
            );

            Schemes? capturedScheme = null;
            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .Callback<Schemes, CancellationToken>((s, ct) => capturedScheme = s)
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            await controller.UpdateScheme(schemeDto);

            // Assert
            Assert.NotNull(capturedScheme);
            Assert.Equal(schemeId, capturedScheme.SchemesID);
            Assert.Equal("Mapped Scheme", capturedScheme.SchemeName);
            Assert.Equal("Full", capturedScheme.Coverage);
            Assert.Equal(300.75m, capturedScheme.MaxPayable);
            Assert.Equal(80.50m, capturedScheme.Recovery);
        }

        /// <summary>
        /// Tests that UpdateScheme passes the cancellation token to the repository method.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_ValidSchemeDto_PassesCancellationTokenToRepository()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "Test Scheme",
                "Relative",
                100.00m,
                50.00m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            await controller.UpdateScheme(schemeDto);

            // Assert
            mockRepository.Verify(
                r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken),
                Times.Once);
        }

        /// <summary>
        /// Tests UpdateScheme with various edge case values for decimal properties.
        /// </summary>
        /// <param name="maxPayable">The MaxPayable value to test.</param>
        /// <param name="recovery">The Recovery value to test.</param>
        [Theory]
        [InlineData(0.01, 0)]
        [InlineData(0.01, 1)]
        [InlineData(999999999.99, 100)]
        [InlineData(1.00, 50.50)]
        public async Task UpdateScheme_EdgeCaseDecimalValues_ProcessesSuccessfully(decimal maxPayable, decimal recovery)
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "Edge Case Scheme",
                "Full",
                maxPayable,
                recovery
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            mockRepository.Verify(
                r => r.UpdateSchemeAsync(
                    It.Is<Schemes>(s => s.MaxPayable == maxPayable && s.Recovery == recovery),
                    cancellationToken),
                Times.Once);
        }

        /// <summary>
        /// Tests UpdateScheme with different valid Coverage values.
        /// </summary>
        /// <param name="coverage">The Coverage value to test.</param>
        [Theory]
        [InlineData("Full")]
        [InlineData("Relative")]
        [InlineData("Fixed")]
        public async Task UpdateScheme_DifferentCoverageValues_ProcessesSuccessfully(string coverage)
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "Coverage Test Scheme",
                coverage,
                100.00m,
                50.00m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            mockRepository.Verify(
                r => r.UpdateSchemeAsync(
                    It.Is<Schemes>(s => s.Coverage == coverage),
                    cancellationToken),
                Times.Once);
        }

        /// <summary>
        /// Tests UpdateScheme with Guid.Empty as the scheme ID.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_EmptyGuid_ProcessesRequest()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.Empty,
                "Empty Guid Scheme",
                "Full",
                100.00m,
                50.00m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(false);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            mockRepository.Verify(
                r => r.UpdateSchemeAsync(
                    It.Is<Schemes>(s => s.SchemesID == Guid.Empty),
                    cancellationToken),
                Times.Once);
        }

        /// <summary>
        /// Tests UpdateScheme with minimum valid string length for SchemeName.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_MinimumLengthSchemeName_ProcessesSuccessfully()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "ABC",
                "Full",
                100.00m,
                50.00m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            mockRepository.Verify(
                r => r.UpdateSchemeAsync(
                    It.Is<Schemes>(s => s.SchemeName == "ABC"),
                    cancellationToken),
                Times.Once);
        }

        /// <summary>
        /// Tests UpdateScheme with maximum valid string length for SchemeName.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_MaximumLengthSchemeName_ProcessesSuccessfully()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var longSchemeName = new string('A', 50);
            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                longSchemeName,
                "Relative",
                100.00m,
                50.00m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            mockRepository.Verify(
                r => r.UpdateSchemeAsync(
                    It.Is<Schemes>(s => s.SchemeName == longSchemeName),
                    cancellationToken),
                Times.Once);
        }

        /// <summary>
        /// Tests that GetSchemeById propagates OperationCanceledException when the cancellation token is cancelled.
        /// Input: A valid Guid with a cancelled cancellation token.
        /// Expected: Throws OperationCanceledException when repository operation is cancelled.
        /// </summary>
        [Fact]
        public async Task GetSchemeById_WithCancelledToken_ThrowsOperationCanceledException()
        {
            // Arrange
            var schemeId = Guid.NewGuid();
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var cancellationToken = cancellationTokenSource.Token;

            mockRepository
                .Setup(r => r.GetSchemeByIdAsync(schemeId, cancellationToken))
                .ThrowsAsync(new OperationCanceledException());

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => controller.GetSchemeById(schemeId));
        }

        /// <summary>
        /// Tests that AddScheme throws OperationCanceledException when the cancellation token is already cancelled.
        /// Input: Valid AddSchemeDto with a cancelled cancellation token.
        /// Expected: Throws OperationCanceledException when SchemeExistsByNameAsync is called.
        /// </summary>
        [Fact]
        public async Task AddScheme_WithCancelledToken_ThrowsOperationCanceledException()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var cancelledToken = cancellationTokenSource.Token;

            var schemeDto = new AddSchemeDto(
                SchemeName: "Test Scheme",
                Coverage: "Full",
                MaxPayable: 1000.00m,
                Recovery: 50.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancelledToken);

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() => controller.AddScheme(schemeDto));
        }

        /// <summary>
        /// Tests that AddScheme handles Recovery value at its maximum boundary (100.0) correctly.
        /// Input: AddSchemeDto with Recovery set to 100.0 (maximum valid per entity validation).
        /// Expected: Returns OkObjectResult with Guid.
        /// </summary>
        [Fact]
        public async Task AddScheme_MaximumRecoveryValue_ReturnsOk()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedGuid = Guid.NewGuid();

            var schemeDto = new AddSchemeDto(
                SchemeName: "Max Recovery Scheme",
                Coverage: "Full",
                MaxPayable: 1000.00m,
                Recovery: 100.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.AddScheme(schemeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedGuid, okResult.Value);
        }

        /// <summary>
        /// Tests that AddScheme handles Recovery value at its minimum entity boundary (1.0) correctly.
        /// Input: AddSchemeDto with Recovery set to 1.0 (minimum valid per entity validation).
        /// Expected: Returns OkObjectResult with Guid.
        /// </summary>
        [Fact]
        public async Task AddScheme_MinimumEntityRecoveryValue_ReturnsOk()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedGuid = Guid.NewGuid();

            var schemeDto = new AddSchemeDto(
                SchemeName: "Min Recovery Scheme",
                Coverage: "Relative",
                MaxPayable: 500.00m,
                Recovery: 1.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.AddScheme(schemeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedGuid, okResult.Value);
        }

        /// <summary>
        /// Tests that AddScheme does not catch non-PostgresException exceptions.
        /// Input: Valid AddSchemeDto, but repository throws InvalidOperationException.
        /// Expected: Throws InvalidOperationException without catching it.
        /// </summary>
        [Fact]
        public async Task AddScheme_NonPostgresExceptionThrown_PropagatesException()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new AddSchemeDto(
                SchemeName: "Test Scheme",
                Coverage: "Fixed",
                MaxPayable: 750.00m,
                Recovery: 30.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ThrowsAsync(new InvalidOperationException("Non-Postgres error"));

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => controller.AddScheme(schemeDto));
        }

        /// <summary>
        /// Tests that AddScheme handles Recovery values at various boundary points correctly.
        /// Input: AddSchemeDto with Recovery values at 1.0, 50.0, 99.9, and 100.0.
        /// Expected: Returns OkObjectResult with Guid for all boundary values.
        /// </summary>
        [Theory]
        [InlineData(1.0)]
        [InlineData(50.0)]
        [InlineData(99.9)]
        [InlineData(100.0)]
        public async Task AddScheme_RecoveryBoundaryValues_ReturnsOk(decimal recovery)
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedGuid = Guid.NewGuid();

            var schemeDto = new AddSchemeDto(
                SchemeName: $"Recovery {recovery} Scheme",
                Coverage: "Full",
                MaxPayable: 1000.00m,
                Recovery: recovery
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.AddScheme(schemeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedGuid, okResult.Value);
        }

        /// <summary>
        /// Tests that AddScheme correctly passes Recovery value to the Schemes entity.
        /// Input: AddSchemeDto with specific Recovery value.
        /// Expected: Schemes entity passed to repository contains the correct Recovery value.
        /// </summary>
        [Fact]
        public async Task AddScheme_ValidRecovery_PassesCorrectRecoveryToRepository()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedGuid = Guid.NewGuid();
            var expectedRecovery = 75.5m;
            Schemes? capturedScheme = null;

            var schemeDto = new AddSchemeDto(
                SchemeName: "Test Recovery Scheme",
                Coverage: "Relative",
                MaxPayable: 2000.00m,
                Recovery: expectedRecovery
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .Callback<Schemes, CancellationToken>((scheme, token) => capturedScheme = scheme)
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.AddScheme(schemeDto);

            // Assert
            Assert.NotNull(capturedScheme);
            Assert.Equal(expectedRecovery, capturedScheme.Recovery);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedGuid, okResult.Value);
        }

        /// <summary>
        /// Tests that AddScheme handles MaxPayable at minimum valid boundary (0.01) and Recovery at minimum entity boundary (1.0).
        /// Input: AddSchemeDto with MaxPayable = 0.01 and Recovery = 1.0.
        /// Expected: Returns OkObjectResult with Guid.
        /// </summary>
        [Fact]
        public async Task AddScheme_MinimumMaxPayableAndMinimumRecovery_ReturnsOk()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedGuid = Guid.NewGuid();

            var schemeDto = new AddSchemeDto(
                SchemeName: "Minimum Values Scheme",
                Coverage: "Fixed",
                MaxPayable: 0.01m,
                Recovery: 1.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.AddScheme(schemeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedGuid, okResult.Value);
        }

        /// <summary>
        /// Tests that AddScheme handles MaxPayable at maximum and Recovery at maximum entity boundary (100.0).
        /// Input: AddSchemeDto with MaxPayable = decimal.MaxValue and Recovery = 100.0.
        /// Expected: Returns OkObjectResult with Guid.
        /// </summary>
        [Fact]
        public async Task AddScheme_MaximumMaxPayableAndMaximumRecovery_ReturnsOk()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedGuid = Guid.NewGuid();

            var schemeDto = new AddSchemeDto(
                SchemeName: "Maximum Values Scheme",
                Coverage: "Full",
                MaxPayable: decimal.MaxValue,
                Recovery: 100.0m
            );

            mockRepository
                .Setup(r => r.SchemeExistsByNameAsync(schemeDto.SchemeName, cancellationToken))
                .ReturnsAsync(false);

            mockRepository
                .Setup(r => r.AddSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(expectedGuid);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.AddScheme(schemeDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedGuid, okResult.Value);
        }

        /// <summary>
        /// Tests that UpdateScheme handles scheme names with special characters correctly.
        /// Input: UpdateSchemeDto with special characters in SchemeName.
        /// Expected: Returns AcceptedResult and correctly maps the scheme name with special characters.
        /// </summary>
        [Theory]
        [InlineData("Test@Scheme")]
        [InlineData("Scheme-2024")]
        [InlineData("Scheme_Name")]
        [InlineData("Scheme (Test)")]
        [InlineData("Test's Scheme")]
        [InlineData("Scheme#123")]
        public async Task UpdateScheme_SchemeNameWithSpecialCharacters_ProcessesSuccessfully(string schemeName)
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                schemeName,
                "Full",
                100.00m,
                50.00m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            mockRepository.Verify(
                r => r.UpdateSchemeAsync(
                    It.Is<Schemes>(s => s.SchemeName == schemeName),
                    cancellationToken),
                Times.Once);
        }

        /// <summary>
        /// Tests that UpdateScheme handles decimal.MaxValue for MaxPayable correctly.
        /// Input: UpdateSchemeDto with MaxPayable set to decimal.MaxValue.
        /// Expected: Returns AcceptedResult and correctly processes the maximum decimal value.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_MaximumDecimalMaxPayable_ProcessesSuccessfully()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "Max Payable Test",
                "Fixed",
                decimal.MaxValue,
                50.00m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            mockRepository.Verify(
                r => r.UpdateSchemeAsync(
                    It.Is<Schemes>(s => s.MaxPayable == decimal.MaxValue),
                    cancellationToken),
                Times.Once);
        }

        /// <summary>
        /// Tests that UpdateScheme handles decimal.MaxValue for Recovery correctly.
        /// Input: UpdateSchemeDto with Recovery set to decimal.MaxValue.
        /// Expected: Returns AcceptedResult and correctly processes the maximum decimal value.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_MaximumDecimalRecovery_ProcessesSuccessfully()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "Max Recovery Test",
                "Relative",
                100.00m,
                decimal.MaxValue
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            mockRepository.Verify(
                r => r.UpdateSchemeAsync(
                    It.Is<Schemes>(s => s.Recovery == decimal.MaxValue),
                    cancellationToken),
                Times.Once);
        }

        /// <summary>
        /// Tests that UpdateScheme processes various valid Guid values correctly.
        /// Input: UpdateSchemeDto with different valid Guid values.
        /// Expected: Returns AcceptedResult for each valid Guid.
        /// </summary>
        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012")]
        [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
        [InlineData("00000000-0000-0000-0000-000000000001")]
        [InlineData("a1b2c3d4-e5f6-7890-abcd-ef1234567890")]
        public async Task UpdateScheme_VariousValidGuids_ProcessesSuccessfully(string guidString)
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var guid = Guid.Parse(guidString);
            var schemeDto = new UpdateSchemeDto(
                guid,
                "Guid Test Scheme",
                "Full",
                100.00m,
                50.00m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            mockRepository.Verify(
                r => r.UpdateSchemeAsync(
                    It.Is<Schemes>(s => s.SchemesID == guid),
                    cancellationToken),
                Times.Once);
        }

        /// <summary>
        /// Tests that UpdateScheme returns BadRequest with correct message property when scheme does not exist.
        /// Input: UpdateSchemeDto for non-existent scheme.
        /// Expected: Returns BadRequestObjectResult with "Message" property (capital M) containing the error message.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_SchemeDoesNotExist_ReturnsBadRequestWithCapitalMessageProperty()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeName = "NonExistent Scheme";
            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                schemeName,
                "Full",
                100.00m,
                50.00m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(false);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);

            var messageProperty = badRequestResult.Value.GetType().GetProperty("Message");
            Assert.NotNull(messageProperty);

            var messageValue = messageProperty?.GetValue(badRequestResult.Value);
            Assert.Equal($"Scheme {schemeName} does not exist", messageValue);
        }

        /// <summary>
        /// Tests that UpdateScheme returns BadRequest with correct lowercase message property when PostgresException occurs.
        /// Input: UpdateSchemeDto that triggers PostgresException.
        /// Expected: Returns BadRequestObjectResult with "message" property (lowercase m) containing the database error message.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_PostgresExceptionThrown_ReturnsBadRequestWithLowercaseMessageProperty()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "Database Error Scheme",
                "Fixed",
                100.00m,
                50.00m
            );

            var postgresException = new PostgresException("Test error", "Error", "Error", "23505");
            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ThrowsAsync(postgresException);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);

            var messageProperty = badRequestResult.Value.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);

            var messageValue = messageProperty?.GetValue(badRequestResult.Value);
            Assert.Equal("There was a database level error", messageValue);
        }

        /// <summary>
        /// Tests that UpdateScheme correctly logs the scheme name when PostgresException is thrown.
        /// Input: UpdateSchemeDto that triggers PostgresException with specific scheme name.
        /// Expected: Logs error message containing the scheme name and exception details.
        /// </summary>
        [Theory]
        [InlineData("Test Scheme 1")]
        [InlineData("Production Scheme")]
        [InlineData("Dev@Scheme#123")]
        public async Task UpdateScheme_PostgresExceptionThrown_LogsErrorWithSchemeName(string schemeName)
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                schemeName,
                "Full",
                100.00m,
                50.00m
            );

            var postgresException = new PostgresException("Test error", "Error", "Error", "23505");
            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ThrowsAsync(postgresException);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            mockLogger.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains($"Error updating scheme") && v.ToString()!.Contains(schemeName)),
                    postgresException,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        /// <summary>
        /// Tests that UpdateScheme handles combination of minimum MaxPayable and maximum Recovery correctly.
        /// Input: UpdateSchemeDto with MaxPayable = 0.01 and Recovery = decimal.MaxValue.
        /// Expected: Returns AcceptedResult and correctly processes both boundary values.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_MinimumMaxPayableAndMaximumRecovery_ProcessesSuccessfully()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "Boundary Combination Test",
                "Relative",
                0.01m,
                decimal.MaxValue
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            mockRepository.Verify(
                r => r.UpdateSchemeAsync(
                    It.Is<Schemes>(s => s.MaxPayable == 0.01m && s.Recovery == decimal.MaxValue),
                    cancellationToken),
                Times.Once);
        }

        /// <summary>
        /// Tests that UpdateScheme handles combination of maximum MaxPayable and minimum Recovery correctly.
        /// Input: UpdateSchemeDto with MaxPayable = decimal.MaxValue and Recovery = 0.
        /// Expected: Returns AcceptedResult and correctly processes both boundary values.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_MaximumMaxPayableAndMinimumRecovery_ProcessesSuccessfully()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "Boundary Combination Test 2",
                "Fixed",
                decimal.MaxValue,
                0m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.UpdateScheme(schemeDto);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            mockRepository.Verify(
                r => r.UpdateSchemeAsync(
                    It.Is<Schemes>(s => s.MaxPayable == decimal.MaxValue && s.Recovery == 0m),
                    cancellationToken),
                Times.Once);
        }

        /// <summary>
        /// Tests that UpdateScheme verifies the repository is called exactly once regardless of outcome.
        /// Input: UpdateSchemeDto with valid data.
        /// Expected: Repository UpdateSchemeAsync is called exactly once.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_ValidRequest_CallsRepositoryExactlyOnce()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "Single Call Test",
                "Full",
                100.00m,
                50.00m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            await controller.UpdateScheme(schemeDto);

            // Assert
            mockRepository.Verify(
                r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken),
                Times.Once);
        }

        /// <summary>
        /// Tests that UpdateScheme does not log when operation succeeds.
        /// Input: UpdateSchemeDto with valid data that succeeds.
        /// Expected: Logger is not called at all when operation succeeds.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_SuccessfulUpdate_DoesNotLog()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "No Log Test",
                "Relative",
                100.00m,
                50.00m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            await controller.UpdateScheme(schemeDto);

            // Assert
            mockLogger.Verify(
                l => l.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }

        /// <summary>
        /// Tests that UpdateScheme does not log when scheme does not exist.
        /// Input: UpdateSchemeDto for non-existent scheme (repository returns false).
        /// Expected: Logger is not called when scheme is not found.
        /// </summary>
        [Fact]
        public async Task UpdateScheme_SchemeNotFound_DoesNotLog()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            var schemeDto = new UpdateSchemeDto(
                Guid.NewGuid(),
                "Not Found No Log Test",
                "Fixed",
                100.00m,
                50.00m
            );

            mockRepository
                .Setup(r => r.UpdateSchemeAsync(It.IsAny<Schemes>(), cancellationToken))
                .ReturnsAsync(false);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            await controller.UpdateScheme(schemeDto);

            // Assert
            mockLogger.Verify(
                l => l.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }

        /// <summary>
        /// Tests that DeleteScheme with various valid Guids processes correctly when deleted.
        /// Input: Different valid Guid values that exist in the repository.
        /// Expected: Returns OkResult for each successfully deleted scheme.
        /// </summary>
        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012")]
        [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
        [InlineData("00000000-0000-0000-0000-000000000001")]
        [InlineData("a1b2c3d4-e5f6-7890-abcd-ef1234567890")]
        public async Task DeleteScheme_VariousValidGuids_ReturnsOk(string guidString)
        {
            // Arrange
            var schemeId = Guid.Parse(guidString);
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.DeleteSchemeAsync(schemeId, cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.DeleteScheme(schemeId);

            // Assert
            Assert.IsType<OkResult>(result);
            mockRepository.Verify(r => r.DeleteSchemeAsync(schemeId, cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that DeleteScheme does not call logger when deletion is successful.
        /// Input: Valid Guid with successful deletion.
        /// Expected: Logger.LogError is never called.
        /// </summary>
        [Fact]
        public async Task DeleteScheme_SuccessfulDeletion_DoesNotLogError()
        {
            // Arrange
            var schemeId = Guid.NewGuid();
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.DeleteSchemeAsync(schemeId, cancellationToken))
                .ReturnsAsync(true);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            await controller.DeleteScheme(schemeId);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }

        /// <summary>
        /// Tests that DeleteScheme does not call logger when scheme is not found.
        /// Input: Valid Guid with repository returning false.
        /// Expected: Logger.LogError is never called.
        /// </summary>
        [Fact]
        public async Task DeleteScheme_NotFound_DoesNotLogError()
        {
            // Arrange
            var schemeId = Guid.NewGuid();
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;

            mockRepository
                .Setup(r => r.DeleteSchemeAsync(schemeId, cancellationToken))
                .ReturnsAsync(false);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            await controller.DeleteScheme(schemeId);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }

        /// <summary>
        /// Tests that GetSchemes returns OkObjectResult with an empty collection when repository returns no schemes.
        /// Input: Repository returns empty collection.
        /// Expected: Returns OkObjectResult containing empty IEnumerable.
        /// </summary>
        [Fact]
        public async Task GetSchemes_EmptyCollection_ReturnsOkWithEmptyCollection()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var emptySchemes = new List<SchemesDTO>();

            mockRepository
                .Setup(r => r.GetAllSchemesAsync(cancellationToken))
                .ReturnsAsync(emptySchemes);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.GetSchemes();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<SchemesDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedSchemes = Assert.IsAssignableFrom<IEnumerable<SchemesDTO>>(okResult.Value);
            Assert.Empty(returnedSchemes);
            mockRepository.Verify(r => r.GetAllSchemesAsync(cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that GetSchemes returns OkObjectResult with a single scheme when repository returns one scheme.
        /// Input: Repository returns collection with one SchemesDTO.
        /// Expected: Returns OkObjectResult containing single scheme with correct properties.
        /// </summary>
        [Fact]
        public async Task GetSchemes_SingleScheme_ReturnsOkWithSingleScheme()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var schemeId = Guid.NewGuid();
            var expectedScheme = new SchemesDTO(schemeId, "Test Scheme", "Full", 1000.00m, 50.00m);
            var schemes = new List<SchemesDTO> { expectedScheme };

            mockRepository
                .Setup(r => r.GetAllSchemesAsync(cancellationToken))
                .ReturnsAsync(schemes);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.GetSchemes();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<SchemesDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedSchemes = Assert.IsAssignableFrom<IEnumerable<SchemesDTO>>(okResult.Value);
            var schemesList = returnedSchemes.ToList();
            Assert.Single(schemesList);
            Assert.Equal(expectedScheme, schemesList[0]);
            mockRepository.Verify(r => r.GetAllSchemesAsync(cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that GetSchemes returns OkObjectResult with multiple schemes when repository returns multiple schemes.
        /// Input: Repository returns collection with multiple SchemesDTO objects.
        /// Expected: Returns OkObjectResult containing all schemes in correct order.
        /// </summary>
        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        public async Task GetSchemes_MultipleSchemes_ReturnsOkWithAllSchemes(int count)
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedSchemes = new List<SchemesDTO>();
            for (int i = 0; i < count; i++)
            {
                expectedSchemes.Add(new SchemesDTO(
                    Guid.NewGuid(),
                    $"Scheme {i}",
                    "Full",
                    1000.00m + i,
                    50.00m + i
                ));
            }

            mockRepository
                .Setup(r => r.GetAllSchemesAsync(cancellationToken))
                .ReturnsAsync(expectedSchemes);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.GetSchemes();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<SchemesDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedSchemes = Assert.IsAssignableFrom<IEnumerable<SchemesDTO>>(okResult.Value);
            var schemesList = returnedSchemes.ToList();
            Assert.Equal(count, schemesList.Count);
            Assert.Equal(expectedSchemes, schemesList);
            mockRepository.Verify(r => r.GetAllSchemesAsync(cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that GetSchemes returns OkObjectResult with schemes having different coverage types.
        /// Input: Repository returns schemes with various coverage values (Full, Relative, Fixed).
        /// Expected: Returns OkObjectResult containing all schemes with different coverage types preserved.
        /// </summary>
        [Fact]
        public async Task GetSchemes_DifferentCoverageTypes_ReturnsOkWithAllSchemes()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedSchemes = new List<SchemesDTO>
            {
                new SchemesDTO(Guid.NewGuid(), "Full Coverage Scheme", "Full", 1000.00m, 50.00m),
                new SchemesDTO(Guid.NewGuid(), "Relative Coverage Scheme", "Relative", 2000.00m, 75.00m),
                new SchemesDTO(Guid.NewGuid(), "Fixed Coverage Scheme", "Fixed", 3000.00m, 100.00m)
            };

            mockRepository
                .Setup(r => r.GetAllSchemesAsync(cancellationToken))
                .ReturnsAsync(expectedSchemes);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.GetSchemes();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<SchemesDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedSchemes = Assert.IsAssignableFrom<IEnumerable<SchemesDTO>>(okResult.Value);
            var schemesList = returnedSchemes.ToList();
            Assert.Equal(3, schemesList.Count);
            Assert.Contains(schemesList, s => s.Coverage == "Full");
            Assert.Contains(schemesList, s => s.Coverage == "Relative");
            Assert.Contains(schemesList, s => s.Coverage == "Fixed");
            mockRepository.Verify(r => r.GetAllSchemesAsync(cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that GetSchemes returns OkObjectResult with schemes having edge case decimal values.
        /// Input: Repository returns schemes with minimum and maximum valid decimal values for MaxPayable and Recovery.
        /// Expected: Returns OkObjectResult containing schemes with edge case decimal values preserved.
        /// </summary>
        [Fact]
        public async Task GetSchemes_EdgeCaseDecimalValues_ReturnsOkWithCorrectValues()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedSchemes = new List<SchemesDTO>
            {
                new SchemesDTO(Guid.NewGuid(), "Min MaxPayable", "Full", 0.01m, 0m),
                new SchemesDTO(Guid.NewGuid(), "Max MaxPayable", "Relative", decimal.MaxValue, decimal.MaxValue),
                new SchemesDTO(Guid.NewGuid(), "Large Values", "Fixed", 999999999.99m, 999999999.99m)
            };

            mockRepository
                .Setup(r => r.GetAllSchemesAsync(cancellationToken))
                .ReturnsAsync(expectedSchemes);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.GetSchemes();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<SchemesDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedSchemes = Assert.IsAssignableFrom<IEnumerable<SchemesDTO>>(okResult.Value);
            var schemesList = returnedSchemes.ToList();
            Assert.Equal(3, schemesList.Count);
            Assert.Contains(schemesList, s => s.MaxPayable == 0.01m);
            Assert.Contains(schemesList, s => s.MaxPayable == decimal.MaxValue);
            Assert.Contains(schemesList, s => s.Recovery == 0m);
            mockRepository.Verify(r => r.GetAllSchemesAsync(cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that GetSchemes returns OkObjectResult with schemes having special characters in scheme names.
        /// Input: Repository returns schemes with various special characters in SchemeName.
        /// Expected: Returns OkObjectResult containing schemes with special characters preserved.
        /// </summary>
        [Fact]
        public async Task GetSchemes_SchemeNamesWithSpecialCharacters_ReturnsOkWithCorrectNames()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedSchemes = new List<SchemesDTO>
            {
                new SchemesDTO(Guid.NewGuid(), "Scheme@2024", "Full", 1000.00m, 50.00m),
                new SchemesDTO(Guid.NewGuid(), "Scheme-With-Dashes", "Relative", 2000.00m, 75.00m),
                new SchemesDTO(Guid.NewGuid(), "Scheme_With_Underscores", "Fixed", 3000.00m, 100.00m),
                new SchemesDTO(Guid.NewGuid(), "Scheme (With Parentheses)", "Full", 4000.00m, 125.00m),
                new SchemesDTO(Guid.NewGuid(), "Scheme's Name", "Relative", 5000.00m, 150.00m)
            };

            mockRepository
                .Setup(r => r.GetAllSchemesAsync(cancellationToken))
                .ReturnsAsync(expectedSchemes);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.GetSchemes();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<SchemesDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedSchemes = Assert.IsAssignableFrom<IEnumerable<SchemesDTO>>(okResult.Value);
            var schemesList = returnedSchemes.ToList();
            Assert.Equal(5, schemesList.Count);
            Assert.Equal(expectedSchemes, schemesList);
            mockRepository.Verify(r => r.GetAllSchemesAsync(cancellationToken), Times.Once);
        }

        /// <summary>
        /// Tests that GetSchemes returns OkObjectResult when called multiple times sequentially.
        /// Input: Multiple sequential calls to GetSchemes.
        /// Expected: Each call returns OkObjectResult with correct schemes and repository is called each time.
        /// </summary>
        [Fact]
        public async Task GetSchemes_MultipleSequentialCalls_ReturnsOkEachTime()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedSchemes = new List<SchemesDTO>
            {
                new SchemesDTO(Guid.NewGuid(), "Test Scheme", "Full", 1000.00m, 50.00m)
            };

            mockRepository
                .Setup(r => r.GetAllSchemesAsync(cancellationToken))
                .ReturnsAsync(expectedSchemes);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result1 = await controller.GetSchemes();
            var result2 = await controller.GetSchemes();
            var result3 = await controller.GetSchemes();

            // Assert
            var actionResult1 = Assert.IsType<ActionResult<IEnumerable<SchemesDTO>>>(result1);
            var okResult1 = Assert.IsType<OkObjectResult>(actionResult1.Result);
            Assert.NotNull(okResult1.Value);

            var actionResult2 = Assert.IsType<ActionResult<IEnumerable<SchemesDTO>>>(result2);
            var okResult2 = Assert.IsType<OkObjectResult>(actionResult2.Result);
            Assert.NotNull(okResult2.Value);

            var actionResult3 = Assert.IsType<ActionResult<IEnumerable<SchemesDTO>>>(result3);
            var okResult3 = Assert.IsType<OkObjectResult>(actionResult3.Result);
            Assert.NotNull(okResult3.Value);

            mockRepository.Verify(r => r.GetAllSchemesAsync(cancellationToken), Times.Exactly(3));
        }

        /// <summary>
        /// Tests that GetSchemes returns OkObjectResult with schemes having Guid.Empty as ID.
        /// Input: Repository returns scheme with Guid.Empty.
        /// Expected: Returns OkObjectResult containing scheme with Guid.Empty preserved.
        /// </summary>
        [Fact]
        public async Task GetSchemes_SchemeWithEmptyGuid_ReturnsOkWithEmptyGuid()
        {
            // Arrange
            var mockRepository = new Mock<ISchemesRepository>();
            var mockLogger = new Mock<ILogger<SchemesController>>();
            var cancellationToken = CancellationToken.None;
            var expectedSchemes = new List<SchemesDTO>
            {
                new SchemesDTO(Guid.Empty, "Scheme With Empty Guid", "Full", 1000.00m, 50.00m)
            };

            mockRepository
                .Setup(r => r.GetAllSchemesAsync(cancellationToken))
                .ReturnsAsync(expectedSchemes);

            var controller = new SchemesController(mockRepository.Object, mockLogger.Object, cancellationToken);

            // Act
            var result = await controller.GetSchemes();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<SchemesDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnedSchemes = Assert.IsAssignableFrom<IEnumerable<SchemesDTO>>(okResult.Value);
            var schemesList = returnedSchemes.ToList();
            Assert.Single(schemesList);
            Assert.Equal(Guid.Empty, schemesList[0].SchemesID);
            mockRepository.Verify(r => r.GetAllSchemesAsync(cancellationToken), Times.Once);
        }
    }
}