using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;
using MongoDB.Driver;


namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Repositories
{
    public class ServicioRepository(MongoDbContext unContexto) : IServicioRepository
    {
        private readonly MongoDbContext contextoDB = unContexto;

        public async Task<List<Servicio>> GetAllAsync()
        {
            var conexion = contextoDB
                .CreateConnection();

            var coleccionServicios = conexion
                .GetCollection<Servicio>(contextoDB.ConfiguracionColecciones.ColeccionServicios);

            var losServicios = await coleccionServicios
                .Find(_ => true)
                .SortBy(servicio => servicio.Nombre)
                .ToListAsync();

            return losServicios;
        }

        public async Task<Servicio> GetByIdAsync(string servicio_id)
        {
            Servicio unServicio = new();

            var conexion = contextoDB
                .CreateConnection();

            var coleccionServicios = conexion
                .GetCollection<Servicio>(contextoDB.ConfiguracionColecciones.ColeccionServicios);

            var resultado = await coleccionServicios
                .Find(servicio => servicio.Id == servicio_id)
                .FirstOrDefaultAsync();

            if (resultado is not null)
                unServicio = resultado;

            return unServicio;
        }

        public async Task<List<Componente>> GetAssociatedComponentsAsync(string servicio_id)
        {
            var conexion = contextoDB
                .CreateConnection();

            var coleccionComponentes = conexion
                .GetCollection<Componente>(contextoDB.ConfiguracionColecciones.ColeccionComponentes);

            var losComponentes = await coleccionComponentes
                .Find(componente => componente.ServicioId!.ToLower() == servicio_id.ToLower())
                .SortBy(componente => componente.Nombre)
                .ToListAsync();

            return losComponentes;
        }

        public async Task<List<Consumo>> GetAssociatedConsumptionAsync(string servicio_id)
        {
            var conexion = contextoDB
                .CreateConnection();

            var coleccionConsumos = conexion
                .GetCollection<Consumo>(contextoDB.ConfiguracionColecciones.VistaConsumos);

            var losConsumos = await coleccionConsumos
                .Find(consumo => consumo.ServicioId!.ToLower() == servicio_id.ToLower())
                .SortBy(consumo => consumo.MesFacturacion)
                .ToListAsync();

            return losConsumos;
        }

        public async Task<Servicio> GetByNameAsync(string servicio_nombre)
        {
            Servicio unServicio = new();

            var conexion = contextoDB
                .CreateConnection();

            var coleccionServicios = conexion
                .GetCollection<Servicio>(contextoDB.ConfiguracionColecciones.ColeccionServicios);

            var resultado = await coleccionServicios
                .Find(servicio => servicio.Nombre!.Equals(servicio_nombre, StringComparison.CurrentCultureIgnoreCase))
                .FirstOrDefaultAsync();

            if (resultado is not null)
                unServicio = resultado;

            return unServicio;
        }

        public async Task<int> GetTotalComponentsByServiceIdAsync(string servicio_id)
        {
            var losComponentes = await GetAssociatedComponentsAsync(servicio_id);
            return losComponentes.Count;
        }

        public async Task<int> GetTotalConsumptionByServiceIdAsync(string servicio_id)
        {
            var losConsumos = await GetAssociatedConsumptionAsync(servicio_id);
            return losConsumos.Count;
        }

        public async Task<bool> CreateAsync(Servicio unServicio)
        {
            bool resultadoAccion = false;

            var conexion = contextoDB
                .CreateConnection();

            var coleccionServicios = conexion
                .GetCollection<Servicio>(contextoDB.ConfiguracionColecciones.ColeccionServicios);

            await coleccionServicios
                .InsertOneAsync(unServicio);

            var resultado = await GetByNameAsync(unServicio.Nombre!);

            if (resultado is not null)
                resultadoAccion = true;

            return resultadoAccion;
        }

        public async Task<bool> UpdateAsync(Servicio unServicio)
        {
            bool resultadoAccion = false;

            var conexion = contextoDB
                .CreateConnection();

            var coleccionServicios = conexion
                .GetCollection<Servicio>(contextoDB.ConfiguracionColecciones.ColeccionServicios);

            var resultado = await coleccionServicios
                .ReplaceOneAsync(servicio => servicio.Id == unServicio.Id, unServicio);

            if (resultado.IsAcknowledged)
                resultadoAccion = true;

            return resultadoAccion;
        }

        public async Task<bool> RemoveAsync(string servicio_id)
        {
            bool resultadoAccion = false;

            var conexion = contextoDB
                .CreateConnection();

            var coleccionServicios = conexion
                .GetCollection<Servicio>(contextoDB.ConfiguracionColecciones.ColeccionServicios);

            var resultado = await coleccionServicios
                .DeleteOneAsync(servicio => servicio.Id == servicio_id);

            if (resultado.IsAcknowledged)
                resultadoAccion = true;

            return resultadoAccion;
        }
    }
}
