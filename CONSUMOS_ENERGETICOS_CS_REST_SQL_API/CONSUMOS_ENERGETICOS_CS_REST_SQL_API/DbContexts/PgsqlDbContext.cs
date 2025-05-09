using Npgsql;
using System.Data;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.DbContexts
{
    public class PgsqlDbContext(IConfiguration unaConfiguracion)
    {
        // private readonly string cadenaConexion = unaConfiguracion.GetConnectionString("ConsumosPL")!;

        public IDbConnection CreateConnection()
        {
            SecretClientOptions options = new SecretClientOptions()
            {
                Retry =
                {
                    Delay= TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                }
            };
            var client = new SecretClient(new Uri("https://consumosenergetico.vault.azure.net/"), new DefaultAzureCredential(), options);

            KeyVaultSecret secret = client.GetSecret("conectionString");

            string secretValue = secret.Value;

            return new NpgsqlConnection(secretValue);
        }
    }
}