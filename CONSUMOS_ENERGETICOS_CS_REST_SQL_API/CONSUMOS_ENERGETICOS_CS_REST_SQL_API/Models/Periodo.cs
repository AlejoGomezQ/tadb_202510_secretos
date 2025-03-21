using System.Text.Json.Serialization;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models
{
    public class Periodo
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; } = Guid.Empty;

        [JsonPropertyName("fecha_inicio")]
        public string? FechaInicio { get; set; } = null;

        [JsonPropertyName("fecha_final")]
        public string? FechaFinal { get; set; } = null;
        
        [JsonPropertyName("total_dias")]
        public int TotalDias { get; set; } = 0;

        [JsonPropertyName("mes_facturacion")]
        public string? MesFacturacion { get; set; } = null;

    }
}