using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;
using MongoDB.Driver;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Repositories
{
    public class PeriodoRepository(MongoDbContext unContexto) : IPeriodoRepository
    {
        private readonly MongoDbContext contextoDB = unContexto;

        public async Task<List<Periodo>> GetAllAsync()
        {
            var conexion = contextoDB
                .CreateConnection();

            var coleccionPeriodos = conexion
                .GetCollection<Periodo>(contextoDB.ConfiguracionColecciones.ColeccionPeriodos);

            var losPeriodos = await coleccionPeriodos
                .Find(_ => true)
                .SortBy(periodo => periodo.MesFacturacion)
                .ToListAsync();

            return losPeriodos;
        }

        public async Task<Periodo> GetByIdAsync(string periodo_id)
        {
            Periodo unPeriodo = new();

            var conexion = contextoDB
                .CreateConnection();

            var coleccionPeriodos = conexion
                .GetCollection<Periodo>(contextoDB.ConfiguracionColecciones.ColeccionPeriodos);

            var resultado = await coleccionPeriodos
                .Find(periodo => periodo.Id == periodo_id)
                .FirstOrDefaultAsync();

            if (resultado is not null)
                unPeriodo = resultado;

            return unPeriodo;
        }

        public async Task<Periodo> GetByBillingMonthAsync(string periodo_mes_facturacion)
        {
            Periodo unPeriodo = new();

            var conexion = contextoDB
                .CreateConnection();

            var coleccionPeriodos = conexion
                .GetCollection<Periodo>(contextoDB.ConfiguracionColecciones.ColeccionPeriodos);

            var resultado = await coleccionPeriodos
                .Find(periodo => periodo.MesFacturacion!.Equals(periodo_mes_facturacion, StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefaultAsync();

            if (resultado is not null)
                unPeriodo = resultado;

            return unPeriodo;
        }

        public async Task<Periodo> GetByDatesAndBillingMonthAsync(Periodo unPeriodo)
        {
            Periodo periodoExistente = new();

            var conexion = contextoDB
                .CreateConnection();

            var coleccionPeriodos = conexion
                .GetCollection<Periodo>(contextoDB.ConfiguracionColecciones.ColeccionPeriodos);

            var builder = Builders<Periodo>.Filter;
            var filtro = builder.And(
                builder.Eq(periodo => periodo.FechaInicio, unPeriodo.FechaInicio),
                builder.Eq(periodo => periodo.FechaFinal, unPeriodo.FechaFinal),
                builder.Eq(periodo => periodo.MesFacturacion, unPeriodo.MesFacturacion));

            var resultado = await coleccionPeriodos
                .Find(filtro)
                .FirstOrDefaultAsync();

            if (resultado is not null)
                periodoExistente = resultado;

            return periodoExistente;
        }

        public async Task<List<Consumo>> GetAssociatedConsumptionAsync(string periodo_id)
        {
            var conexion = contextoDB
                .CreateConnection();

            var coleccionConsumos = conexion
                .GetCollection<Consumo>(contextoDB.ConfiguracionColecciones.VistaConsumos);

            var losConsumos = await coleccionConsumos
                .Find(consumo => consumo.PeriodoId!.ToLower() == periodo_id.ToLower())
                .SortBy(consumo => consumo.Servicio)
                .ToListAsync();

            return losConsumos;
        }

        //public async Task<int> GetTotalComponentCostsByTermGuidAsync(string periodo_id)
        //{
        //    var conexion = contextoDB.CreateConnection();

        //    DynamicParameters parametrosSentencia = new();
        //    parametrosSentencia.Add("@periodo_id", periodo_id,
        //                            DbType.Guid, ParameterDirection.Input);

        //    string sentenciaSQL = "SELECT COUNT(componente_uuid) totalRegistros " +
        //        "FROM core.v_info_costos_componentes " +
        //        "WHERE periodo_uuid = @periodo_id";

        //    var totalRegistros = await conexion
        //        .QueryFirstAsync<int>(sentenciaSQL, parametrosSentencia);

        //    return totalRegistros;
        //}

        public async Task<int> GetTotalConsumptionByTermGuidAsync(string periodo_id)
        {
            var losConsumos = await GetAssociatedConsumptionAsync(periodo_id);

            return losConsumos.Count;
        }

        public async Task<bool> CreateAsync(Periodo unPeriodo)
        {
            bool resultadoAccion = false;

            var conexion = contextoDB
                .CreateConnection();

            var coleccionPeriodos = conexion
                .GetCollection<Periodo>(contextoDB.ConfiguracionColecciones.ColeccionPeriodos);

            await coleccionPeriodos
                .InsertOneAsync(unPeriodo);

            var resultado = await GetByBillingMonthAsync(unPeriodo.MesFacturacion!);

            if (resultado is not null)
                resultadoAccion = true;

            return resultadoAccion;
        }

        public async Task<bool> UpdateAsync(Periodo unPeriodo)
        {
            bool resultadoAccion = false;

            var conexion = contextoDB
                .CreateConnection();

            var coleccionPeriodos = conexion
                .GetCollection<Periodo>(contextoDB.ConfiguracionColecciones.ColeccionPeriodos);

            var resultado = await coleccionPeriodos
                .ReplaceOneAsync(periodo => periodo.Id == unPeriodo.Id, unPeriodo);

            if (resultado.IsAcknowledged)
                resultadoAccion = true;

            return resultadoAccion;
        }

        public async Task<bool> RemoveAsync(string periodo_id)
        {
            bool resultadoAccion = false;

            var conexion = contextoDB
                .CreateConnection();

            var coleccionPeriodos = conexion
                .GetCollection<Periodo>(contextoDB.ConfiguracionColecciones.ColeccionPeriodos);

            var resultado = await coleccionPeriodos
                .DeleteOneAsync(periodo => periodo.Id == periodo_id);

            if (resultado.IsAcknowledged)
                resultadoAccion = true;

            return resultadoAccion;
        }
    }
}
