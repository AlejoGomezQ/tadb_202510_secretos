using System.Text.Json.Serialization;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models
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
                hash = hash * 5 + Id.GetHashCode();
                hash = hash * 5 + (FechaInicio?.GetHashCode() ?? 0);
                hash = hash * 5 + (FechaFinal?.GetHashCode() ?? 0);
                hash = hash * 5 + (MesFacturacion?.GetHashCode() ?? 0);

                return hash;
            }
        }
    }
}