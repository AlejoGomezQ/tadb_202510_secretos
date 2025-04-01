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
                hash = hash * 5 + Id.GetHashCode();
                hash = hash * 5 + (Nombre?.GetHashCode() ?? 0);
                hash = hash * 5 + (UnidadMedida?.GetHashCode() ?? 0);

                return hash;
            }
        }
    }
}
