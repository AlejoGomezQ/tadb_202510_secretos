using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;
using System.Data;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Repositories
{
    public class ConsumoRepository(MongoDbContext unContexto) : IConsumoRepository
    {
        private readonly MongoDbContext contextoDB = unContexto;

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

            return [.. resultadoConsumos];
        }

        public async Task<Consumo> GetByBillingMonthAndServiceAsync(string mes_facturacion, string servicio_nombre)
        {
            Consumo consumoExistente = new();

            var conexion = contextoDB
                .CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@mes_facturacion", mes_facturacion,
                                    DbType.String, ParameterDirection.Input);
            parametrosSentencia.Add("@servicio_nombre", servicio_nombre,
                                    DbType.String, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT DISTINCT servicio_uuid servicioId, servicio, " +
                "periodo_uuid periodoId, mes_facturacion mesFacturacion, " +
                "lectura_actual lecturaActual, lectura_anterior lecturaAnterior, " +
                "constante, valor " +
                "FROM core.v_info_consumos " +
                "WHERE LOWER(mes_facturacion) = LOWER(@mes_facturacion) " +
                "AND LOWER(servicio) = LOWER(@servicio_nombre)";

            var resultado = await conexion.QueryAsync<Consumo>(sentenciaSQL,
                parametrosSentencia);

            if (resultado.Any())
                consumoExistente = resultado.First();

            return consumoExistente;
        }

        public async Task<bool> CreateAsync(Consumo unConsumo)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_inserta_consumo";
                var parametros = new
                {
                    p_periodo_uuid = unConsumo.PeriodoId,
                    p_servicio_uuid = unConsumo.ServicioId,
                    p_lectura_actual = unConsumo.LecturaActual,
                    p_lectura_anterior = unConsumo.LecturaAnterior,
                    p_constante = unConsumo.Constante
                };

                var cantidad_filas = await conexion.ExecuteAsync(
                    procedimiento,
                    parametros,
                    commandType: CommandType.StoredProcedure);

                if (cantidad_filas != 0)
                    resultadoAccion = true;
            }
            catch (NpgsqlException error)
            {
                throw new DbOperationException(error.Message);
            }

            return resultadoAccion;
        }

        public async Task<bool> UpdateAsync(Consumo unConsumo)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_actualiza_consumo";
                var parametros = new
                {
                    p_periodo_uuid = unConsumo.PeriodoId,
                    p_servicio_uuid = unConsumo.ServicioId,
                    p_lectura_actual = unConsumo.LecturaActual,
                    p_lectura_anterior = unConsumo.LecturaAnterior,
                    p_constante = unConsumo.Constante
                };

                var cantidad_filas = await conexion.ExecuteAsync(
                    procedimiento,
                    parametros,
                    commandType: CommandType.StoredProcedure);

                if (cantidad_filas != 0)
                    resultadoAccion = true;
            }
            catch (NpgsqlException error)
            {
                throw new DbOperationException(error.Message);
            }

            return resultadoAccion;
        }

        public async Task<bool> RemoveAsync(Consumo unConsumo)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_elimina_consumo";
                var parametros = new
                {
                    p_periodo_uuid = unConsumo.PeriodoId,
                    p_servicio_uuid = unConsumo.ServicioId
                };

                var cantidad_filas = await conexion.ExecuteAsync(
                    procedimiento,
                    parametros,
                    commandType: CommandType.StoredProcedure);

                if (cantidad_filas != 0)
                    resultadoAccion = true;
            }
            catch (NpgsqlException error)
            {
                throw new DbOperationException(error.Message);
            }

            return resultadoAccion;
        }
    }
}