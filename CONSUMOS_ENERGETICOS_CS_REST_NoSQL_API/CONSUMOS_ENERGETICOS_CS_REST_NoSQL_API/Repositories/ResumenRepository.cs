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

            //Total Periodos
            var coleccionPeriodos = conexion.GetCollection<Periodo>(contextoDB.ConfiguracionColecciones.ColeccionPeriodos);
            var totalPeriodos = await coleccionPeriodos
                .EstimatedDocumentCountAsync();

            unResumen.Periodos = totalPeriodos;

            //Total Servicios
            var coleccionServicios = conexion.GetCollection<Servicio>(contextoDB.ConfiguracionColecciones.ColeccionServicios);
            var totalServicios = await coleccionServicios
                .EstimatedDocumentCountAsync();

            unResumen.Servicios = totalServicios;

            //Total Componentes
            var coleccionComponentes = conexion.GetCollection<Componente>(contextoDB.ConfiguracionColecciones.ColeccionComponentes);
            var totalComponentes = await coleccionComponentes
                .EstimatedDocumentCountAsync();

            unResumen.Componentes = totalComponentes;


            //Total Consumos
            var coleccionConsumos = conexion.GetCollection<Consumo>(contextoDB.ConfiguracionColecciones.ColeccionConsumos);
            var totalConsumos = await coleccionConsumos
                .EstimatedDocumentCountAsync();

            unResumen.Consumos = totalConsumos;

            //sentenciaSQL = "SELECT COUNT(id) total FROM core.departamentos";
            //unResumen.Departamentos = await conexion
            //    .QueryFirstAsync<int>(sentenciaSQL, new DynamicParameters());


            //sentenciaSQL = "SELECT COUNT(id) total FROM core.municipios";
            //unResumen.Municipios = await conexion
            //    .QueryFirstAsync<int>(sentenciaSQL, new DynamicParameters());

            return unResumen;
        }
    }
}