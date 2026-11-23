using Dapper;
using ShimsServer.Models.DTOs;

namespace ShimsServer.Repositories
{
    public interface IInvestigationsRepository
    {
        Task<bool> InvestigationExists(string investigation, CancellationToken token);

        Task<bool> InvestigationExists(Guid id, CancellationToken token);

        Task<int> AddInvestigation(Guid investigationId, AddInvestigationDto investigation, CancellationToken token);

        Task<IEnumerable<InvestigationsDto>> Investigations(CancellationToken token);

        Task<InvestigationsDto?> GetInvestigation(Guid id, CancellationToken token);

        Task<int> DeleteInvestigation(Guid investigationId, CancellationToken token);

        Task<int> EditInvestigation(UpdateInvestigationDto investigation, CancellationToken token);
    }

    public class InvestigationsRepository(IConnection connection) : IInvestigationsRepository
    {
        public async Task<int> AddInvestigation(Guid investigationId, AddInvestigationDto investigation, CancellationToken token)
        {
            const string qry = """
                INSERT INTO public.investigations(
                    investigationsid, investigation, category, investigationtype, levels, labdescription)
                VALUES (
                    @investigationId, @Investigation, @Category, @InvestigationType, @Levels, @LabDescription);
                """;
            using var con = await connection.ConnectionAsync(token);
            var command = new CommandDefinition(qry, new
            {
                investigationId,
                investigation.Investigation,
                investigation.Category,
                investigation.InvestigationType,
                investigation.Levels,
                investigation.LabDescription
            }, cancellationToken: token);
            return await con.ExecuteAsync(command);
        }

        public async Task<IEnumerable<InvestigationsDto>> Investigations(CancellationToken token)
        {
            const string qry = """
                SELECT investigationsid, investigation, category, investigationtype, levels, labdescription
                FROM public.investigations
                ORDER BY investigation;
                """;
            using var con = await connection.ConnectionAsync(token);
            var command = new CommandDefinition(qry, cancellationToken: token);
            return await con.QueryAsync<InvestigationsDto>(command);
        }

        public async Task<InvestigationsDto?> GetInvestigation(Guid id, CancellationToken token)
        {
            const string qry = """
                SELECT investigationsid, investigation, category, investigationtype, levels, labdescription
                FROM public.investigations
                WHERE investigationsid = @id;
                """;
            using var con = await connection.ConnectionAsync(token);
            var command = new CommandDefinition(qry, new { id }, cancellationToken: token);
            return await con.QuerySingleOrDefaultAsync<InvestigationsDto>(command);
        }

        public async Task<int> DeleteInvestigation(Guid id, CancellationToken token)
        {
            const string sql = """
                DELETE FROM public.investigations
                WHERE investigationsid = @id;
                """;
            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: token));
        }

        public async Task<int> EditInvestigation(UpdateInvestigationDto investigation, CancellationToken token)
        {
            const string sql = """
                UPDATE public.investigations
                SET investigation = @Investigation,
                    category = @Category,
                    investigationtype = @InvestigationType,
                    levels = @Levels,
                    labdescription = @LabDescription
                WHERE investigationsid = @InvestigationsID;
                """;
            using var con = await connection.ConnectionAsync(token);
            var command = new CommandDefinition(sql, new
            {
                investigation.Investigation,
                investigation.Category,
                investigation.InvestigationType,
                investigation.Levels,
                investigation.LabDescription,
                investigation.InvestigationsID
            }, cancellationToken: token);
            return await con.ExecuteAsync(command);
        }

        public async Task<bool> InvestigationExists(string investigation, CancellationToken token)
        {
            const string sql = """
                SELECT EXISTS (
                    SELECT 1
                    FROM public.investigations
                    WHERE investigation = @investigation
                );
                """;
            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteScalarAsync<bool>(
                new CommandDefinition(sql, new { investigation }, cancellationToken: token));
        }

        public async Task<bool> InvestigationExists(Guid id, CancellationToken token)
        {
            const string sql = """
                SELECT EXISTS (
                    SELECT 1
                    FROM public.investigations
                    WHERE investigationsid = @id
                );
                """;
            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteScalarAsync<bool>(
                new CommandDefinition(sql, new { id }, cancellationToken: token));
        }
    }
}
