using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces
{
    public interface IPeriodoRepository
    {
        public Task<List<Periodo>> GetAllAsync();

        public Task<Periodo> GetByGuidAsync(Guid periodo_id);

        public Task<Periodo> GetByBillingMonthAsync(string periodo_mes_facturacion);

        public Task<Periodo> GetByDatesAndBillingMonthAsync(Periodo unPeriodo);

        public Task<List<Consumo>> GetAssociatedConsumptionAsync(Guid periodo_id);

        public Task<int> GetTotalComponentCostsByTermGuidAsync(Guid periodo_id);

        public Task<int> GetTotalConsumptionByTermGuidAsync(Guid periodo_id);

        public Task<bool> CreateAsync(Periodo unPeriodo);

        public Task<bool> UpdateAsync(Periodo unPeriodo);

        public Task<bool> RemoveAsync(Guid periodo_id);
    }
}
