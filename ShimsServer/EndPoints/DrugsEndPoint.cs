using Microsoft.EntityFrameworkCore;
using ShimsServer.Context;
using ShimsServer.Models.Drugs;

namespace ShimsServer.EndPoints
{
    public static class DrugsEndPoint
    {
        extension(IEndpointRouteBuilder app)
        {
            public void MapDrugsEndPoints()
            {
                var drugsGroup = app.MapGroup("/drugs/").WithTags("Drugs")
                    .RequireAuthorization()
                    .RequireCors("bStudioApps");
                drugsGroup.MapGet("", GetDrugs);
                drugsGroup.MapGet("{id:guid}", GetDrugById);
                drugsGroup.MapPost("", AddDrug);
                drugsGroup.MapGet("Check", DrugExists);
                drugsGroup.MapPut("{id:guid:required}", UpdateDrug);
                drugsGroup.MapDelete("{id:guid:required}", DeleteDrug);
            }
        }

        static async Task<IResult> GetDrugs(ApplicationDbContext db) =>
            TypedResults.Ok(await db.Drugs.Select(x => new DrugDTO(
                x.DrugsID,
                x.Drug,
                x.Tags,
                x.Description,
                x.DateAdded
            )).ToListAsync());

        static async Task<IResult> GetDrugById(ApplicationDbContext db, Guid id, CancellationToken token) =>
            await db.Drugs.Where(p => p.DrugsID == id)
                .Select(p => new DrugDTO(
                    p.DrugsID,
                    p.Drug,
                    p.Tags,
                    p.Description,
                    p.DateAdded
                )).FirstOrDefaultAsync(token) is DrugDTO drug ?
                TypedResults.Ok(drug) :
                TypedResults.NotFound();

        static async Task<bool> DrugExists(ApplicationDbContext db, string name, CancellationToken token) =>
            await db.Drugs.AnyAsync(x => x.Drug == name, token);

        static async Task<IResult> AddDrug(ApplicationDbContext db, AddDrugDto drugDto, CancellationToken token)
        {
            if (await DrugExists(db, drugDto.Drug, token))
                return TypedResults.Conflict($"Drug with name {drugDto.Drug} already exists.");

            var drug = new Drugs
            {
                DrugsID = Guid.CreateVersion7(),
                Drug = drugDto.Drug,
                Description = drugDto.Description,
                Tags = drugDto.Tags,
                DateAdded = DateTime.UtcNow
            };

            db.Drugs.Add(drug);
            await db.SaveChangesAsync(token);

            return TypedResults.Ok(drug.DrugsID);
        }

        static async Task<IResult> UpdateDrug(ApplicationDbContext db, Guid id, UpdateDrugDto drugDto, CancellationToken token)
        {
            var drug = await db.Drugs.FindAsync(new object[] { id }, cancellationToken: token);
            if (drug == null)
                return TypedResults.NotFound();

            if (drug.Drug != drugDto.Drug && await DrugExists(db, drugDto.Drug, token))
                return TypedResults.Conflict($"Drug with name {drugDto.Drug} already exists.");

            drug.Drug = drugDto.Drug;
            drug.Description = drugDto.Description;
            drug.Tags = drugDto.Tags;

            db.Entry(drug).State = EntityState.Modified;
            await db.SaveChangesAsync(token);

            return TypedResults.Ok();
        }

        static async Task<IResult> DeleteDrug(ApplicationDbContext db, Guid id, CancellationToken token)
        {
            var drug = await db.Drugs.FindAsync(new object[] { id }, cancellationToken: token);
            if (drug == null)
                return TypedResults.NotFound();

            db.Drugs.Remove(drug);
            await db.SaveChangesAsync(token);

            return TypedResults.Ok();
        }
    }
}
