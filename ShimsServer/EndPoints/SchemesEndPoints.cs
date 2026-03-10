using Microsoft.EntityFrameworkCore;
using ShimsServer.Context;
using ShimsServer.Models.Schemes;

namespace ShimsServer.EndPoints
{
    public static class SchemesEndPoints
    {

        extension(IEndpointRouteBuilder app)
        {
            public void MapSchemesEndPoints()
            {
                var schemesGroup = app.MapGroup("/schemes/").WithTags("Schemes")
                    .RequireAuthorization()
                    .RequireCors("bStudioApps");
                schemesGroup.MapGet("", GetSchemes);
                schemesGroup.MapGet("{id:guid}", GetSchemeById);
                schemesGroup.MapPost("", AddScheme);
                schemesGroup.MapGet("Check", SchemeExists);
                schemesGroup.MapPut("{id:guid:required}", UpdateScheme);
                schemesGroup.MapDelete("{id:guid:required}", DeleteScheme);
            }
        }

        static async Task<IResult> GetSchemes(ApplicationDbContext db) =>
             TypedResults.Ok(await db.Schemes.Select(x => new SchemesDTO
             (
                 x.SchemesID,
                 x.SchemeName,
                 x.Coverage,
                 x.MaxPayable,
                 x.Recovery
             )).ToListAsync());

        static async Task<IResult> GetSchemeById(ApplicationDbContext db, Guid id, CancellationToken token) => await db.Schemes.Where(p => p.SchemesID == id)
            .Select(p => new SchemesDTO
            (
                p.SchemesID,
                p.SchemeName,
                p.Coverage,
                p.MaxPayable,
                p.Recovery
            )).FirstOrDefaultAsync(token) is SchemesDTO scheme ?
             TypedResults.Ok<SchemesDTO>(scheme) :
                 TypedResults.NotFound();

        static async Task<bool> SchemeExists(ApplicationDbContext db, string name, CancellationToken token) => await db.Schemes.AnyAsync(x => x.SchemeName == name, token);

        static async Task<IResult> AddScheme(ApplicationDbContext db, AddSchemeDto schemeDto, CancellationToken token)
        {
            if (await SchemeExists(db, schemeDto.SchemeName, token))
                return TypedResults.Conflict($"Scheme with name {schemeDto.SchemeName} already exists.");

            var scheme = new Schemes
            {
                SchemesID = Guid.CreateVersion7(),
                SchemeName = schemeDto.SchemeName,
                Coverage = schemeDto.Coverage,
                MaxPayable = schemeDto.MaxPayable,
                Recovery = schemeDto.Recovery
            };

            db.Schemes.Add(scheme);
            await db.SaveChangesAsync(token);

            return TypedResults.Ok<Guid>(scheme.SchemesID);
        }

        static async Task<IResult> UpdateScheme(ApplicationDbContext db, Guid id, UpdateSchemeDto schemeDto, CancellationToken token)
        {
            var scheme = await db.Schemes.FindAsync(id, token);
            if (scheme == null)
                return TypedResults.NotFound();
            if (scheme.SchemeName != schemeDto.SchemeName && await SchemeExists(db, schemeDto.SchemeName, token))
                return TypedResults.Conflict($"Scheme with name {schemeDto.SchemeName} already exists.");
            scheme.SchemeName = schemeDto.SchemeName;
            scheme.Coverage = schemeDto.Coverage;
            scheme.MaxPayable = schemeDto.MaxPayable;
            scheme.Recovery = schemeDto.Recovery;
            db.Entry(scheme).State = EntityState.Modified;
            await db.SaveChangesAsync(token);
            return TypedResults.Ok();
        }

        static async Task<IResult> DeleteScheme(ApplicationDbContext db, Guid id, CancellationToken token)
        {
            var scheme = await db.Schemes.FindAsync(id, token);
            if (scheme == null)
                return TypedResults.NotFound();
            db.Schemes.Remove(scheme);
            await db.SaveChangesAsync(token);
            return TypedResults.Ok();
        }

        public record SchemesDTO(Guid SchemesID, string SchemeName, string Coverage, decimal MaxPayable, decimal Recovery);

        public record AddSchemeDto(string SchemeName, string Coverage, decimal MaxPayable, decimal Recovery);

        public record UpdateSchemeDto(Guid ID, string SchemeName, string Coverage, decimal MaxPayable, decimal Recovery);
    }
}
