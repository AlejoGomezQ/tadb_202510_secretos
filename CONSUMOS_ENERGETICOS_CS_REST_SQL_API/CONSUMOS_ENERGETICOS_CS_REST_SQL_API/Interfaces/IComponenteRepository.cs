using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces
{
    public interface IComponenteRepository
    {
        public Task<List<Componente>> GetAllAsync();

        public Task<Componente> GetByGuidAsync(Guid componente_id);

        public Task<Componente> GetByNameAndServiceAsync(
            string componente_nombre,
            string componente_servicio);

        public Task<bool> CreateAsync(Componente unComponente);

        public Task<bool> UpdateAsync(Componente unComponente);

        public Task<bool> RemoveAsync(Guid componente_id);
    }
}
