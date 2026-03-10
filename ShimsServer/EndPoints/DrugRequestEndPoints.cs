using Microsoft.EntityFrameworkCore;
using ShimsServer.Context;
using ShimsServer.Models.Drugs;

namespace ShimsServer.EndPoints
{
    public static class DrugRequestEndPoints
    {
        extension(IEndpointRouteBuilder app)
        {
            public void MapDrugRequestEndPoints()
            {
                var requestGroup = app.MapGroup("/drugs/requests/").WithTags("Drug Requests")
                    //  .RequireAuthorization()
                    .RequireCors("bStudioApps");
                requestGroup.MapGet("", GetDrugRequests);
                requestGroup.MapGet("{id:guid}", GetDrugRequestById);
                requestGroup.MapPost("", AddDrugRequest);
                requestGroup.MapGet("summary", GetDrugRequestsSummary);
            }
        }

        static async Task<IResult> GetDrugRequests(ApplicationDbContext db) =>
            TypedResults.Ok(await db.DrugsRequests.Select(x => new DrugRequestDTO(
                x.DrugsRequestsID,
                x.PatientsAttendancesID,
                x.SchemeDrugsID,
                x.SchemeDrugs!.Drugs!.Drug,
                x.Frequency,
                x.Days,
                x.QuantityRequested,
                x.DateRequested,
                x.IsPaid,
                x.IsDispensed
            )).ToListAsync());

        static async Task<IResult> GetDrugRequestById(ApplicationDbContext db, Guid id, CancellationToken token) =>
            await db.DrugsRequests.Where(x => x.DrugsRequestsID == id)
                .Select(x => new DrugRequestDTO(
                    x.DrugsRequestsID,
                    x.PatientsAttendancesID,
                    x.SchemeDrugsID,
                    x.SchemeDrugs!.Drugs!.Drug,
                    x.Frequency,
                    x.Days,
                    x.QuantityRequested,
                    x.DateRequested,
                    x.IsPaid,
                    x.IsDispensed
                )).FirstOrDefaultAsync(token) is DrugRequestDTO request ?
                TypedResults.Ok(request) :
                TypedResults.NotFound();

        static async Task<IResult> AddDrugRequest(ApplicationDbContext db, HttpContext context, AddDrugRequestDto requestDto, CancellationToken token)
        {
            var request = new DrugsRequests
            {
                DrugsRequestsID = Guid.CreateVersion7(),
                PatientsAttendancesID = requestDto.PatientsAttendancesID,
                SchemeDrugsID = requestDto.SchemeDrugsID,
                Frequency = requestDto.Frequency,
                Days = requestDto.Days,
                QuantityRequested = requestDto.QuantityRequested,
                DateRequested = DateTime.UtcNow,
                UserName = context.User.Identity?.Name ?? "Unknown"
            };

            db.DrugsRequests.Add(request);
            await db.SaveChangesAsync(token);

            return TypedResults.Ok(request.DrugsRequestsID);
        }

        static async Task<IResult> GetDrugRequestsSummary(ApplicationDbContext db) =>
            TypedResults.Ok(await db.DrugsRequests.Select(x => new DrugRequestSummaryDto(
                x.DrugsRequestsID,
                "", //x.PatientsAttendances!.PatientSchemes!.Patients!.Surname + " " + x.PatientsAttendances.PatientSchemes.Patients.OtherNames,
                x.SchemeDrugs!.Drugs!.Drug,
                x.Frequency,
                x.Days,
                x.QuantityRequested,
                x.DateRequested,
                x.IsPaid,
                x.IsDispensed
            )).ToListAsync());
    }
}
