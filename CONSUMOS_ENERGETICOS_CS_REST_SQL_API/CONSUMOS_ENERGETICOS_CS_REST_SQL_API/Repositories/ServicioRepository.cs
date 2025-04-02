using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using Dapper;
using Npgsql;
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

            return [.. resultadoServicios];
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

            var resultado = await conexion
                .QueryAsync<Servicio>(sentenciaSQL, parametrosSentencia);

            if (resultado.Any())
                unServicio = resultado.First();

            return unServicio;
        }

        public async Task<List<Componente>> GetAssociatedComponentsAsync(Guid servicio_id)
        {
            List<Componente> componentesAsociados = [];

            var conexion = contextoDB
                .CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@servicio_id", servicio_id,
                                    DbType.Guid, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT DISTINCT componente_uuid id, componente nombre, servicio, " +
                "servicio_uuid servicioId " +
                "FROM core.v_info_componentes " +
                "WHERE servicio_uuid = @servicio_id " +
                "ORDER BY nombre";

            var resultado = await conexion
                .QueryAsync<Componente>(sentenciaSQL, parametrosSentencia);

            if (resultado.Any())
                componentesAsociados = [.. resultado];

            return componentesAsociados;
        }

        public async Task<List<Consumo>> GetAssociatedConsumptionAsync(Guid servicio_id)
        {
            List<Consumo> consumosAsociados = [];

            var conexion = contextoDB
                .CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@servicio_id", servicio_id,
                                    DbType.Guid, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT DISTINCT  servicio_uuid servicioId, servicio, " +
                "periodo_uuid periodoId, mes_facturacion mesFacturacion, " +
                "lectura_actual lecturaActual, lectura_anterior lecturaAnterior, " +
                "constante, valor " +
                "FROM core.v_info_consumos " +
                "WHERE servicio_uuid = @servicio_id " +
                "ORDER BY servicio, lectura_actual";

            var resultado = await conexion
                .QueryAsync<Consumo>(sentenciaSQL, parametrosSentencia);

            if (resultado.Any())
                consumosAsociados = [.. resultado];

            return consumosAsociados;
        }

        public async Task<Servicio> GetByNameAsync(string servicio_nombre)
        {
            Servicio unServicio = new();

            var conexion = contextoDB
                .CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@servicio_nombre", servicio_nombre,
                                    DbType.String, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT DISTINCT uuid id, nombre, " +
                "unidad_medida unidadmedida " +
                "FROM core.servicios " +
                "WHERE LOWER(nombre) = LOWER(@servicio_nombre)";

            var resultado = await conexion
                .QueryAsync<Servicio>(sentenciaSQL, parametrosSentencia);

            if (resultado.Any())
                unServicio = resultado.First();

            return unServicio;
        }

        public async Task<int> GetTotalComponentsByServiceGuidAsync(Guid servicio_id)
        {
            var conexion = contextoDB.CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@servicio_id", servicio_id,
                                    DbType.Guid, ParameterDirection.Input);

            string sentenciaSQL = "SELECT COUNT(componente_uuid) totalRegistros " +
                "FROM core.v_info_componentes " +
                "WHERE servicio_uuid = @servicio_id";

            var totalRegistros = await conexion
                .QueryFirstAsync<int>(sentenciaSQL, parametrosSentencia);

            return totalRegistros;
        }

        public async Task<int> GetTotalConsumptionByServiceGuidAsync(Guid servicio_id)
        {
            var conexion = contextoDB.CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@servicio_id", servicio_id,
                                    DbType.Guid, ParameterDirection.Input);

            string sentenciaSQL = "SELECT COUNT(periodo_uuid) totalRegistros " +
                "FROM core.v_info_consumos " +
                "WHERE servicio_uuid = @servicio_id";

            var totalRegistros = await conexion
                .QueryFirstAsync<int>(sentenciaSQL, parametrosSentencia);

            return totalRegistros;
        }

        public async Task<bool> CreateAsync(Servicio unServicio)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_inserta_servicio";
                var parametros = new
                {
                    p_nombre = unServicio.Nombre,
                    p_unidad_medida = unServicio.UnidadMedida
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

        public async Task<bool> UpdateAsync(Servicio unServicio)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_actualiza_servicio";
                var parametros = new
                {
                    p_uuid = unServicio.Id,
                    p_nombre = unServicio.Nombre,
                    p_unidad_medida = unServicio.UnidadMedida
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

        public async Task<bool> RemoveAsync(Guid servicio_id)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_elimina_servicio";
                var parametros = new
                {
                    p_uuid = servicio_id
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
