using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using Dapper;
using System.Data;

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
                "SELECT DISTINCT uuid id, nombre, " +
                "unidad_medida unidadmedida " +
                "FROM core.servicios ORDER BY nombre";

            var resultadoServicios = await conexion
                .QueryAsync<Servicio>(sentenciaSQL, new DynamicParameters());

            return resultadoServicios.ToList();
        }

        public async Task<Servicio> GetByGuidAsync(Guid servicio_id)
        {
            Servicio unServicio = new();

            var conexion = contextoDB
                .CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@servicio_id", servicio_id,
                                    DbType.Guid, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT DISTINCT uuid id, nombre, " +
                "unidad_medida unidadmedida " +
                "FROM core.servicios " +
                "WHERE uuid = @servicio_id";

            var resultado = await conexion.QueryAsync<Servicio>(sentenciaSQL,
                parametrosSentencia);

            if (resultado.Any())
                unServicio = resultado.First();

            return unServicio;
        }

    }
}
