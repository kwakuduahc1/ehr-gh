//using Microsoft.EntityFrameworkCore;
//using ShimsServer.Context;
//using ShimsServer.Models.Schemes;

//namespace ShimsServer.EndPoints
//{
//    public static class SchemeLabsEndPoints
//    {
//        extension(IEndpointRouteBuilder app)
//        {
//            public void MapSchemeLabsEndPoints()
//            {
//                var schemeLabsGroup = app.MapGroup("/schemes/{schemeId:guid}/labs/").WithTags("Scheme Labs")
//                    //  .RequireAuthorization()
//                    .RequireCors("bStudioApps");
//                schemeLabsGroup.MapGet("", GetSchemeLabs);
//                schemeLabsGroup.MapGet("{id:guid}", GetSchemeLabById);
//                schemeLabsGroup.MapPost("", AddSchemeLab);
//                schemeLabsGroup.MapPut("{id:guid:required}", UpdateSchemeLab);
//                schemeLabsGroup.MapDelete("{id:guid:required}", DeleteSchemeLab);
//                schemeLabsGroup.MapGet("availability", GetSchemeLabsAvailability);
//            }
//        }

//        static async Task<IResult> GetSchemeLabs(ApplicationDbContext db, Guid schemeId) =>
//            TypedResults.Ok(await db.SchemeLabs.Where(x => x.SchemesID == schemeId)
//                .Select(x => new SchemeLabDTO(
//                    x.SchemeLabsID,
//                    x.SchemesID,
//                    x.LabsGroupID,
//                    x.Schemes!.SchemeName,
//                    x.Investigations!.Investigation,
//                    x.Price,
//                    x.DateSet
//                )).ToListAsync());

//        static async Task<IResult> GetSchemeLabById(ApplicationDbContext db, Guid schemeId, Guid id, CancellationToken token) =>
//            await db.SchemeLabs.Where(x => x.SchemesID == schemeId && x.SchemeLabsID == id)
//                .Select(x => new SchemeLabDTO(
//                    x.SchemeLabsID,
//                    x.SchemesID,
//                    x.LabsGroupID,
//                    x.Schemes!.SchemeName,
//                    x.Investigations!.Investigation,
//                    x.Price,
//                    x.DateSet
//                )).FirstOrDefaultAsync(token) is SchemeLabDTO schemeLab ?
//                TypedResults.Ok(schemeLab) :
//                TypedResults.NotFound();

//        static async Task<IResult> AddSchemeLab(ApplicationDbContext db, HttpContext context, Guid schemeId, AddSchemeLabDto labDto, CancellationToken token)
//        {
//            var schemeExists = await db.Schemes.AnyAsync(x => x.SchemesID == schemeId, token);
//            if (!schemeExists)
//                return TypedResults.NotFound("Scheme not found.");

//            var labExists = await db.Investigations.AnyAsync(x => x.InvestigationsID == labDto.LabsGroupID, token);
//            if (!labExists)
//                return TypedResults.NotFound("Lab group not found.");

//            var schemeLab = new SchemeInvestigations
//            {
//                SchemeLabsID = Guid.CreateVersion7(),
//                SchemesID = schemeId,
//                LabsGroupID = labDto.LabsGroupID,
//                Price = labDto.Price,
//                DateSet = DateTime.UtcNow,
//                UserName = context.User.Identity?.Name ?? "Unknown"
//            };

//            db.SchemeLabs.Add(schemeLab);
//            await db.SaveChangesAsync(token);

//            return TypedResults.Ok(schemeLab.SchemeLabsID);
//        }

//        static async Task<IResult> UpdateSchemeLab(ApplicationDbContext db, Guid schemeId, Guid id, UpdateSchemeLabDto labDto, CancellationToken token)
//        {
//            var schemeLab = await db.SchemeLabs.FirstOrDefaultAsync(x => x.SchemesID == schemeId && x.SchemeLabsID == id, token);
//            if (schemeLab == null)
//                return TypedResults.NotFound();

//            schemeLab.Price = labDto.Price;
//            schemeLab.DateSet = DateTime.UtcNow;

//            db.Entry(schemeLab).State = EntityState.Modified;
//            await db.SaveChangesAsync(token);

//            return TypedResults.Ok();
//        }

//        static async Task<IResult> DeleteSchemeLab(ApplicationDbContext db, Guid schemeId, Guid id, CancellationToken token)
//        {
//            var schemeLab = await db.SchemeLabs.FirstOrDefaultAsync(x => x.SchemesID == schemeId && x.SchemeLabsID == id, token);
//            if (schemeLab == null)
//                return TypedResults.NotFound();

//            db.SchemeLabs.Remove(schemeLab);
//            await db.SaveChangesAsync(token);

//            return TypedResults.Ok();
//        }

//        static async Task<IResult> GetSchemeLabsAvailability(ApplicationDbContext db, Guid schemeId) =>
//            TypedResults.Ok(await db.SchemeLabs.Where(x => x.SchemesID == schemeId)
//                .Select(x => new SchemeLabAvailabilityDto(
//                    x.SchemeLabsID,
//                    x.Schemes!.SchemeName,
//                    x.Investigations!.Investigation ,
//                    x.Price,
//                    x.Investigations.LabParameters!.Count // Count available tests
//                )).ToListAsync());
//    }
//}
