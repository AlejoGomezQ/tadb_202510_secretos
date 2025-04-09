using System.Text.Json.Serialization;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models
{
    public class Consumo
    {
        [JsonPropertyName("servicio_id")]
        public Guid ServicioId { get; set; } = Guid.Empty;

        [JsonPropertyName("servicio")]
        public string? Servicio { get; set; } = null;

        [JsonPropertyName("periodo_id")]
        public Guid PeriodoId { get; set; } = Guid.Empty;

        [JsonPropertyName("mes_facturacion")]
        public string? MesFacturacion { get; set; } = null;

        [JsonPropertyName("lectura_actual")]
        public int LecturaActual { get; set; } = 0;

        [JsonPropertyName("lectura_anterior")]
        public int LecturaAnterior { get; set; } = 0;

        [JsonPropertyName("constante")]
        public float Constante { get; set; } = 0;

        [JsonPropertyName("valor")]
        public float Valor { get; set; } = 0;
    }
}
