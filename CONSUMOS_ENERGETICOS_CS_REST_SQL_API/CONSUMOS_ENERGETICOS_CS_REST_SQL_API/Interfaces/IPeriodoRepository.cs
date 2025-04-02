using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces
{
    public interface IPeriodoRepository
    {
        public Task<List<Periodo>> GetAllAsync();

        public Task<Periodo> GetByGuidAsync(Guid periodo_id);

        public Task<List<Consumo>> GetAssociatedConsumptionAsync(Guid periodo_id);

        //TODO: Crear el método para insertar - periodo
        //TODO: Crear el método para actualiza - periodo
        //TODO: Crear el método para borrar - periodo
    }
}
