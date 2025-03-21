using System.Text.Json.Serialization;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models
{
    public class Componente
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; } = null;

        [JsonPropertyName("servicio")]
        public string? Servicio { get; set; } = null;

        [JsonPropertyName("servicio_id")]
        public Guid ServicioId { get; set; } = Guid.Empty;
    }
}
