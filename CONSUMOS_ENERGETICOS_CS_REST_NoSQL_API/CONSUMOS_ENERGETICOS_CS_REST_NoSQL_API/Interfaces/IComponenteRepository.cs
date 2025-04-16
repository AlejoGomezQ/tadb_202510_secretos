using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Interfaces
{
    public interface IComponenteRepository
    {
        public Task<List<Componente>> GetAllAsync();

        public Task<Componente> GetByIdAsync(string componente_id);

        public Task<Componente> GetByNameAndServiceAsync(
            string componente_nombre,
            string componente_servicio);

        public Task<bool> CreateAsync(Componente unComponente);

        public Task<bool> UpdateAsync(Componente unComponente);

        public Task<bool> RemoveAsync(string componente_id);
    }
}
