using Npgsql;
using System.Data;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.DbContexts
{
    public class PgsqlDbContext(IConfiguration unaConfiguracion)
    {
        private readonly string cadenaConexion = unaConfiguracion.GetConnectionString("ConsumosPL")!;

        public IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(cadenaConexion);
        }
    }
}