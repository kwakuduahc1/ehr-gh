using Microsoft.EntityFrameworkCore;
using ShimsServer.Context;
using ShimsServer.Models.Drugs;

namespace ShimsServer.EndPoints
{
    public static class DispensingCalculationEndPoints
    {
        extension(IEndpointRouteBuilder app)
        {
            public void MapDispensingCalculationEndPoints()
            {
                var dispensingCalcGroup = app.MapGroup("/drugs/dispensing-calculations/").WithTags("Dispensing Calculations")
                    //  .RequireAuthorization()
                    .RequireCors("bStudioApps");
                dispensingCalcGroup.MapPost("", AddDispensingCalculation);
                dispensingCalcGroup.MapGet("{id:guid}", GetDispensingCalculation);
                dispensingCalcGroup.MapPut("{id:guid:required}", UpdateDispensingCalculation);
            }
        }

        static async Task<IResult> AddDispensingCalculation(ApplicationDbContext db, HttpContext context, AddDispensingCalculationDto calcDto, CancellationToken token)
        {
            var calc = new DispensingCalculations
            {
                DrugsRequestsID = calcDto.DrugsRequestsID,
                Quantity = calcDto.Quantity,
                DateDone = DateTime.UtcNow,
                Notes = calcDto.Notes,
                UserName = context.User.Identity?.Name ?? "Unknown"
            };

            db.DispensingCalculations.Add(calc);
            await db.SaveChangesAsync(token);

            return TypedResults.Ok(calc.DrugsRequestsID);
        }

        static async Task<IResult> GetDispensingCalculation(ApplicationDbContext db, Guid id, CancellationToken token) =>
            await db.DispensingCalculations.Where(x => x.DrugsRequestsID == id)
                .Select(x => new DispensingCalculationDTO(
                    x.DrugsRequestsID,
                    x.Quantity,
                    x.DateDone,
                    x.UserName,
                    x.Notes
                )).FirstOrDefaultAsync(token) is DispensingCalculationDTO calc ?
                TypedResults.Ok(calc) :
                TypedResults.NotFound();

        static async Task<IResult> UpdateDispensingCalculation(ApplicationDbContext db, Guid id, UpdateDispensingCalculationDto calcDto, CancellationToken token)
        {
            var calc = await db.DispensingCalculations.FindAsync(new object[] { id }, cancellationToken: token);
            if (calc == null)
                return TypedResults.NotFound();

            calc.Quantity = calcDto.Quantity;
            calc.Notes = calcDto.Notes;
            calc.DateDone = DateTime.UtcNow;

            db.Entry(calc).State = EntityState.Modified;
            await db.SaveChangesAsync(token);

            return TypedResults.Ok();
        }
    }
}
