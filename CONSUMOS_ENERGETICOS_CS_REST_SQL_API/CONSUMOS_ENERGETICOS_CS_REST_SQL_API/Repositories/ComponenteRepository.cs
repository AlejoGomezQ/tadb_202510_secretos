using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using Dapper;
using System.Data;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Repositories
{
    public class ComponenteRepository(PgsqlDbContext unContexto) : IComponenteRepository
    {
        private readonly PgsqlDbContext contextoDB = unContexto;

        public async Task<List<Componente>> GetAllAsync()
        {
            var conexion = contextoDB
                .CreateConnection();

            string sentenciaSQL =
                "SELECT DISTINCT componente_uuid id, componente nombre, servicio, " +
                "servicio_uuid servicioId " +
                "FROM core.v_info_componentes " +
                "ORDER BY servicio, nombre";

            var resultadoComponentes = await conexion
                .QueryAsync<Componente>(sentenciaSQL, new DynamicParameters());

            return resultadoComponentes.ToList();
        }
    }
}