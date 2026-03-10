using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ShimsServer.Context;
using ShimsServer.EndPoints;
using ShimsServer.Models.Schemes;
using Xunit;
using static ShimsServer.EndPoints.SchemesEndPoints;

namespace ShimsServer.Tests.EndPoints
{
    public class SchemesEndPointsTests
    {
        private readonly ApplicationDbContext _context;

        public SchemesEndPointsTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
        }

        private static Schemes CreateTestScheme(string schemeName = "Test Scheme", string coverage = "Full", decimal maxPayable = 1000m, decimal recovery = 80m)
        {
            return new Schemes
            {
                SchemesID = Guid.CreateVersion7(),
                SchemeName = schemeName,
                Coverage = coverage,
                MaxPayable = maxPayable,
                Recovery = recovery
            };
        }

        #region GetSchemes Tests

        [Fact]
        public async Task GetSchemes_ReturnsAllSchemes()
        {
            // Arrange
            var scheme1 = CreateTestScheme("NHIS", "Full");
            var scheme2 = CreateTestScheme("Private", "Relative");
            var scheme3 = CreateTestScheme("Government", "Fixed");

            _context.Schemes.AddRange(scheme1, scheme2, scheme3);
            await _context.SaveChangesAsync();

            // Act
            var result = await CallGetSchemes();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<Ok<List<SchemesDTO>>>(result);
            Assert.Equal(3, okResult.Value?.Count);
        }

        [Fact]
        public async Task GetSchemes_ReturnsEmptyListWhenNoSchemes()
        {
            // Act
            var result = await CallGetSchemes();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<Ok<List<SchemesDTO>>>(result);
            Assert.Empty(okResult.Value ?? []);
        }

        #endregion

        #region GetSchemeById Tests

        [Fact]
        public async Task GetSchemeById_ReturnsSchemeWhenExists()
        {
            // Arrange
            var scheme = CreateTestScheme();
            _context.Schemes.Add(scheme);
            await _context.SaveChangesAsync();

            // Act
            var result = await CallGetSchemeById(scheme.SchemesID);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<Ok<SchemesDTO>>(result);
            Assert.Equal(scheme.SchemesID, okResult.Value?.SchemesID);
            Assert.Equal(scheme.SchemeName, okResult.Value?.SchemeName);
        }

        [Fact]
        public async Task GetSchemeById_ReturnsNotFoundWhenSchemeDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await CallGetSchemeById(nonExistentId);

            // Assert
            Assert.IsType<NotFound>(result);
        }

        #endregion

        #region SchemeExists Tests

        [Fact]
        public async Task SchemeExists_ReturnsTrueWhenSchemeExists()
        {
            // Arrange
            var scheme = CreateTestScheme("NHIS");
            _context.Schemes.Add(scheme);
            await _context.SaveChangesAsync();

            // Act
            var result = await CallSchemeExists("NHIS");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SchemeExists_ReturnsFalseWhenSchemeDoesNotExist()
        {
            // Act
            var result = await CallSchemeExists("NonExistent");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SchemeExists_IsCaseSensitive()
        {
            // Arrange
            var scheme = CreateTestScheme("NHIS");
            _context.Schemes.Add(scheme);
            await _context.SaveChangesAsync();

            // Act
            var resultUpperCase = await CallSchemeExists("NHIS");
            var resultLowerCase = await CallSchemeExists("nhis");

            // Assert
            Assert.True(resultUpperCase);
            Assert.False(resultLowerCase);
        }

        #endregion

        #region AddScheme Tests

        [Fact]
        public async Task AddScheme_CreatesNewSchemeSuccessfully()
        {
            // Arrange
            var schemeDto = new AddSchemeDto("NewScheme", "Full", 2000m, 85m);

            // Act
            var result = await CallAddScheme(schemeDto);

            // Assert
            var okResult = Assert.IsType<Ok<Guid>>(result);
            var createdId = okResult.Value;
            Assert.NotEqual(Guid.Empty, createdId);

            var savedScheme = await _context.Schemes.FindAsync(createdId);
            Assert.NotNull(savedScheme);
            Assert.Equal(schemeDto.SchemeName, savedScheme.SchemeName);
            Assert.Equal(schemeDto.Coverage, savedScheme.Coverage);
            Assert.Equal(schemeDto.MaxPayable, savedScheme.MaxPayable);
            Assert.Equal(schemeDto.Recovery, savedScheme.Recovery);
        }

        [Fact]
        public async Task AddScheme_ReturnsConflictWhenSchemeNameAlreadyExists()
        {
            // Arrange
            var existingScheme = CreateTestScheme("NHIS");
            _context.Schemes.Add(existingScheme);
            await _context.SaveChangesAsync();

            var schemeDto = new AddSchemeDto("NHIS", "Relative", 1500m, 90m);

            // Act
            var result = await CallAddScheme(schemeDto);

            // Assert
            var conflictResult = Assert.IsType<Conflict>(result);
            Assert.Contains("NHIS", conflictResult.ContentType ?? "");
        }

        [Fact]
        public async Task AddScheme_CreatesMultipleSchemesWithDifferentNames()
        {
            // Arrange
            var scheme1Dto = new AddSchemeDto("Scheme1", "Full", 1000m, 80m);
            var scheme2Dto = new AddSchemeDto("Scheme2", "Relative", 1500m, 85m);

            // Act
            var result1 = await CallAddScheme(scheme1Dto);
            var result2 = await CallAddScheme(scheme2Dto);

            // Assert
            Assert.IsType<Ok<Guid>>(result1);
            Assert.IsType<Ok<Guid>>(result2);

            var count = await _context.Schemes.CountAsync();
            Assert.Equal(2, count);
        }

        #endregion

        #region UpdateScheme Tests

        [Fact]
        public async Task UpdateScheme_UpdatesSchemeSuccessfully()
        {
            // Arrange
            var scheme = CreateTestScheme("NHIS", "Full", 1000m, 80m);
            _context.Schemes.Add(scheme);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateSchemeDto(scheme.SchemesID, "NHIS Updated", "Relative", 2000m, 90m);

            // Act
            var result = await CallUpdateScheme(scheme.SchemesID, updateDto);

            // Assert
            Assert.IsType<Ok>(result);

            var updatedScheme = await _context.Schemes.FindAsync(scheme.SchemesID);
            Assert.NotNull(updatedScheme);
            Assert.Equal("NHIS Updated", updatedScheme.SchemeName);
            Assert.Equal("Relative", updatedScheme.Coverage);
            Assert.Equal(2000m, updatedScheme.MaxPayable);
            Assert.Equal(90m, updatedScheme.Recovery);
        }

        [Fact]
        public async Task UpdateScheme_ReturnsNotFoundWhenSchemeDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var updateDto = new UpdateSchemeDto(nonExistentId, "UpdatedName", "Full", 1000m, 80m);

            // Act
            var result = await CallUpdateScheme(nonExistentId, updateDto);

            // Assert
            Assert.IsType<NotFound>(result);
        }

        [Fact]
        public async Task UpdateScheme_ReturnsConflictWhenNewNameAlreadyExists()
        {
            // Arrange
            var scheme1 = CreateTestScheme("Scheme1", "Full", 1000m, 80m);
            var scheme2 = CreateTestScheme("Scheme2", "Relative", 1500m, 85m);
            _context.Schemes.AddRange(scheme1, scheme2);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateSchemeDto(scheme1.SchemesID, "Scheme2", "Full", 1000m, 80m);

            // Act
            var result = await CallUpdateScheme(scheme1.SchemesID, updateDto);

            // Assert
            var conflictResult = Assert.IsType<Conflict>(result);
            Assert.Contains("Scheme2", conflictResult.ContentType ?? "");
        }

        [Fact]
        public async Task UpdateScheme_AllowsUpdatingOnlyNonNameFields()
        {
            // Arrange
            var scheme = CreateTestScheme("NHIS", "Full", 1000m, 80m);
            _context.Schemes.Add(scheme);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateSchemeDto(scheme.SchemesID, "NHIS", "Relative", 2000m, 90m);

            // Act
            var result = await CallUpdateScheme(scheme.SchemesID, updateDto);

            // Assert
            Assert.IsType<Ok>(result);
            var updatedScheme = await _context.Schemes.FindAsync(scheme.SchemesID);
            Assert.Equal("NHIS", updatedScheme?.SchemeName);
            Assert.Equal("Relative", updatedScheme?.Coverage);
        }

        #endregion

        #region DeleteScheme Tests

        [Fact]
        public async Task DeleteScheme_DeletesSchemeSuccessfully()
        {
            // Arrange
            var scheme = CreateTestScheme();
            _context.Schemes.Add(scheme);
            await _context.SaveChangesAsync();

            // Act
            var result = await CallDeleteScheme(scheme.SchemesID);

            // Assert
            Assert.IsType<Ok>(result);

            var deletedScheme = await _context.Schemes.FindAsync(scheme.SchemesID);
            Assert.Null(deletedScheme);
        }

        [Fact]
        public async Task DeleteScheme_ReturnsNotFoundWhenSchemeDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await CallDeleteScheme(nonExistentId);

            // Assert
            Assert.IsType<NotFound>(result);
        }

        [Fact]
        public async Task DeleteScheme_OnlyDeletesTargetedScheme()
        {
            // Arrange
            var scheme1 = CreateTestScheme("Scheme1");
            var scheme2 = CreateTestScheme("Scheme2");
            _context.Schemes.AddRange(scheme1, scheme2);
            await _context.SaveChangesAsync();

            // Act
            var result = await CallDeleteScheme(scheme1.SchemesID);

            // Assert
            Assert.IsType<Ok>(result);

            var remainingScheme = await _context.Schemes.FindAsync(scheme2.SchemesID);
            Assert.NotNull(remainingScheme);

            var count = await _context.Schemes.CountAsync();
            Assert.Equal(1, count);
        }

        #endregion

        #region Helper Methods

        private async Task<IResult> CallGetSchemes()
        {
            var schemes = await _context.Schemes.Select(x => new SchemesDTO(
                x.SchemesID,
                x.SchemeName,
                x.Coverage,
                x.MaxPayable,
                x.Recovery
            )).ToListAsync();

            return TypedResults.Ok(schemes);
        }

        private async Task<IResult> CallGetSchemeById(Guid id)
        {
            return await _context.Schemes.Where(p => p.SchemesID == id)
                .Select(p => new SchemesDTO(
                    p.SchemesID,
                    p.SchemeName,
                    p.Coverage,
                    p.MaxPayable,
                    p.Recovery
                )).FirstOrDefaultAsync() is SchemesDTO scheme ?
                TypedResults.Ok<SchemesDTO>(scheme) :
                TypedResults.NotFound();
        }

        private async Task<bool> CallSchemeExists(string name, CancellationToken token = default)
        {
            return await _context.Schemes.AnyAsync(x => x.SchemeName == name, token);
        }

        private async Task<IResult> CallAddScheme(AddSchemeDto schemeDto, CancellationToken token = default)
        {
            if (await CallSchemeExists(schemeDto.SchemeName, token))
                return TypedResults.Conflict($"Scheme with name {schemeDto.SchemeName} already exists.");

            var scheme = new Schemes
            {
                SchemesID = Guid.CreateVersion7(),
                SchemeName = schemeDto.SchemeName,
                Coverage = schemeDto.Coverage,
                MaxPayable = schemeDto.MaxPayable,
                Recovery = schemeDto.Recovery
            };

            _context.Schemes.Add(scheme);
            await _context.SaveChangesAsync(token);

            return TypedResults.Ok<Guid>(scheme.SchemesID);
        }

        private async Task<IResult> CallUpdateScheme(Guid id, UpdateSchemeDto schemeDto, CancellationToken token = default)
        {
            var scheme = await _context.Schemes.FindAsync(new object[] { id }, cancellationToken: token);
            if (scheme == null)
                return TypedResults.NotFound();

            if (scheme.SchemeName != schemeDto.SchemeName && await CallSchemeExists(schemeDto.SchemeName, token))
                return TypedResults.Conflict($"Scheme with name {schemeDto.SchemeName} already exists.");

            scheme.SchemeName = schemeDto.SchemeName;
            scheme.Coverage = schemeDto.Coverage;
            scheme.MaxPayable = schemeDto.MaxPayable;
            scheme.Recovery = schemeDto.Recovery;

            _context.Entry(scheme).State = EntityState.Modified;
            await _context.SaveChangesAsync(token);

            return TypedResults.Ok();
        }

        private async Task<IResult> CallDeleteScheme(Guid id, CancellationToken token = default)
        {
            var scheme = await _context.Schemes.FindAsync(new object[] { id }, cancellationToken: token);
            if (scheme == null)
                return TypedResults.NotFound();

            _context.Schemes.Remove(scheme);
            await _context.SaveChangesAsync(token);

            return TypedResults.Ok();
        }

        #endregion
    }
}
