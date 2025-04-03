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
                hash = hash * 5 + Id.GetHashCode();
                hash = hash * 5 + (Nombre?.GetHashCode() ?? 0);
                hash = hash * 5 + (Servicio?.GetHashCode() ?? 0);

                return hash;
            }
        }
    }
}
