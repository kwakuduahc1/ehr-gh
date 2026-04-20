namespace ShimsServer.Models.Schemes
{
    public record SchemeInvestigationDTO(
        Guid SchemeInvestigationsID,
        Guid InvestigationsID,
        Guid SchemesID,
        string GDRG,
        decimal Price,
        bool IsActive,
        string Investigation,
        string? Narration);
}
