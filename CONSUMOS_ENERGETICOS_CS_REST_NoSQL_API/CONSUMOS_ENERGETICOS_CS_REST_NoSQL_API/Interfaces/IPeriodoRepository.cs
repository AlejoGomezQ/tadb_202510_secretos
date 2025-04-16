using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Interfaces
{
    public interface IPeriodoRepository
    {
        public Task<List<Periodo>> GetAllAsync();

        public Task<Periodo> GetByIdAsync(string periodo_id);

        public Task<Periodo> GetByBillingMonthAsync(string periodo_mes_facturacion);

        public Task<Periodo> GetByDatesAndBillingMonthAsync(Periodo unPeriodo);

        public Task<List<Consumo>> GetAssociatedConsumptionAsync(string periodo_id);

        //public Task<int> GetTotalComponentCostsByTermGuidAsync(string periodo_id);

        public Task<int> GetTotalConsumptionByTermGuidAsync(string periodo_id);

        public Task<bool> CreateAsync(Periodo unPeriodo);

        public Task<bool> UpdateAsync(Periodo unPeriodo);

        public Task<bool> RemoveAsync(string periodo_id);
    }
}
