using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Services
{
    public class PeriodoService(IPeriodoRepository periodoRepository)
    {
        private readonly IPeriodoRepository _periodoRepository = periodoRepository;

        public async Task<List<Periodo>> GetAllAsync()
        {
            return await _periodoRepository
                .GetAllAsync();
        }

        public async Task<Periodo> GetByIdAsync(string periodo_id)
        {
            Periodo unPeriodo = await _periodoRepository
                .GetByIdAsync(periodo_id);

            if (string.IsNullOrEmpty(unPeriodo.Id))
                throw new AppValidationException($"Periodo no encontrado con el Guid {periodo_id}");

            return unPeriodo;
        }

        public async Task<List<Consumo>> GetAssociatedConsumptionAsync(string periodo_id)
        {
            Periodo unPeriodo = await _periodoRepository
                .GetByIdAsync(periodo_id);

            if (string.IsNullOrEmpty(unPeriodo.Id))
                throw new AppValidationException($"Periodo no encontrado con el Guid {periodo_id}");

            var consumosAsociados = await _periodoRepository
                .GetAssociatedConsumptionAsync(periodo_id);

            if (consumosAsociados.Count == 0)
                throw new AppValidationException($"Periodo {unPeriodo.MesFacturacion} no tiene consumos asociados");

            return consumosAsociados;
        }

        public async Task<Periodo> CreateAsync(Periodo unPeriodo)
        {
            string resultadoValidacion = EvaluateTermDetailsAsync(unPeriodo);

            if (!string.IsNullOrEmpty(resultadoValidacion))
                throw new AppValidationException(resultadoValidacion);

            //Validamos primero si existe un periodo con esas fechas y mes de facturacion
            var periodoExistente = await _periodoRepository
                .GetByDatesAndBillingMonthAsync(unPeriodo);

            // Si existe, no se puede insertar
            if (!string.IsNullOrEmpty(periodoExistente.Id))
                throw new AppValidationException($"Ya existe un periodo para el mes de facturacion {unPeriodo.MesFacturacion} " +
                    $"pero con fechas {unPeriodo.FechaInicio} y {unPeriodo.FechaFinal}");

            //Si existe y los datos son iguales, se retorna el objeto para garantizar idempotencia
            if (periodoExistente.MesFacturacion == unPeriodo.MesFacturacion &&
                periodoExistente.FechaInicio == unPeriodo.FechaInicio &&
                periodoExistente.FechaFinal == unPeriodo.FechaFinal)
                return periodoExistente;

            try
            {
                bool resultadoAccion = await _periodoRepository
                    .CreateAsync(unPeriodo);

                if (!resultadoAccion)
                    throw new AppValidationException("Operación ejecutada pero no generó cambios en la DB");

                periodoExistente = await _periodoRepository
                    .GetByDatesAndBillingMonthAsync(unPeriodo);
            }
            catch (DbOperationException)
            {
                throw;
            }

            return periodoExistente;
        }

        public async Task<Periodo> UpdateAsync(Periodo unPeriodo)
        {
            string resultadoValidacion = EvaluateTermDetailsAsync(unPeriodo);

            if (!string.IsNullOrEmpty(resultadoValidacion))
                throw new AppValidationException(resultadoValidacion);

            //Primero, validamos si existe un periodo con ese Guid
            var periodoExistente = await _periodoRepository
                .GetByIdAsync(unPeriodo.Id!);

            if (string.IsNullOrEmpty(periodoExistente.Id))
                throw new AppValidationException($"No existe un periodo con el Guid {unPeriodo.Id} que se pueda actualizar");

            //Si existe y los datos son iguales, se retorna el objeto para garantizar idempotencia
            if (periodoExistente.Equals(unPeriodo))
                return periodoExistente;

            try
            {
                bool resultadoAccion = await _periodoRepository
                    .UpdateAsync(unPeriodo);

                if (!resultadoAccion)
                    throw new AppValidationException("Operación ejecutada pero no generó cambios en la DB");

                periodoExistente = await _periodoRepository
                    .GetByIdAsync(unPeriodo.Id!);
            }
            catch (DbOperationException)
            {
                throw;
            }

            return periodoExistente;
        }

        public async Task<string> RemoveAsync(string periodo_id)
        {
            Periodo unPeriodo = await _periodoRepository
                .GetByIdAsync(periodo_id);

            if (string.IsNullOrEmpty(unPeriodo.Id))
                throw new AppValidationException($"Periodo no encontrado con el id {periodo_id}");

            //TODO: Habilitar consulta de costos de componentes por periodo

            //var totalCostosComponentePeriodo = await _periodoRepository
            //    .GetTotalComponentCostsByTermGuidAsync(periodo_id);

            //if (totalCostosComponentePeriodo > 0)
            //    throw new AppValidationException($"No se puede eliminar {unPeriodo.MesFacturacion} porque " +
            //        $"tiene {totalCostosComponentePeriodo} costos de componentes asociados");

            var totalConsumosPorPeriodo = await _periodoRepository
                .GetTotalConsumptionByTermGuidAsync(periodo_id);

            if (totalConsumosPorPeriodo > 0)
                throw new AppValidationException($"No se puede eliminar {unPeriodo.MesFacturacion} porque " +
                    $"tiene consumos en {totalConsumosPorPeriodo} asociados");

            string mesFacturacionEliminado = unPeriodo.MesFacturacion!;

            try
            {
                bool resultadoAccion = await _periodoRepository
                    .RemoveAsync(periodo_id);

                if (!resultadoAccion)
                    throw new DbOperationException("Operación ejecutada pero no generó cambios en la DB");
            }
            catch (DbOperationException)
            {
                throw;
            }

            return mesFacturacionEliminado;
        }

        private static string EvaluateTermDetailsAsync(Periodo unPeriodo)
        {
            if (unPeriodo.FechaInicio!.Length == 0)
                return "No se puede insertar un periodo con fecha inicial nula";

            if (unPeriodo.FechaFinal!.Length == 0)
                return "No se puede insertar un periodo con fecha final nula";

            if (unPeriodo.MesFacturacion!.Length == 0)
                return "No se puede insertar un periodo con mes de facturacion nulo";

            if (unPeriodo.TotalDias <= 0)
                return "No se puede insertar un periodo con duración en días menor o igual a cero";

            return string.Empty;
        }
    }
}
