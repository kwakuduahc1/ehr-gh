using Microsoft.EntityFrameworkCore;
using ShimsServer.Context;
using ShimsServer.Models.Drugs;

namespace ShimsServer.EndPoints
{
    public static class DrugPaymentEndPoints
    {
        extension(IEndpointRouteBuilder app)
        {
            public void MapDrugPaymentEndPoints()
            {
                var paymentGroup = app.MapGroup("/drugs/payments/").WithTags("Drug Payments")
                    //  .RequireAuthorization()
                    .RequireCors("bStudioApps");
                paymentGroup.MapPost("", AddDrugPayment);
                paymentGroup.MapGet("{id:guid}", GetDrugPayment);
                paymentGroup.MapPut("{id:guid:required}", UpdateDrugPayment);
                paymentGroup.MapGet("details/{id:guid}", GetDrugPaymentDetailed);
            }
        }

        static async Task<IResult> AddDrugPayment(ApplicationDbContext db, HttpContext context, AddDrugPaymentDto paymentDto, CancellationToken token)
        {
            var payment = new DrugPayments
            {
                DispensingCaculationsID = paymentDto.DispensingCaculationsID,
                Receipt = paymentDto.Receipt,
                QuantityPaid = paymentDto.QuantityPaid,
                Amount = paymentDto.Amount,
                DatePaid = DateTime.UtcNow,
                PaymentTypesID = paymentDto.PaymentTypesID,
                UserName = context.User.Identity?.Name ?? "Unknown"
            };

            db.DrugPayments.Add(payment);
            await db.SaveChangesAsync(token);

            return TypedResults.Ok(payment.DispensingCaculationsID);
        }

        static async Task<IResult> GetDrugPayment(ApplicationDbContext db, Guid id, CancellationToken token) =>
            await db.DrugPayments.Where(x => x.DispensingCaculationsID == id)
                .Select(x => new DrugPaymentDTO(
                    x.DispensingCaculationsID,
                    x.Receipt,
                    x.QuantityPaid,
                    x.Amount,
                    x.DatePaid,
                    x.PaymentTypesID,
                    x.UserName
                )).FirstOrDefaultAsync(token) is DrugPaymentDTO payment ?
                TypedResults.Ok(payment) :
                TypedResults.NotFound();

        static async Task<IResult> UpdateDrugPayment(ApplicationDbContext db, Guid id, UpdateDrugPaymentDto paymentDto, CancellationToken token)
        {
            var payment = await db.DrugPayments.FindAsync(new object[] { id }, cancellationToken: token);
            if (payment == null)
                return TypedResults.NotFound();

            payment.Receipt = paymentDto.Receipt;
            payment.QuantityPaid = paymentDto.QuantityPaid;
            payment.Amount = paymentDto.Amount;
            payment.PaymentTypesID = paymentDto.PaymentTypesID;

            db.Entry(payment).State = EntityState.Modified;
            await db.SaveChangesAsync(token);

            return TypedResults.Ok();
        }

        static async Task<IResult> GetDrugPaymentDetailed(ApplicationDbContext db, Guid id, CancellationToken token) =>
            await db.DrugPayments.Where(x => x.DispensingCaculationsID == id)
                .Select(x => new DrugPaymentDetailedDto(
                    x.DispensingCaculationsID,
                    x.Receipt,
                    x.QuantityPaid,
                    x.Amount,
                    x.DatePaid,
                    x.Dispensing!.QuantityDispensed,
                    x.Dispensing.DateDispensed,
                    x.Dispensing.UserName
                )).FirstOrDefaultAsync(token) is DrugPaymentDetailedDto payment ?
                TypedResults.Ok(payment) :
                TypedResults.NotFound();
    }
}
