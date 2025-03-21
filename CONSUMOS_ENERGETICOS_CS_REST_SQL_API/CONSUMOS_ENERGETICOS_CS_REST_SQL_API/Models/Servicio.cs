using System.Text.Json.Serialization;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models
{
    public class Servicio
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonPropertyName("nombre")]
        public string? Nombre { get; set; } = null;

        [JsonPropertyName("unidad_medida")]
        public string? UnidadMedida { get; set; } = null;


    }
}
