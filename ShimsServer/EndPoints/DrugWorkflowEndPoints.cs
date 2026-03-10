using Microsoft.EntityFrameworkCore;
using ShimsServer.Context;
using ShimsServer.Models.Drugs;

namespace ShimsServer.EndPoints
{
    public static class DrugWorkflowEndPoints
    {
        extension(IEndpointRouteBuilder app)
        {
            public void MapDrugWorkflowEndPoints()
            {
                var workflowGroup = app.MapGroup("/drugs/workflow/").WithTags("Drug Workflow")
                    .RequireAuthorization()
                    .RequireCors("bStudioApps");
                workflowGroup.MapGet("{id:guid}", GetDrugWorkflow);
            }
        }

        static async Task<IResult> GetDrugWorkflow(ApplicationDbContext db, Guid id, CancellationToken token) =>
            await db.DrugsRequests.Where(x => x.DrugsRequestsID == id)
                .Select(x => new DrugWorkflowDTO(
                    x.DrugsRequestsID,
                    x.SchemeDrugs!.Drugs!.Drug,
                    x.QuantityRequested,
                    x.DispensingCalculations!.Quantity,
                    x.DispensingCalculations.DrugPayments!.Dispensing!.QuantityDispensed,
                    x.DispensingCalculations.DrugPayments.Amount,
                    x.IsDispensed ? "Dispensed" : (x.IsPaid ? "Paid" : (x.DispensingCalculations != null ? "Calculated" : "Requested"))
                )).FirstOrDefaultAsync(token) is DrugWorkflowDTO workflow ?
                TypedResults.Ok(workflow) :
                TypedResults.NotFound();
    }
}
