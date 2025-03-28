using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using Dapper;
using System.Data;

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
                "FROM core.v_info_consumos " +
                "ORDER BY servicio, lectura_actual";

            var resultadoConsumos = await conexion
                .QueryAsync<Consumo>(sentenciaSQL, new DynamicParameters());

            return resultadoConsumos.ToList();
        }

        public async Task<List<Consumo>> GetByTermGuidAsync(Guid periodo_id)
        {
            var conexion = contextoDB
                .CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@periodo_id", periodo_id,
                                    DbType.Guid, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT DISTINCT  servicio_uuid servicioId, servicio, " +
                "periodo_uuid periodoId, mes_facturacion mesFacturacion, " +
                "lectura_actual lecturaActual, lectura_anterior lecturaAnterior, " +
                "constante, valor " +
                "FROM core.v_info_consumos " +
                "WHERE periodo_uuid = @periodo_id " +
                "ORDER BY servicio, lectura_actual";

            var resultadoConsumos = await conexion
                .QueryAsync<Consumo>(sentenciaSQL,
                                    parametrosSentencia);

            return resultadoConsumos.ToList();
        }

        //TODO: Consultar Consumo por Guid de Servicio
        //TODO: Crear el método para insertar - Consumo
        //TODO: Crear el método para actualiza - Consumo
        //TODO: Crear el método para borrar - Consumo
    }
}