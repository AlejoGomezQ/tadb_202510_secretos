using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using Dapper;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Repositories
{
    public class ServicioRepository(PgsqlDbContext unContexto) : IServicioRepository
    {
        private readonly PgsqlDbContext contextoDB = unContexto;

        public async Task<List<Servicio>> GetAllAsync()
        {
            var conexion = contextoDB
                .CreateConnection();

            string sentenciaSQL =
                "SELECT DISTINCT id, nombre, " +
                "unidad_medida unidadmedida " +
                "FROM core.servicios ORDER BY nombre";

            var resultadoServicios = await conexion
                .QueryAsync<Servicio>(sentenciaSQL, new DynamicParameters());

            return resultadoServicios.ToList();
        }
    }
}
