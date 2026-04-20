using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.Schemes
{
    public record AddSchemeInvestigationDto(
        Guid InvestigationsID,
        Guid SchemesID,
        [StringLength(20)] string GDRG,
        [Range(0.5D, double.MaxValue)] decimal Price,
        [StringLength(500)] string? Narration = null);
}
