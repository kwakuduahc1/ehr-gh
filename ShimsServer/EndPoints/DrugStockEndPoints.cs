using Microsoft.EntityFrameworkCore;
using ShimsServer.Context;
using ShimsServer.Models.Drugs;

namespace ShimsServer.EndPoints
{
    public static class DrugStockEndPoints
    {
        extension(IEndpointRouteBuilder app)
        {
            public void MapDrugStockEndPoints()
            {
                var stockGroup = app.MapGroup("/drugs/stock/").WithTags("Drug Stock")
                    .RequireAuthorization()
                    .RequireCors("bStudioApps");
                stockGroup.MapPost("", AddDrugStock);
                stockGroup.MapGet("{id:guid}", GetDrugStock);
                stockGroup.MapPut("{id:guid:required}", UpdateDrugStock);
                stockGroup.MapGet("inventory", GetDrugInventory);
            }
        }

        static async Task<IResult> AddDrugStock(ApplicationDbContext db, AddDrugStockDto stockDto, CancellationToken token)
        {
            var drugExists = await db.Drugs.AnyAsync(x => x.DrugsID == stockDto.DrugsID, token);
            if (!drugExists)
                return TypedResults.NotFound($"Drug not found.");

            var stock = new DrugsStock
            {
                DrugsStockID = Guid.CreateVersion7(),
                DrugsID = stockDto.DrugsID,
                Quantity = stockDto.Quantity,
                TranDate = DateTime.UtcNow
            };

            db.DrugsStocks.Add(stock);
            await db.SaveChangesAsync(token);

            return TypedResults.Ok(stock.DrugsStockID);
        }

        static async Task<IResult> GetDrugStock(ApplicationDbContext db, Guid id, CancellationToken token) =>
            await db.DrugsStocks.Where(x => x.DrugsStockID == id)
                .Select(x => new DrugStockDTO(
                    x.DrugsStockID,
                    x.DrugsID,
                    x.Drugs!.Drug,
                    x.Quantity,
                    x.TranDate
                )).FirstOrDefaultAsync(token) is DrugStockDTO stock ?
                TypedResults.Ok(stock) :
                TypedResults.NotFound();

        static async Task<IResult> UpdateDrugStock(ApplicationDbContext db, Guid id, UpdateDrugStockDto stockDto, CancellationToken token)
        {
            var stock = await db.DrugsStocks.FindAsync(id,  token);
            if (stock == null)
                return TypedResults.NotFound();

            stock.Quantity = stockDto.Quantity;
            stock.TranDate = DateTime.UtcNow;

            db.Entry(stock).State = EntityState.Modified;
            await db.SaveChangesAsync(token);

            return TypedResults.Ok();
        }

        static async Task<IResult> GetDrugInventory(ApplicationDbContext db) =>
            TypedResults.Ok(await db.Drugs.Select(x => new DrugWithStockDto(
                x.DrugsID,
                x.Drug,
                x.Description,
                (short)(x.DrugsStocks!.Sum(s => s.Quantity)),
                10 // Default low stock threshold
            )).ToListAsync());
    }
}
