using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models
{
    public class Servicio
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("id")]
        public string? Id { get; set; } = string.Empty;

        [BsonElement("nombre")]
        [JsonPropertyName("nombre")]
        [BsonRepresentation(BsonType.String)]
        public string? Nombre { get; set; } = null;

        [BsonElement("unidad_medida")]
        [JsonPropertyName("unidad_medida")]
        [BsonRepresentation(BsonType.String)]
        public string? UnidadMedida { get; set; } = null;

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var otroServicio = (Servicio)obj;

            return Id == otroServicio.Id
                && Nombre!.Equals(otroServicio.Nombre)
                && UnidadMedida!.Equals(otroServicio.UnidadMedida);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 3;
                hash = hash * 5 + (Id?.GetHashCode() ?? 0);
                hash = hash * 5 + (Nombre?.GetHashCode() ?? 0);
                hash = hash * 5 + (UnidadMedida?.GetHashCode() ?? 0);

                return hash;
            }
        }
    }
}
