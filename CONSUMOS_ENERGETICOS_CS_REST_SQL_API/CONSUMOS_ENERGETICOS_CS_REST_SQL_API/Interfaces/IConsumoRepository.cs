using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces
{
    public interface IConsumoRepository
    {
        public Task<List<Consumo>> GetAllAsync();

        public Task<Consumo> GetByBillingMonthAndServiceAsync(string mes_facturacion, string servicio_nombre);

        public Task<bool> CreateAsync(Consumo unConsumo);

        public Task<bool> UpdateAsync(Consumo unConsumo);

        public Task<bool> RemoveAsync(Consumo unConsumo);
    }
}
