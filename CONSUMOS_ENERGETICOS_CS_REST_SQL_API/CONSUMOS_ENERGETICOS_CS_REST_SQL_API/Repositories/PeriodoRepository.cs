using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using Dapper;
using Npgsql;
using System.Data;

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
                "SELECT DISTINCT uuid id, fecha_inicio," +
                "to_char(fecha_inicio,'DD/MM/YYYY') fechaInicio, " +
                "to_char(fecha_final,'DD/MM/YYYY') fechaFinal, " +
                "total_dias totaldias, mes_facturacion mesFacturacion " +
                "FROM core.periodos ORDER BY fecha_inicio";

            var resultadoPeriodos = await conexion
                .QueryAsync<Periodo>(sentenciaSQL, new DynamicParameters());

            return [.. resultadoPeriodos];
        }

        public async Task<Periodo> GetByGuidAsync(Guid periodo_id)
        {
            Periodo unPeriodo = new();

            var conexion = contextoDB
                .CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@periodo_id", periodo_id,
                                    DbType.Guid, ParameterDirection.Input);

            string sentenciaSQL =
            "SELECT DISTINCT uuid id," +
            "to_char(fecha_inicio,'DD/MM/YYYY') fechaInicio, " +
            "to_char(fecha_final,'DD/MM/YYYY') fechaFinal, " +
            "total_dias totaldias, mes_facturacion mesFacturacion " +
            "FROM core.periodos " +
            "WHERE uuid = @periodo_id ";

            var resultado = await conexion.QueryAsync<Periodo>(sentenciaSQL,
                parametrosSentencia);

            if (resultado.Any())
                unPeriodo = resultado.First();

            return unPeriodo;
        }

        public async Task<Periodo> GetByBillingMonthAsync(string periodo_mes_facturacion)
        {
            Periodo periodoExistente = new();

            var conexion = contextoDB
                .CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@mes_facturacion", periodo_mes_facturacion,
                        DbType.String, ParameterDirection.Input);

            string sentenciaSQL =
            "SELECT DISTINCT uuid id," +
            "to_char(fecha_inicio,'DD/MM/YYYY') fechaInicio, " +
            "to_char(fecha_final,'DD/MM/YYYY') fechaFinal, " +
            "total_dias totaldias, mes_facturacion mesFacturacion " +
            "FROM core.periodos " +
            "WHERE LOWER(mes_facturacion) = LOWER(@mes_facturacion)";

            var resultado = await conexion.QueryAsync<Periodo>(sentenciaSQL,
                parametrosSentencia);

            if (resultado.Any())
                periodoExistente = resultado.First();

            return periodoExistente;
        }

        public async Task<Periodo> GetByDatesAndBillingMonthAsync(Periodo unPeriodo)
        {
            Periodo periodoExistente = new();

            var conexion = contextoDB
                .CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@fecha_inicio", unPeriodo.FechaInicio,
                                    DbType.String, ParameterDirection.Input);

            parametrosSentencia.Add("@fecha_final", unPeriodo.FechaFinal,
                                    DbType.String, ParameterDirection.Input);

            parametrosSentencia.Add("@mes_facturacion", unPeriodo.MesFacturacion,
                        DbType.String, ParameterDirection.Input);

            string sentenciaSQL =
            "SELECT DISTINCT uuid id," +
            "to_char(fecha_inicio,'DD/MM/YYYY') fechaInicio, " +
            "to_char(fecha_final,'DD/MM/YYYY') fechaFinal, " +
            "total_dias totaldias, mes_facturacion mesFacturacion " +
            "FROM core.periodos " +
            "WHERE to_char(fecha_inicio,'DD/MM/YYYY') = @fecha_inicio " +
            "AND to_char(fecha_final,'DD/MM/YYYY') = @fecha_final " +
            "AND LOWER(mes_facturacion) = LOWER(@mes_facturacion)";

            var resultado = await conexion.QueryAsync<Periodo>(sentenciaSQL,
                parametrosSentencia);

            if (resultado.Any())
                periodoExistente = resultado.First();

            return periodoExistente;
        }

        public async Task<List<Consumo>> GetAssociatedConsumptionAsync(Guid periodo_id)
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

            return [.. resultadoConsumos];
        }

        public async Task<int> GetTotalComponentCostsByTermGuidAsync(Guid periodo_id)
        {
            var conexion = contextoDB.CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@periodo_id", periodo_id,
                                    DbType.Guid, ParameterDirection.Input);

            string sentenciaSQL = "SELECT COUNT(componente_uuid) totalRegistros " +
                "FROM core.v_info_costos_componentes " +
                "WHERE periodo_uuid = @periodo_id";

            var totalRegistros = await conexion
                .QueryFirstAsync<int>(sentenciaSQL, parametrosSentencia);

            return totalRegistros;
        }

        public async Task<int> GetTotalConsumptionByTermGuidAsync(Guid periodo_id)
        {
            var conexion = contextoDB.CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@periodo_id", periodo_id,
                                    DbType.Guid, ParameterDirection.Input);

            string sentenciaSQL = "SELECT COUNT(periodo_uuid) totalRegistros " +
                "FROM core.v_info_consumos " +
                "WHERE periodo_uuid = @periodo_id";

            var totalRegistros = await conexion
                .QueryFirstAsync<int>(sentenciaSQL, parametrosSentencia);

            return totalRegistros;
        }

        public async Task<bool> CreateAsync(Periodo unPeriodo)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_inserta_periodo";
                var parametros = new
                {
                    p_fecha_inicial = unPeriodo.FechaInicio,
                    p_fecha_final = unPeriodo.FechaFinal,
                    p_total_dias = unPeriodo.TotalDias,
                    p_mes_facturacion = unPeriodo.MesFacturacion
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

        public async Task<bool> UpdateAsync(Periodo unPeriodo)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_actualiza_periodo";
                var parametros = new
                {
                    p_uuid = unPeriodo.Id,
                    p_fecha_inicial = unPeriodo.FechaInicio,
                    p_fecha_final = unPeriodo.FechaFinal,
                    p_total_dias = unPeriodo.TotalDias,
                    p_mes_facturacion = unPeriodo.MesFacturacion
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

        public async Task<bool> RemoveAsync(Guid periodo_id)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_elimina_periodo";
                var parametros = new
                {
                    p_uuid = periodo_id
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
