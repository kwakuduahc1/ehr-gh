using Dapper;
using ShimsServer.Models.Drugs;

namespace ShimsServer.Repositories
{
    public interface IDrugsRepository
    {
        Task<bool> DrugExists(string drug, CancellationToken token);

        Task<bool> DrugExists(Guid id, CancellationToken token);

        Task<int> AddDrug(Guid DrugsID, AddDrugDto drug, CancellationToken token);

        Task<IEnumerable<DrugsDTO>> Drugs(CancellationToken token);

        Task<int> DeleteDrug(Guid drugId, CancellationToken token);

        Task<int> EditDrug(UpdateDrugDto drug, CancellationToken token);
    }

    public class DrugsRepository(IConnection connection) : IDrugsRepository
    {
        public async Task<int> AddDrug(Guid DrugsID, AddDrugDto drug, CancellationToken token)
        {
            const string qry = """
                INSERT INTO public.drugs(drugsid, drug, tags, description, dateadded)
                VALUES (@drugsid, @drug, @tags, @description, now());;
                """;
            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(qry, new { DrugsID, drug.Drug, drug.Description, drug.Tags });
        }

        public async Task<IEnumerable<DrugsDTO>> Drugs(CancellationToken token)
        {
            const string qry = """
                SELECT drugsid, drug, tags, description, dateadded
                FROM drugs;
                """;
            using var con = await connection.ConnectionAsync(token);
            return await con.QueryAsync<DrugsDTO>(qry);
        }

        public async Task<int> DeleteDrug(Guid id, CancellationToken token)
        {
            const string sql = """
                UPDATE drugs
                SET isdeleted = true
                WHERE drugsid = @id;
                """;
            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(sql, new { id });
        }

        public async Task<int> EditDrug(UpdateDrugDto drug, CancellationToken token)
        {
            const string sql = """
                UPDATE drugs
                SET drug = @Drug, tags = @Tags, description = @Description
                WHERE drugsid = @DrugsID;
                """;
            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(sql, new { drug.Drug, drug.Tags, drug.Description, drug.DrugsID });
        }

        public async Task<bool> DrugExists(string drug, CancellationToken token)
        {
            const string sql = """
                SELECT EXISTS (
                    SELECT 1
                    FROM drugs
                    WHERE drug = @drug
                );
                """;
            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteScalarAsync<bool>(sql, new { drug });
        }

        public async Task<bool> DrugExists(Guid id, CancellationToken token)
        {
            const string sql =
                """
                SELECT EXISTS(
                    SELECT true
                    FROM drugs
                    WHERE drugsid = @id
                """;
            using var con= await connection.ConnectionAsync(token);
            return await con.ExecuteScalarAsync<bool>(sql, new {id});
        }
    }
}
