using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using Dapper;
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
                "SELECT DISTINCT id, fecha_inicio," +
                "to_char(fecha_inicio,'DD/MM/YYYY') fechaInicio, " +
                "to_char(fecha_final,'DD/MM/YYYY') fechaFinal, " +
                "total_dias totaldias, mes_facturacion mesFacturacion " +
                "FROM core.periodos ORDER BY fecha_inicio";

            var resultadoPeriodos = await conexion
                .QueryAsync<Periodo>(sentenciaSQL, new DynamicParameters());

            return resultadoPeriodos.ToList();
        }

        public async Task<Periodo> GetByIdAsync(int periodo_id)
        {
            Periodo unPeriodo = new();

            var conexion = contextoDB
                .CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@periodo_id", periodo_id,
                                    DbType.Int32, ParameterDirection.Input);

            string sentenciaSQL =
            "SELECT DISTINCT id," +
            "to_char(fecha_inicio,'DD/MM/YYYY') fechaInicio, " +
            "to_char(fecha_final,'DD/MM/YYYY') fechaFinal, " +
            "total_dias totaldias, mes_facturacion mesFacturacion " +
            "FROM core.periodos " +
            "WHERE id = @periodo_id ";
                
            var resultado = await conexion.QueryAsync<Periodo>(sentenciaSQL,
                parametrosSentencia);

            if (resultado.Any())
                unPeriodo = resultado.First();

            return unPeriodo;
        }
    }
}
