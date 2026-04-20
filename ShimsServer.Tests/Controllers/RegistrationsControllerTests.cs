using System.Security.Claims;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Npgsql;
using ShimsServer.Controllers.Records;
using ShimsServer.Repositories;
using Xunit;

namespace ShimsServer.Tests.Controllers
{
    public class RegistrationsControllerTests
    {
        private readonly Mock<IRegistrationRepository> _mockRepository;
        private readonly Mock<ILogger<RegistrationsController>> _mockLogger;
        private readonly RegistrationsController _controller;
        private readonly CancellationToken _cancellationToken;

        public RegistrationsControllerTests()
        {
            _mockRepository = new Mock<IRegistrationRepository>();
            _mockLogger = new Mock<ILogger<RegistrationsController>>();
            _cancellationToken = CancellationToken.None;
            _controller = new RegistrationsController(_mockRepository.Object, _mockLogger.Object, _cancellationToken);

            // Setup user context
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        #region RegisterPatient Tests

        [Fact]
        public async Task RegisterPatient_WithValidData_ReturnsOkWithPatientId()
        {
            // Arrange
            var dto = new AddPatientDto(
                Surname: "Doe",
                OtherNames: "John",
                DateOfBirth: new DateTime(1990, 1, 1),
                GhanaCard: "GHA-0123456789-01",
                Sex: "Male",
                SchemesID: Guid.NewGuid(),
                CardID: "CARD123456789",
                ExpiryDate: new DateTime(2026, 12, 31),
                PhoneNumber: "0541234567"
            );

            _mockRepository
                .Setup(r => r.AddPatientAsync(
                    It.IsAny<AddPatientDto>(),
                    It.IsAny<(Guid, Guid, string)>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RegisterPatient(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            _mockRepository.Verify(
                r => r.AddPatientAsync(
                    It.IsAny<AddPatientDto>(),
                    It.IsAny<(Guid, Guid, string)>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task RegisterPatient_WithValidData_PassesCorrectUserName()
        {
            // Arrange
            var dto = new AddPatientDto(
                Surname: "Smith",
                OtherNames: "Jane",
                DateOfBirth: new DateTime(1995, 5, 15),
                GhanaCard: "GHA-9876543210-05",
                Sex: "Female",
                SchemesID: Guid.NewGuid(),
                CardID: "CARD987654321",
                ExpiryDate: new DateTime(2027, 6, 30),
                PhoneNumber: "0559876543"
            );

            (Guid, Guid, string) capturedIds = default;

            _mockRepository
                .Setup(r => r.AddPatientAsync(
                    It.IsAny<AddPatientDto>(),
                    It.IsAny<(Guid, Guid, string)>(),
                    It.IsAny<CancellationToken>()))
                .Callback<AddPatientDto, (Guid, Guid, string), CancellationToken>((d, ids, ct) =>
                {
                    capturedIds = ids;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _controller.RegisterPatient(dto);

            // Assert
            Assert.Equal("testuser", capturedIds.Item3);
        }

        [Fact]
        public async Task RegisterPatient_WhenDatabaseError_ReturnsBadRequest()
        {
            // Arrange
            var dto = new AddPatientDto(
                Surname: "Doe",
                OtherNames: "John",
                DateOfBirth: new DateTime(1990, 1, 1),
                GhanaCard: "GHA-0123456789-01",
                Sex: "Male",
                SchemesID: Guid.NewGuid(),
                CardID: "CARD123456789",
                ExpiryDate: new DateTime(2026, 12, 31),
                PhoneNumber: "0541234567"
            );

            var postgresException = new PostgresException("Database error", "severity", "invariantSeverity", "23505");

            _mockRepository
                .Setup(r => r.AddPatientAsync(
                    It.IsAny<AddPatientDto>(),
                    It.IsAny<(Guid, Guid, string)>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(postgresException);

            // Act
            var result = await _controller.RegisterPatient(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            _mockLogger.Verify(
                static l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task RegisterPatient_WhenGenericError_ReturnsBadRequest()
        {
            // Arrange
            var dto = new AddPatientDto(
                Surname: "Doe",
                OtherNames: "John",
                DateOfBirth: new DateTime(1990, 1, 1),
                GhanaCard: "GHA-0123456789-01",
                Sex: "Male",
                SchemesID: Guid.NewGuid(),
                CardID: "CARD123456789",
                ExpiryDate: new DateTime(2026, 12, 31),
                PhoneNumber: "0541234567"
            );

            _mockRepository
                .Setup(r => r.AddPatientAsync(
                    It.IsAny<AddPatientDto>(),
                    It.IsAny<(Guid, Guid, string)>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Unexpected error"));

            // Act
            var result = await _controller.RegisterPatient(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        #endregion

        #region GetPatients Tests

        [Fact]
        public async Task GetPatients_ReturnsOkWithPatients()
        {
            // Arrange
            var patients = new List<ListPatientsDto>
            {
                new ListPatientsDto(
                    PatientID: Guid.NewGuid(),
                    SchemesID: Guid.NewGuid(),
                    Age: 30,
                    Gender: "Male",
                    FullName: "John Doe",
                    Scheme: "NHIS",
                    HospitalID: "HOSP-2025-01-000001",
                    CardID: "CARD123456789",
                    ExpiryDate: new DateTime(2026, 12, 31),
                    AttendanceDate: DateTime.Now
                ),
                new ListPatientsDto(
                    PatientID: Guid.NewGuid(),
                    SchemesID: Guid.NewGuid(),
                    Age: 25,
                    Gender: "Female",
                    FullName: "Jane Smith",
                    Scheme: "NHIS",
                    HospitalID: "HOSP-2025-01-000002",
                    CardID: "CARD987654321",
                    ExpiryDate: new DateTime(2027, 6, 30),
                    AttendanceDate: DateTime.Now.AddDays(-1)
                )
            };

            _mockRepository
                .Setup(r => r.GetPatientsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(patients);

            // Act
            var result = await _controller.GetPatients();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPatients = Assert.IsAssignableFrom<IEnumerable<ListPatientsDto>>(okResult.Value);
            Assert.Equal(2, returnedPatients.Count());
        }

        [Fact]
        public async Task GetPatients_WhenEmpty_ReturnsOkWithEmptyList()
        {
            // Arrange
            var emptyList = new List<ListPatientsDto>();

            _mockRepository
                .Setup(r => r.GetPatientsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetPatients();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPatients = Assert.IsAssignableFrom<IEnumerable<ListPatientsDto>>(okResult.Value);
            Assert.Empty(returnedPatients);
        }

        #endregion

        #region GetPatientById Tests

        [Fact]
        public async Task GetPatientById_WithValidId_ReturnsOkWithPatient()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patient = new ListPatientsDto(
                PatientID: patientId,
                SchemesID: Guid.NewGuid(),
                Age: 30,
                Gender: "Male",
                FullName: "John Doe",
                Scheme: "NHIS",
                HospitalID: "HOSP-2025-01-000001",
                CardID: "CARD123456789",
                ExpiryDate: new DateTime(2026, 12, 31),
                AttendanceDate: DateTime.Now
            );

            _mockRepository
                .Setup(r => r.GetPatientByIdAsync(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(patient);

            // Act
            var result = await _controller.GetPatientById(patientId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPatient = Assert.IsType<ListPatientsDto>(okResult.Value);
            Assert.Equal(patientId, returnedPatient.PatientID);
            Assert.Equal("John Doe", returnedPatient.FullName);
        }

        [Fact]
        public async Task GetPatientById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            _mockRepository
                .Setup(r => r.GetPatientByIdAsync(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((ListPatientsDto?)null);

            // Act
            var result = await _controller.GetPatientById(patientId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        #endregion

        #region SearchPatients Tests

        [Fact]
        public async Task SearchPatients_WithValidSearchTerm_ReturnsOkWithMatches()
        {
            // Arrange
            var searchTerm = "John";
            var matches = new List<ListPatientsDto>
            {
                new ListPatientsDto(
                    PatientID: Guid.NewGuid(),
                    SchemesID: Guid.NewGuid(),
                    Age: 30,
                    Gender: "Male",
                    FullName: "John Doe",
                    Scheme: "NHIS",
                    HospitalID: "HOSP-2025-01-000001",
                    CardID: "CARD123456789",
                    ExpiryDate: new DateTime(2026, 12, 31),
                    AttendanceDate: DateTime.Now
                )
            };

            _mockRepository
                .Setup(r => r.SearchPatientsAsync(searchTerm.Trim(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(matches);

            // Act
            var result = await _controller.SearchPatients(searchTerm);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPatients = Assert.IsAssignableFrom<IEnumerable<ListPatientsDto>>(okResult.Value);
            Assert.Single(returnedPatients);
        }

        [Fact]
        public async Task SearchPatients_WithNoMatches_ReturnsOkWithEmptyList()
        {
            // Arrange
            var searchTerm = "NonExistent";
            var emptyList = new List<ListPatientsDto>();

            _mockRepository
                .Setup(r => r.SearchPatientsAsync(searchTerm.Trim(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.SearchPatients(searchTerm);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPatients = Assert.IsAssignableFrom<IEnumerable<ListPatientsDto>>(okResult.Value);
            Assert.Empty(returnedPatients);
        }

        [Fact]
        public async Task SearchPatients_WithEmptySearchTerm_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.SearchPatients("");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            _mockRepository.Verify(
                r => r.SearchPatientsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task SearchPatients_WithWhitespaceSearchTerm_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.SearchPatients("   ");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task SearchPatients_TrimsSearchTerm()
        {
            // Arrange
            var searchTerm = "  John  ";
            var matches = new List<ListPatientsDto>();

            _mockRepository
                .Setup(r => r.SearchPatientsAsync("John", It.IsAny<CancellationToken>()))
                .ReturnsAsync(matches);

            // Act
            await _controller.SearchPatients(searchTerm);

            // Assert
            _mockRepository.Verify(
                r => r.SearchPatientsAsync("John", It.IsAny<CancellationToken>()),
                Times.Once);
        }

        #endregion

        #region UpdatePatient Tests

        [Fact]
        public async Task UpdatePatient_WithValidData_ReturnsOk()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var dto = new EditPatientDto(
                PatientID: patientId,
                HospitalID: "HOSP-2025-01-000001",
                GhanaCard: "GHA-1111111111-02",
                Surname: "UpdatedSurname",
                OtherNames: "UpdatedNames",
                DateOfBirth: new DateTime(1992, 3, 10),
                Sex: "Male",
                PhoneNumber: "0501111111"
            );

            _mockRepository
                .Setup(r => r.PatientExists(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockRepository
                .Setup(r => r.EditPatientAsync(dto, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.UpdatePatient(patientId, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            _mockRepository.Verify(
                r => r.EditPatientAsync(dto, It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task UpdatePatient_WithMismatchedIds_ReturnsBadRequest()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var differentId = Guid.NewGuid();
            var dto = new EditPatientDto(
                PatientID: differentId,
                HospitalID: "HOSP-2025-01-000001",
                GhanaCard: null,
                Surname: "Surname",
                OtherNames: "Names",
                DateOfBirth: new DateTime(1990, 1, 1),
                Sex: "Female",
                PhoneNumber: "0500000000"
            );

            // Act
            var result = await _controller.UpdatePatient(patientId, dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            _mockRepository.Verify(
                r => r.PatientExists(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdatePatient_WhenPatientNotFound_ReturnsNotFound()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var dto = new EditPatientDto(
                PatientID: patientId,
                HospitalID: "HOSP-2025-01-000001",
                GhanaCard: null,
                Surname: "Surname",
                OtherNames: "Names",
                DateOfBirth: new DateTime(1990, 1, 1),
                Sex: "Female",
                PhoneNumber: "0500000000"
            );

            _mockRepository
                .Setup(r => r.PatientExists(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdatePatient(patientId, dto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task UpdatePatient_WhenDatabaseError_ReturnsBadRequest()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var dto = new EditPatientDto(
                PatientID: patientId,
                HospitalID: "HOSP-2025-01-000001",
                GhanaCard: null,
                Surname: "Surname",
                OtherNames: "Names",
                DateOfBirth: new DateTime(1990, 1, 1),
                Sex: "Female",
                PhoneNumber: "0500000000"
            );

            var postgresException = new PostgresException("Database error", "severity", "invariantSeverity", "23505");

            _mockRepository
                .Setup(r => r.PatientExists(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockRepository
                .Setup(r => r.EditPatientAsync(dto, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(postgresException);

            // Act
            var result = await _controller.UpdatePatient(patientId, dto);

            // Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        #endregion

        #region DeletePatient Tests

        [Fact]
        public async Task DeletePatient_WithValidId_ReturnsOk()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            _mockRepository
                .Setup(r => r.PatientExists(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockRepository
                .Setup(r => r.DeletePatientAsync(patientId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePatient(patientId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            _mockRepository.Verify(
                r => r.DeletePatientAsync(patientId, It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task DeletePatient_WhenPatientNotFound_ReturnsNotFound()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            _mockRepository
                .Setup(r => r.PatientExists(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeletePatient(patientId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            _mockRepository.Verify(
                r => r.DeletePatientAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task DeletePatient_WhenDatabaseError_ReturnsBadRequest()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var postgresException = new PostgresException("Database error", "severity", "invariantSeverity", "23505");

            _mockRepository
                .Setup(r => r.PatientExists(patientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            _mockRepository
                .Setup(r => r.DeletePatientAsync(patientId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(postgresException);

            // Act
            var result = await _controller.DeletePatient(patientId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }

        #endregion
    }
}
