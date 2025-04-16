using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models
{
    public class Periodo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string? Id { get; set; } = string.Empty;


        [BsonElement("fecha_inicio")]
        [JsonPropertyName("fecha_inicio")]
        [BsonRepresentation(BsonType.String)]
        public string? FechaInicio { get; set; } = null;

        [BsonElement("fecha_final")]
        [JsonPropertyName("fecha_final")]
        [BsonRepresentation(BsonType.String)]
        public string? FechaFinal { get; set; } = null;

        [BsonElement("total_dias")]
        [JsonPropertyName("total_dias")]
        [BsonRepresentation(BsonType.Int32)]
        public int TotalDias { get; set; } = 0;

        [BsonElement("mes_facturacion")]
        [JsonPropertyName("mes_facturacion")]
        [BsonRepresentation(BsonType.String)]
        public string? MesFacturacion { get; set; } = null;

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var otroPeriodo = (Periodo)obj;

            return Id == otroPeriodo.Id
                && FechaInicio!.Equals(otroPeriodo.FechaInicio)
                && FechaFinal!.Equals(otroPeriodo.FechaFinal)
                && MesFacturacion!.Equals(otroPeriodo.MesFacturacion);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 3;
                hash = hash * 5 + (Id?.GetHashCode() ?? 0);
                hash = hash * 5 + (FechaInicio?.GetHashCode() ?? 0);
                hash = hash * 5 + (FechaFinal?.GetHashCode() ?? 0);
                hash = hash * 5 + (MesFacturacion?.GetHashCode() ?? 0);

                return hash;
            }
        }
    }
}