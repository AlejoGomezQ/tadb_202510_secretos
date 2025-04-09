using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Repositories
{
    public class ResumenRepository(MongoDbContext unContexto) : IResumenRepository
    {
        private readonly MongoDbContext contextoDB = unContexto;

        public async Task<Resumen> GetAllAsync()
        {
            Resumen unResumen = new();

            var conexion = contextoDB.CreateConnection();

            string sentenciaSQL = "SELECT COUNT(id) total FROM core.periodos";
            unResumen.Periodos = await conexion
                .QueryFirstAsync<int>(sentenciaSQL, new DynamicParameters());

            sentenciaSQL = "SELECT COUNT(id) total FROM core.servicios";
            unResumen.Servicios = await conexion
                .QueryFirstAsync<int>(sentenciaSQL, new DynamicParameters());

            sentenciaSQL = "SELECT COUNT(id) total FROM core.componentes";
            unResumen.Componentes = await conexion
                .QueryFirstAsync<int>(sentenciaSQL, new DynamicParameters());

            sentenciaSQL = "SELECT COUNT(id) total FROM core.departamentos";
            unResumen.Departamentos = await conexion
                .QueryFirstAsync<int>(sentenciaSQL, new DynamicParameters());

            sentenciaSQL = "SELECT COUNT(id) total FROM core.municipios";
            unResumen.Municipios = await conexion
                .QueryFirstAsync<int>(sentenciaSQL, new DynamicParameters());

            return unResumen;
        }
    }
}