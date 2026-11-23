using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.DTOs
{
    public record InvestigationsDto(
        Guid InvestigationsID,
        string Investigation,
        string Category,
        string InvestigationType,
        string[] Levels,
        string? LabDescription);

    public record AddInvestigationDto(
        [Required, StringLength(150, MinimumLength = 3)] string Investigation,
        [Required, StringLength(50, MinimumLength = 3)] string Category,
        [Required, StringLength(20, MinimumLength = 3)] string InvestigationType,
        [Required, MinLength(1)] string[] Levels,
        [StringLength(500, MinimumLength = 2)] string? LabDescription);

    public record UpdateInvestigationDto(
        Guid InvestigationsID,
        [Required, StringLength(150, MinimumLength = 3)] string Investigation,
        [Required, StringLength(50, MinimumLength = 3)] string Category,
        [Required, StringLength(20, MinimumLength = 3)] string InvestigationType,
        [Required, MinLength(1)] string[] Levels,
        [StringLength(500, MinimumLength = 2)] string? LabDescription);
}
