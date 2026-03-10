using Microsoft.EntityFrameworkCore;
using ShimsServer.Context;
using ShimsServer.Models.Schemes;

namespace ShimsServer.EndPoints
{
    public static class SchemeServicesEndPoints
    {
        extension(IEndpointRouteBuilder app)
        {
            public void MapSchemeServicesEndPoints()
            {
                var schemeServicesGroup = app.MapGroup("/schemes/{schemeId:guid}/services/").WithTags("Scheme Services")
                    //  .RequireAuthorization()
                    .RequireCors("bStudioApps");
                schemeServicesGroup.MapGet("", GetSchemeServices);
                schemeServicesGroup.MapGet("{id:guid}", GetSchemeServiceById);
                schemeServicesGroup.MapPost("", AddSchemeService);
                schemeServicesGroup.MapPut("{id:guid:required}", UpdateSchemeService);
                schemeServicesGroup.MapDelete("{id:guid:required}", DeleteSchemeService);
                schemeServicesGroup.MapGet("coverage", GetSchemeServicesCoverage);
            }
        }

        static async Task<IResult> GetSchemeServices(ApplicationDbContext db, Guid schemeId) =>
            TypedResults.Ok(await db.SchemeServices.Where(x => x.SchemesID == schemeId)
                .Select(x => new SchemeServiceDTO(
                    x.SchemeServicesID,
                    x.SchemesID,
                    x.ServicesID,
                    x.Schemes!.SchemeName,
                    x.Services!.Service,
                    x.Price,
                    x.DateSet
                )).ToListAsync());

        static async Task<IResult> GetSchemeServiceById(ApplicationDbContext db, Guid schemeId, Guid id, CancellationToken token) =>
            await db.SchemeServices.Where(x => x.SchemesID == schemeId && x.SchemeServicesID == id)
                .Select(x => new SchemeServiceDTO(
                    x.SchemeServicesID,
                    x.SchemesID,
                    x.ServicesID,
                    x.Schemes!.SchemeName,
                    x.Services!.Service,
                    x.Price,
                    x.DateSet
                )).FirstOrDefaultAsync(token) is SchemeServiceDTO schemeService ?
                TypedResults.Ok(schemeService) :
                TypedResults.NotFound();

        static async Task<IResult> AddSchemeService(ApplicationDbContext db, HttpContext context, Guid schemeId, AddSchemeServiceDto serviceDto, CancellationToken token)
        {
            var schemeExists = await db.Schemes.AnyAsync(x => x.SchemesID == schemeId, token);
            if (!schemeExists)
                return TypedResults.NotFound("Scheme not found.");

            var serviceExists = await db.Services.AnyAsync(x => x.ServicesID == serviceDto.ServicesID, token);
            if (!serviceExists)
                return TypedResults.NotFound("Service not found.");

            var schemeService = new SchemeServices
            {
                SchemeServicesID = Guid.CreateVersion7(),
                SchemesID = schemeId,
                ServicesID = serviceDto.ServicesID,
                Price = serviceDto.Price,
                DateSet = DateTime.UtcNow,
                UserName = context.User.Identity?.Name ?? "Unknown"
            };

            db.SchemeServices.Add(schemeService);
            await db.SaveChangesAsync(token);

            return TypedResults.Ok(schemeService.SchemeServicesID);
        }

        static async Task<IResult> UpdateSchemeService(ApplicationDbContext db, Guid schemeId, Guid id, UpdateSchemeServiceDto serviceDto, CancellationToken token)
        {
            var schemeService = await db.SchemeServices.FirstOrDefaultAsync(x => x.SchemesID == schemeId && x.SchemeServicesID == id, token);
            if (schemeService == null)
                return TypedResults.NotFound();

            schemeService.Price = serviceDto.Price;
            schemeService.DateSet = DateTime.UtcNow;

            db.Entry(schemeService).State = EntityState.Modified;
            await db.SaveChangesAsync(token);

            return TypedResults.Ok();
        }

        static async Task<IResult> DeleteSchemeService(ApplicationDbContext db, Guid schemeId, Guid id, CancellationToken token)
        {
            var schemeService = await db.SchemeServices.FirstOrDefaultAsync(x => x.SchemesID == schemeId && x.SchemeServicesID == id, token);
            if (schemeService == null)
                return TypedResults.NotFound();

            db.SchemeServices.Remove(schemeService);
            await db.SaveChangesAsync(token);

            return TypedResults.Ok();
        }

        static async Task<IResult> GetSchemeServicesCoverage(ApplicationDbContext db, Guid schemeId) =>
            TypedResults.Ok(await db.SchemeServices.Where(x => x.SchemesID == schemeId)
                .Select(x => new SchemeServiceAvailabilityDto(
                    x.SchemeServicesID,
                    x.Schemes!.SchemeName,
                    x.Services!.Service,
                    x.Services.ServiceGroup,
                    x.Price,
                    true // All scheme services are considered covered
                )).ToListAsync());
    }
}
