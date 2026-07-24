using Dapper;
using ShimsServer.Models.Drugs;

namespace ShimsServer.Repositories
{
    public interface IDispensaryRepository
    {
        Task<bool> CheckPrescription(Guid PrescriptionID, CancellationToken token);

        Task<int> StockBalance(Guid[] DrugsID, CancellationToken token);

        Task<int> SetQuantities(AddDispensingCalculationDto[] prescriptions, CancellationToken token);

        Task<int> DeletePrescription(Guid prescriptionID, CancellationToken token);

        Task<int> EditCalculation(UpdateDispensingCalculationDto[] calculations, CancellationToken token);
    }

    public class DispensaryRepository(IConnection connection) : IDispensaryRepository
    {
        public Task<bool> CheckPrescription(Guid PrescriptionID, CancellationToken token)
        {
        throw new NotImplementedException();
        }

        public Task<int> DeletePrescription(Guid prescriptionID, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<int> EditCalculation(UpdateDispensingCalculationDto[] calculations, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<int> SetQuantities(AddDispensingCalculationDto[] prescriptions, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<int> StockBalance(Guid[] DrugsID, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
