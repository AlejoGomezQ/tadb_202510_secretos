using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using Dapper;
using Npgsql;
using System.Data;
using System.Security.Claims;

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
                componentesAsociados = resultado.ToList();

            return componentesAsociados;
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
                .QueryAsync<int>(sentenciaSQL, parametrosSentencia);

            return totalRegistros.FirstOrDefault();
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
                .QueryAsync<int>(sentenciaSQL, parametrosSentencia);

            return totalRegistros.FirstOrDefault();
        }

        public async Task<bool> CreateAsync(Servicio unServicio)
        {
            bool resultadoAccion = false;

            //Validamos primero si existe con ese nombre y con otra unidad de medida
            var servicioExistente = await GetByNameAsync(unServicio.Nombre!);

            // Si existe, pero con otra unidad de medida, no se puede insertar
            if (servicioExistente.Id != Guid.Empty && servicioExistente.UnidadMedida != unServicio.UnidadMedida)
                throw new AppValidationException($"Ya existe un servicio {unServicio.Nombre} " +
                    $"pero con unidad de medida {servicioExistente.UnidadMedida}");

            //Si existe y los datos son iguales, se retorna el objeto para garantizar idempotencia
            if (servicioExistente.Nombre == unServicio.Nombre && servicioExistente.UnidadMedida == unServicio.UnidadMedida)
                return true;

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

            var servicioExistente = await GetByGuidAsync(unServicio.Id);

            if (servicioExistente.Id == Guid.Empty)
                throw new DbOperationException($"No se puede actualizar. No existe un servicio con Guid {unServicio.Id}.");

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

            var climaExistente = await GetByGuidAsync(servicio_id);

            if (climaExistente.Id == Guid.Empty)
                throw new DbOperationException($"No se puede eliminar. No existe el servicio con el Id {servicio_id}.");

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
