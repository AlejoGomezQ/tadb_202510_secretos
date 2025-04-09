using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Interfaces
{
    public interface IResumenRepository
    {
        public Task<Resumen> GetAllAsync();
    }
}