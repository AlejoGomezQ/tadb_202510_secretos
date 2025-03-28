using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces
{
    public interface IConsumoRepository
    {
        public Task<List<Consumo>> GetAllAsync();
        public Task<List<Consumo>> GetByTermGuidAsync(Guid periodo_id);

        //TODO: Consultar Consumo por Guid de Servicio
        //TODO: Crear el método para insertar - Consumo
        //TODO: Crear el método para actualiza - Consumo
        //TODO: Crear el método para borrar - Consumo
    }
}
