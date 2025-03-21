using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces
{
    public interface IComponenteRepository
    {
        public Task<List<Componente>> GetAllAsync();
    }
}
