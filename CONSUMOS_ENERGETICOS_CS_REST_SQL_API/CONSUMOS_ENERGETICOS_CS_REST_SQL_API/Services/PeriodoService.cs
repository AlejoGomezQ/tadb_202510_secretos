using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Services
{
    public class PeriodoService(IPeriodoRepository periodoRepository)
    {
        private readonly IPeriodoRepository _periodoRepository = periodoRepository;

        public async Task<List<Periodo>> GetAllAsync()
        {
            return await _periodoRepository
                .GetAllAsync();
        }
    }
}
