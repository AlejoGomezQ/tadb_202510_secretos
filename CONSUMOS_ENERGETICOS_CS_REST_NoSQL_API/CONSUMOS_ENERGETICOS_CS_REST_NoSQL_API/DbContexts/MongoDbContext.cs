using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;
using MongoDB.Driver;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.DbContexts
{
    public class MongoDbContext(IConfiguration unaConfiguracion)
    {
        private readonly string cadenaConexion = unaConfiguracion.GetConnectionString("Mongo")!;
        private readonly ConsumosEnergeticosDatabaseSettings _michisDatabaseSettings = new(unaConfiguracion);

        public IMongoDatabase CreateConnection()
        {
            var clienteDB = new MongoClient(cadenaConexion);
            var miDB = clienteDB.GetDatabase(_michisDatabaseSettings.DatabaseName);

            return miDB;
        }

        public ConsumosEnergeticosDatabaseSettings ConfiguracionColecciones
        {
            get { return _michisDatabaseSettings; }
        }
    }
}