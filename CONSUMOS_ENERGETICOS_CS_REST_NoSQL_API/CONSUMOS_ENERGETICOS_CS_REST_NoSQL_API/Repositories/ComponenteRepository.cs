using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;
using MongoDB.Driver;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Repositories
{
    public class ComponenteRepository(MongoDbContext unContexto) : IComponenteRepository
    {
        private readonly MongoDbContext contextoDB = unContexto;

        public async Task<List<Componente>> GetAllAsync()
        {
            var conexion = contextoDB
                .CreateConnection();

            var coleccionComponentes = conexion
                .GetCollection<Componente>(contextoDB.ConfiguracionColecciones.ColeccionComponentes);

            var losComponentes = await coleccionComponentes
                .Find(_ => true)
                .SortBy(componente => componente.Nombre)
                .ToListAsync();

            return losComponentes;
        }

        public async Task<Componente> GetByIdAsync(string componente_id)
        {
            Componente unComponente = new();

            var conexion = contextoDB
                .CreateConnection();

            var coleccionComponentes = conexion
                .GetCollection<Componente>(contextoDB.ConfiguracionColecciones.ColeccionComponentes);

            var resultado = await coleccionComponentes
                .Find(componente => componente.Id == componente_id)
                .FirstOrDefaultAsync();

            if (resultado is not null)
                unComponente = resultado;

            return unComponente;
        }

        public async Task<Componente> GetByNameAndServiceAsync(string componente_nombre, string componente_servicio)
        {
            Componente unComponente = new();

            var conexion = contextoDB
                .CreateConnection();

            var coleccionComponentes = conexion
                .GetCollection<Componente>(contextoDB.ConfiguracionColecciones.ColeccionComponentes);

            var builder = Builders<Componente>.Filter;
            var filtro = builder.And(
                builder.Eq(componente => componente.Nombre, componente_nombre),
                builder.Eq(componente => componente.Servicio, componente_servicio));

            var resultado = await coleccionComponentes
                .Find(filtro)
                .FirstOrDefaultAsync();

            if (resultado is not null)
                unComponente = resultado;

            return unComponente;
        }

        //TODO: Crear el método para obtener - Componentes por Periodo

        public async Task<bool> CreateAsync(Componente unComponente)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB
                .CreateConnection();

                var coleccionComponentes = conexion
                    .GetCollection<Componente>(contextoDB.ConfiguracionColecciones.ColeccionComponentes);

                await coleccionComponentes
                    .InsertOneAsync(unComponente);

                var resultado = await GetByNameAndServiceAsync(unComponente.Nombre!, unComponente.Servicio!);

                if (resultado is not null)
                    resultadoAccion = true;
            }
            catch (MongoWriteException error)
            {
                throw new DbOperationException($"Fallo al grabar el componente. {error.Message}");
            }

            return resultadoAccion;
        }

        public async Task<bool> UpdateAsync(Componente unComponente)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB
                    .CreateConnection();

                var coleccionComponentes = conexion
                    .GetCollection<Componente>(contextoDB.ConfiguracionColecciones.ColeccionComponentes);

                var resultado = await coleccionComponentes
                    .ReplaceOneAsync(componente => componente.Id == unComponente.Id, unComponente);

                if (resultado.IsAcknowledged)
                    resultadoAccion = true;
            }
            catch (MongoWriteException error)
            {
                throw new DbOperationException($"Fallo al grabar el componente. {error.Message}");
            }

            return resultadoAccion;
        }

        public async Task<bool> RemoveAsync(string componente_id)
        {
            bool resultadoAccion = false;

            try
            {
                var conexion = contextoDB
                    .CreateConnection();

                var coleccionComponentes = conexion
                    .GetCollection<Componente>(contextoDB.ConfiguracionColecciones.ColeccionComponentes);

                var resultado = await coleccionComponentes
                    .DeleteOneAsync(componente => componente.Id == componente_id);

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