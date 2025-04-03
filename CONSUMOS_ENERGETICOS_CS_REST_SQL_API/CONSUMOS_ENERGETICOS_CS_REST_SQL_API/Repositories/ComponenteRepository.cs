using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using Dapper;
using Npgsql;
using System.Data;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Repositories
{
    public class ComponenteRepository(PgsqlDbContext unContexto) : IComponenteRepository
    {
        private readonly PgsqlDbContext contextoDB = unContexto;

        public async Task<List<Componente>> GetAllAsync()
        {
            var conexion = contextoDB
                .CreateConnection();

            string sentenciaSQL =
                "SELECT DISTINCT componente_uuid id, componente nombre, servicio, " +
                "servicio_uuid servicioId " +
                "FROM core.v_info_componentes " +
                "ORDER BY servicio, nombre";

            var resultadoComponentes = await conexion
                .QueryAsync<Componente>(sentenciaSQL, new DynamicParameters());

            return [.. resultadoComponentes];
        }

        public async Task<Componente> GetByGuidAsync(Guid componente_id)
        {
            Componente unComponente = new();

            var conexion = contextoDB
                .CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@componente_id", componente_id,
                                    DbType.Guid, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT DISTINCT componente_uuid id, componente nombre, servicio, " +
                "servicio_uuid servicioId " +
                "FROM core.v_info_componentes " +
                "WHERE componente_uuid = @componente_id";

            var resultado = await conexion.QueryAsync<Componente>(sentenciaSQL,
                parametrosSentencia);

            if (resultado.Any())
                unComponente = resultado.First();

            return unComponente;
        }


        public async Task<Componente> GetByNameAndServiceAsync(string componente_nombre, string componente_servicio)
        {
            Componente unComponente = new();

            var conexion = contextoDB
                .CreateConnection();

            DynamicParameters parametrosSentencia = new();
            parametrosSentencia.Add("@componente_nombre", componente_nombre,
                                    DbType.String, ParameterDirection.Input);
            parametrosSentencia.Add("@componente_servicio", componente_servicio,
                                    DbType.String, ParameterDirection.Input);

            string sentenciaSQL =
                "SELECT DISTINCT componente_uuid id, componente nombre, servicio, " +
                "servicio_uuid servicioId " +
                "FROM core.v_info_componentes " +
                "WHERE componente = @componente_nombre " +
                "AND servicio = @componente_servicio";

            var resultado = await conexion.QueryAsync<Componente>(sentenciaSQL,
                parametrosSentencia);

            if (resultado.Any())
                unComponente = resultado.First();

            return unComponente;
        }

        //TODO: Crear el método para obtener - Componentes por Periodo
        public async Task<bool> CreateAsync(Componente unComponente)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_inserta_componente";
                var parametros = new
                {
                    p_nombre = unComponente.Nombre,
                    p_servicio = unComponente.Servicio
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

        public async Task<bool> UpdateAsync(Componente unComponente)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_actualiza_componente";
                var parametros = new
                {
                    p_uuid = unComponente.Id,
                    p_nombre = unComponente.Nombre,
                    p_servicio = unComponente.Servicio
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

        public async Task<bool> RemoveAsync(Guid componente_id)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB.CreateConnection();

                string procedimiento = "core.p_elimina_componente";
                var parametros = new
                {
                    p_uuid = componente_id
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