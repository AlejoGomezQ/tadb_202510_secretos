using System.Text.Json.Serialization;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models
{
    public class Resumen
    {
        [JsonPropertyName("periodos")]
        public long Periodos { get; set; } = 0;

        [JsonPropertyName("servicios")]
        public long Servicios { get; set; } = 0;

        [JsonPropertyName("componentes")]
        public long Componentes { get; set; } = 0;

        [JsonPropertyName("consumos")]
        public long Consumos { get; set; } = 0;
    }
}