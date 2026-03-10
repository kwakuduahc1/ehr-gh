using Microsoft.EntityFrameworkCore;
using ShimsServer.Context;
using ShimsServer.Models.Drugs;

namespace ShimsServer.EndPoints
{
    public static class DispensingEndPoints
    {
        extension(IEndpointRouteBuilder app)
        {
            public void MapDispensingEndPoints()
            {
                var dispensingGroup = app.MapGroup("/drugs/dispensing/").WithTags("Dispensing")
                    //  .RequireAuthorization()
                    .RequireCors("bStudioApps");
                dispensingGroup.MapPost("", AddDispensing);
                dispensingGroup.MapGet("{id:guid}", GetDispensing);
                dispensingGroup.MapPut("{id:guid:required}", UpdateDispensing);
            }
        }

        static async Task<IResult> AddDispensing(ApplicationDbContext db, HttpContext context, AddDispensingDto dispensingDto, CancellationToken token)
        {
            var dispensing = new Dispensing
            {
                DrugPaymentsID = dispensingDto.DrugPaymentsID,
                QuantityDispensed = dispensingDto.QuantityDispensed,
                DateDispensed = DateTime.UtcNow,
                UserName = context.User.Identity?.Name ?? "Unknown"
            };

            db.Dispensings.Add(dispensing);
            await db.SaveChangesAsync(token);

            return TypedResults.Ok(dispensing.DrugPaymentsID);
        }

        static async Task<IResult> GetDispensing(ApplicationDbContext db, Guid id, CancellationToken token) =>
            await db.Dispensings.Where(x => x.DrugPaymentsID == id)
                .Select(x => new DispensingDTO(
                    x.DrugPaymentsID,
                    x.DateDispensed,
                    x.QuantityDispensed,
                    x.UserName
                )).FirstOrDefaultAsync(token) is DispensingDTO dispensing ?
                TypedResults.Ok(dispensing) :
                TypedResults.NotFound();

        static async Task<IResult> UpdateDispensing(ApplicationDbContext db, Guid id, UpdateDispensingDto dispensingDto, CancellationToken token)
        {
            var dispensing = await db.Dispensings.FindAsync(new object[] { id }, cancellationToken: token);
            if (dispensing == null)
                return TypedResults.NotFound();

            dispensing.QuantityDispensed = dispensingDto.QuantityDispensed;
            dispensing.DateDispensed = DateTime.UtcNow;

            db.Entry(dispensing).State = EntityState.Modified;
            await db.SaveChangesAsync(token);

            return TypedResults.Ok();
        }
    }
}
