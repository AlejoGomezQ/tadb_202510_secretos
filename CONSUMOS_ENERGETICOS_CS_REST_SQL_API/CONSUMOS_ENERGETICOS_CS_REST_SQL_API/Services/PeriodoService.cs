using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
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

        public async Task<Periodo> GetByIdAsync(int periodo_id)
        {
            Periodo unPeriodo = await _periodoRepository
                .GetByIdAsync(periodo_id);

            if (unPeriodo.Id == 0)
                throw new AppValidationException($"Periodo no encontrado con el ID {periodo_id}");

            return unPeriodo;
        }
    }
}
