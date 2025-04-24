using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models
{
    public class Componente
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string? Id { get; set; } = string.Empty;

        [BsonElement("nombre")]
        [JsonPropertyName("nombre")]
        [BsonRepresentation(BsonType.String)]
        public string? Nombre { get; set; } = null;

        [BsonElement("servicio")]
        [JsonPropertyName("servicio")]
        [BsonRepresentation(BsonType.String)]
        public string? Servicio { get; set; } = null;

        [BsonElement("servicio_id")]
        [JsonPropertyName("servicio_id")]
        [BsonRepresentation(BsonType.String)]
        public string? ServicioId { get; set; } = string.Empty;
        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var otroComponente = (Componente)obj;

            return Id == otroComponente.Id
                && Nombre!.Equals(otroComponente.Nombre)
                && Servicio!.Equals(otroComponente.Servicio);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 3;
                hash = hash * 5 + (Id?.GetHashCode() ?? 0);
                hash = hash * 5 + (Nombre?.GetHashCode() ?? 0);
                hash = hash * 5 + (Servicio?.GetHashCode() ?? 0);

                return hash;
            }
        }
    }
}
