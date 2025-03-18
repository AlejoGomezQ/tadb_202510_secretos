using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using Dapper;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Repositories
{
    public class PeriodoRepository(PgsqlDbContext unContexto) : IPeriodoRepository
    {
        private readonly PgsqlDbContext contextoDB = unContexto;

        public async Task<List<Periodo>> GetAllAsync()
        {
            var conexion = contextoDB
                .CreateConnection();

            string sentenciaSQL =
                "SELECT DISTINCT id, fecha_inicio," +
                "to_char(fecha_inicio,'DD/MM/YYYY') fechaInicio, " +
                "to_char(fecha_final,'DD/MM/YYYY') fechaFinal, " +
                "mes_facturacion mesFacturacion " +
                "FROM core.periodos ORDER BY fecha_inicio";

            var resultadoPeriodos = await conexion
                .QueryAsync<Periodo>(sentenciaSQL, new DynamicParameters());

            return resultadoPeriodos.ToList();
        }
    }
}
