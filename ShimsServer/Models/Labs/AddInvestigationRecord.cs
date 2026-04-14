using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.Labs
{
    public record AddInvestigationRecord(
        [StringLength(150, MinimumLength = 3)] string Investigation,
        [StringLength(50, MinimumLength = 3)] string Category,
        [StringLength(20, MinimumLength = 3)] string InvestigationType,
        string[] Levels,
        [StringLength(500, MinimumLength = 2)] string? LabDescription = "");
}
