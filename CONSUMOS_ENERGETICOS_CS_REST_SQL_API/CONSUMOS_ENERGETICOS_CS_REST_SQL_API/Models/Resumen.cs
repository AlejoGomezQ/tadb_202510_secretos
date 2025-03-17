using System.Text.Json.Serialization;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models
{
    public class Resumen
    {
        [JsonPropertyName("periodos")]
        public int Periodos { get; set; } = 0;

        [JsonPropertyName("servicios")]
        public int Servicios { get; set; } = 0;

        [JsonPropertyName("componentes")]
        public int Componentes { get; set; } = 0;
    }
}