using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models
{
    public class Consumo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string? Id { get; set; } = string.Empty;

        [BsonElement("servicio_id")]
        [JsonPropertyName("servicio_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? ServicioId { get; set; } = string.Empty;

        [BsonElement("servicio")]
        [JsonPropertyName("servicio")]
        [BsonRepresentation(BsonType.String)]
        public string? Servicio { get; set; } = null;

        [BsonElement("periodo_id")]
        [JsonPropertyName("periodo_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? PeriodoId { get; set; } = string.Empty;

        [BsonElement("mes_facturacion")]
        [JsonPropertyName("mes_facturacion")]
        [BsonRepresentation(BsonType.String)]
        public string? MesFacturacion { get; set; } = null;

        [BsonElement("lectura_actual")]
        [JsonPropertyName("lectura_actual")]
        [BsonRepresentation(BsonType.Int32)]
        public int LecturaActual { get; set; } = 0;

        [BsonElement("lectura_anterior")]
        [JsonPropertyName("lectura_anterior")]
        [BsonRepresentation(BsonType.Int32)]
        public int LecturaAnterior { get; set; } = 0;

        [BsonElement("constante")]
        [JsonPropertyName("constante")]
        [BsonRepresentation(BsonType.Double)]
        public double Constante { get; set; } = 0;

        [BsonElement("valor")]
        [JsonPropertyName("valor")]
        [BsonRepresentation(BsonType.Double)]
        public double Valor { get; set; } = 0;
    }
}
