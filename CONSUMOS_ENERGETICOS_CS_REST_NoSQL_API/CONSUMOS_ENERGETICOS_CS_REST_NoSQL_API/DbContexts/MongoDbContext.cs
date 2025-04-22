using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;
using MongoDB.Driver;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.DbContexts
{
    public class MongoDbContext(IConfiguration unaConfiguracion)
    {
        private readonly ConsumosEnergeticosDatabaseSettings _consumosEnergeticosDatabaseSettings = new(unaConfiguracion);

        public IMongoDatabase CreateConnection()
        {
            var configuracion = unaConfiguracion.GetSection("ConsumosEnergeticosDatabaseSettings");

            var baseDeDatos = configuracion.GetSection("BaseDeDatos").Value!;
            var usuario = configuracion.GetSection("Usuario").Value!;
            var password = configuracion.GetSection("PassWord").Value!;
            var servidor = configuracion.GetSection("Servidor").Value!;
            var puerto = configuracion.GetSection("Puerto").Value!;
            var mecanismoAutenticacion = configuracion.GetSection("MecanismoAutenticacion").Value!;

            var cadenaConexion = $"mongodb://{usuario}:{password}@{servidor}:{puerto}/{baseDeDatos}?authMechanism={mecanismoAutenticacion}";

            var clienteDB = new MongoClient(cadenaConexion);
            var miDB = clienteDB.GetDatabase(_consumosEnergeticosDatabaseSettings.BaseDeDatos);

            return miDB;
        }

        public ConsumosEnergeticosDatabaseSettings ConfiguracionColecciones
        {
            get { return _consumosEnergeticosDatabaseSettings; }
        }
    }
}