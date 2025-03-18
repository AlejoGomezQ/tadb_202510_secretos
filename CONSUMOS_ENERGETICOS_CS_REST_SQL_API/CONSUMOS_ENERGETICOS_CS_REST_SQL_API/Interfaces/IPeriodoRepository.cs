using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces
{
    public interface IPeriodoRepository
    {
        public Task<List<Periodo>> GetAllAsync();
    }
}
