using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Services
{
    public class ComponenteService(IComponenteRepository componenteRepository)
    {
        private readonly IComponenteRepository _componenteRepository = componenteRepository;

        public async Task<List<Componente>> GetAllAsync()
        {
            return await _componenteRepository
                .GetAllAsync();
        }       
    }
}
