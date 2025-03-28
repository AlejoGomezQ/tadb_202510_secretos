using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using Dapper;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Repositories
{
    public class ConsumoRepository(PgsqlDbContext unContexto) : IConsumoRepository
    {
        private readonly PgsqlDbContext contextoDB = unContexto;

        public async Task<List<Consumo>> GetAllAsync()
        {
            var conexion = contextoDB
                .CreateConnection();

            string sentenciaSQL =
                "SELECT DISTINCT  servicio_uuid servicioId, servicio, " +
                "periodo_uuid periodoId, mes_facturacion mesFacturacion, " +
                "lectura_actual lecturaActual, lectura_anterior lecturaAnterior, " +
                "constante, valor " +
                "FROM v_info_consumos " +
                "ORDER BY servicio, lectura_actual";

            var resultadoConsumos = await conexion
                .QueryAsync<Consumo>(sentenciaSQL, new DynamicParameters());

            return resultadoConsumos.ToList();

            //TODO: Crear el método para consultar por Guid - Consumo
            //TODO: Crear el método para insertar - Consumo
            //TODO: Crear el método para actualiza - Consumo
            //TODO: Crear el método para borrar - Consumo
        }
    }
}