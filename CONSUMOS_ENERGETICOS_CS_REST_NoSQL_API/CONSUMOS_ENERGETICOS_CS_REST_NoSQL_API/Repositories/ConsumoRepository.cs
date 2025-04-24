using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;
using MongoDB.Driver;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Repositories
{
    public class ConsumoRepository(MongoDbContext unContexto) : IConsumoRepository
    {
        private readonly MongoDbContext contextoDB = unContexto;

        public async Task<List<Consumo>> GetAllAsync()
        {
            var conexion = contextoDB
                .CreateConnection();

            var coleccionConsumos = conexion
                .GetCollection<Consumo>(contextoDB.ConfiguracionColecciones.ColeccionConsumos);

            var losConsumos = await coleccionConsumos
                .Find(_ => true)
                .SortBy(consumo => consumo.Servicio)
                .ToListAsync();

            return losConsumos;
        }

        public async Task<Consumo> GetByBillingMonthAndServiceAsync(string mes_facturacion, string servicio_nombre)
        {
            Consumo unConsumo = new();

            var conexion = contextoDB
                .CreateConnection();

            var coleccionConsumos = conexion
                .GetCollection<Consumo>(contextoDB.ConfiguracionColecciones.ColeccionConsumos);

            var builder = Builders<Consumo>.Filter;
            var filtro = builder.And(
                builder.Eq(consumo => consumo.MesFacturacion, mes_facturacion),
                builder.Eq(consumo => consumo.Servicio, servicio_nombre));

            var resultado = await coleccionConsumos
                .Find(filtro)
                .FirstOrDefaultAsync();

            if (resultado is not null)
                unConsumo = resultado;

            return unConsumo;
        }

        public async Task<bool> CreateAsync(Consumo unConsumo)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB
                    .CreateConnection();

                var coleccionConsumos = conexion
                    .GetCollection<Consumo>(contextoDB.ConfiguracionColecciones.ColeccionConsumos);

                await coleccionConsumos
                    .InsertOneAsync(unConsumo);

                var resultado = await GetByBillingMonthAndServiceAsync(unConsumo.MesFacturacion!, unConsumo.Servicio!);

                if (resultado is not null)
                    resultadoAccion = true;
            }
            catch (MongoWriteException error)
            {
                throw new DbOperationException($"Fallo al grabar el componente. {error.Message}");
            }

            return resultadoAccion;
        }

        public async Task<bool> UpdateAsync(Consumo unConsumo)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB
                    .CreateConnection();

                var coleccionConsumos = conexion
                    .GetCollection<Consumo>(contextoDB.ConfiguracionColecciones.ColeccionConsumos);

                var resultado = await coleccionConsumos
                    .ReplaceOneAsync(consumo => consumo.Id == unConsumo.Id, unConsumo);

                if (resultado.IsAcknowledged)
                    resultadoAccion = true;

            }
            catch (MongoWriteException error)
            {
                throw new DbOperationException($"Fallo al grabar el componente. {error.Message}");
            }

            return resultadoAccion;
        }

        public async Task<bool> RemoveAsync(Consumo unConsumo)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB
                    .CreateConnection();

                var coleccionConsumos = conexion
                    .GetCollection<Consumo>(contextoDB.ConfiguracionColecciones.ColeccionConsumos);

                var resultado = await coleccionConsumos
                    .DeleteOneAsync(consumo => consumo.Id == unConsumo.Id);

                if (resultado.IsAcknowledged)
                    resultadoAccion = true;
            }
            catch (MongoWriteException error)
            {
                throw new DbOperationException($"Fallo al grabar el componente. {error.Message}");
            }

            return resultadoAccion;
        }
    }
}