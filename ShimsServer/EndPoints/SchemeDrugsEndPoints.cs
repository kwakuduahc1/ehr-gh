using Microsoft.EntityFrameworkCore;
using ShimsServer.Context;
using ShimsServer.Models.Schemes;

namespace ShimsServer.EndPoints
{
    public static class SchemeDrugsEndPoints
    {
        extension(IEndpointRouteBuilder app)
        {
            public void MapSchemeDrugsEndPoints()
            {
                var schemeDrugsGroup = app.MapGroup("/schemes/{schemeId:guid}/drugs/").WithTags("Scheme Drugs")
                    //  .RequireAuthorization()
                    .RequireCors("bStudioApps");
                schemeDrugsGroup.MapGet("", GetSchemeDrugs);
                schemeDrugsGroup.MapGet("{id:guid}", GetSchemeDrugById);
                schemeDrugsGroup.MapPost("", AddSchemeDrug);
                schemeDrugsGroup.MapPut("{id:guid:required}", UpdateSchemeDrug);
                schemeDrugsGroup.MapDelete("{id:guid:required}", DeleteSchemeDrug);
                schemeDrugsGroup.MapGet("availability", GetSchemeDrugsAvailability);
            }
        }

        static async Task<IResult> GetSchemeDrugs(ApplicationDbContext db, Guid schemeId) =>
            TypedResults.Ok(await db.SchemeDrugs.Where(x => x.SchemesID == schemeId)
                .Select(x => new SchemeDrugDTO(
                    x.SchemeDrugsID,
                    x.SchemesID,
                    x.DrugsID,
                    x.Schemes!.SchemeName,
                    x.Drugs!.Drug,
                    x.Price,
                    x.DateSet
                )).ToListAsync());

        static async Task<IResult> GetSchemeDrugById(ApplicationDbContext db, Guid schemeId, Guid id, CancellationToken token) =>
            await db.SchemeDrugs.Where(x => x.SchemesID == schemeId && x.SchemeDrugsID == id)
                .Select(x => new SchemeDrugDTO(
                    x.SchemeDrugsID,
                    x.SchemesID,
                    x.DrugsID,
                    x.Schemes!.SchemeName,
                    x.Drugs!.Drug,
                    x.Price,
                    x.DateSet
                )).FirstOrDefaultAsync(token) is SchemeDrugDTO schemeDrug ?
                TypedResults.Ok(schemeDrug) :
                TypedResults.NotFound();

        static async Task<IResult> AddSchemeDrug(ApplicationDbContext db, HttpContext context, Guid schemeId, AddSchemeDrugDto drugDto, CancellationToken token)
        {
            var schemeExists = await db.Schemes.AnyAsync(x => x.SchemesID == schemeId, token);
            if (!schemeExists)
                return TypedResults.NotFound("Scheme not found.");

            var drugExists = await db.Drugs.AnyAsync(x => x.DrugsID == drugDto.DrugsID, token);
            if (!drugExists)
                return TypedResults.NotFound("Drug not found.");

            var schemeDrug = new SchemeDrugs
            {
                SchemeDrugsID = Guid.CreateVersion7(),
                SchemesID = schemeId,
                DrugsID = drugDto.DrugsID,
                Price = drugDto.Price,
                DateSet = DateTime.UtcNow,
                UserName = context.User.Identity?.Name ?? "Unknown"
            };

            db.SchemeDrugs.Add(schemeDrug);
            await db.SaveChangesAsync(token);

            return TypedResults.Ok(schemeDrug.SchemeDrugsID);
        }

        static async Task<IResult> UpdateSchemeDrug(ApplicationDbContext db, Guid schemeId, Guid id, UpdateSchemeDrugDto drugDto, CancellationToken token)
        {
            var schemeDrug = await db.SchemeDrugs.FirstOrDefaultAsync(x => x.SchemesID == schemeId && x.SchemeDrugsID == id, token);
            if (schemeDrug == null)
                return TypedResults.NotFound();

            schemeDrug.Price = drugDto.Price;
            schemeDrug.DateSet = DateTime.UtcNow;

            db.Entry(schemeDrug).State = EntityState.Modified;
            await db.SaveChangesAsync(token);

            return TypedResults.Ok();
        }

        static async Task<IResult> DeleteSchemeDrug(ApplicationDbContext db, Guid schemeId, Guid id, CancellationToken token)
        {
            var schemeDrug = await db.SchemeDrugs.FirstOrDefaultAsync(x => x.SchemesID == schemeId && x.SchemeDrugsID == id, token);
            if (schemeDrug == null)
                return TypedResults.NotFound();

            db.SchemeDrugs.Remove(schemeDrug);
            await db.SaveChangesAsync(token);

            return TypedResults.Ok();
        }

        static async Task<IResult> GetSchemeDrugsAvailability(ApplicationDbContext db, Guid schemeId) =>
            TypedResults.Ok(await db.SchemeDrugs.Where(x => x.SchemesID == schemeId)
                .Select(x => new SchemeDrugAvailabilityDto(
                    x.SchemeDrugsID,
                    x.Schemes!.SchemeName,
                    x.Drugs!.Drug,
                    x.Price,
                    true // All scheme drugs are considered available
                )).ToListAsync());
    }
}
