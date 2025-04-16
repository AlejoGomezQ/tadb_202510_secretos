using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Services
{
    public class ConsumoService(IConsumoRepository consumoRepository,
                                IPeriodoRepository periodoRepository,
                                IServicioRepository servicioRepository)
    {
        private readonly IConsumoRepository _consumoRepository = consumoRepository;
        private readonly IPeriodoRepository _periodoRepository = periodoRepository;
        private readonly IServicioRepository _servicioRepository = servicioRepository;

        public async Task<List<Consumo>> GetAllAsync()
        {
            return await _consumoRepository
                .GetAllAsync();
        }

        public async Task<Consumo> CreateAsync(Consumo unConsumo)
        {
            string resultadoValidacion = EvaluateConsumptionDetailsAsync(unConsumo);

            if (!string.IsNullOrEmpty(resultadoValidacion))
                throw new AppValidationException(resultadoValidacion);

            //Validamos primero si existe un periodo con ese mes de facturacion
            var periodoExistente = await _periodoRepository
                .GetByBillingMonthAsync(unConsumo.MesFacturacion!);

            //Si no existe, no se puede registrar consumo
            if (string.IsNullOrEmpty(periodoExistente.Id))
                throw new AppValidationException($"No existe un periodo para el mes de facturacion {unConsumo.MesFacturacion}.");

            //Validamos si existe un servicio con ese nombre 
            var servicioExistente = await _servicioRepository
                .GetByNameAsync(unConsumo.Servicio!);

            //Si no existe, no se puede registrar consumo
            if (string.IsNullOrEmpty(servicioExistente.Id))
                throw new AppValidationException($"No existe un servicio {unConsumo.Servicio} para el cual registrar consumo.");

            //Si ya existe consumo para ese mes y servicio, se retorna el objeto para garantizar idempotencia
            var consumoExistente = await _consumoRepository
                .GetByBillingMonthAndServiceAsync(unConsumo.MesFacturacion!, unConsumo.Servicio!);

            if (!string.IsNullOrEmpty(consumoExistente.MesFacturacion) && !string.IsNullOrEmpty(consumoExistente.Servicio))
                return consumoExistente;

            //Se completa los datos necesarios para hacer la inserción
            unConsumo.ServicioId = servicioExistente.Id;
            unConsumo.PeriodoId = periodoExistente.Id;

            try
            {
                bool resultadoAccion = await _consumoRepository
                    .CreateAsync(unConsumo);

                if (!resultadoAccion)
                    throw new AppValidationException("Operación ejecutada pero no generó cambios en la DB");

                consumoExistente = await _consumoRepository
                .GetByBillingMonthAndServiceAsync(unConsumo.MesFacturacion!, unConsumo.Servicio!);
            }
            catch (DbOperationException)
            {
                throw;
            }

            return consumoExistente;
        }

        public async Task<Consumo> UpdateAsync(Consumo unConsumo)
        {
            string resultadoValidacion = EvaluateConsumptionDetailsAsync(unConsumo);

            if (!string.IsNullOrEmpty(resultadoValidacion))
                throw new AppValidationException(resultadoValidacion);

            //Validamos primero si existe un periodo con ese mes de facturacion
            var periodoExistente = await _periodoRepository
                .GetByBillingMonthAsync(unConsumo.MesFacturacion!);

            //Si no existe, no se puede registrar consumo
            if (string.IsNullOrEmpty(periodoExistente.Id))
                throw new AppValidationException($"No existe un periodo para el mes de facturacion {unConsumo.MesFacturacion}.");

            //Validamos si existe un servicio con ese nombre 
            var servicioExistente = await _servicioRepository
                .GetByNameAsync(unConsumo.Servicio!);

            //Si no existe, no se puede registrar consumo
            if (string.IsNullOrEmpty(servicioExistente.Id))
                throw new AppValidationException($"No existe un servicio {unConsumo.Servicio} para el cual registrar consumo.");

            //Si no existe consumo para ese mes y servicio, no se puede actualizar
            var consumoExistente = await _consumoRepository
                .GetByBillingMonthAndServiceAsync(unConsumo.MesFacturacion!, unConsumo.Servicio!);

            if (string.IsNullOrEmpty(consumoExistente.MesFacturacion) || string.IsNullOrEmpty(consumoExistente.Servicio))
                throw new AppValidationException($"No existe un consumo para  {unConsumo.Servicio} en {unConsumo.MesFacturacion} que actualizar.");

            //Se completa los datos necesarios para hacer la actualización
            unConsumo.ServicioId = servicioExistente.Id;
            unConsumo.PeriodoId = periodoExistente.Id;

            try
            {
                bool resultadoAccion = await _consumoRepository
                    .UpdateAsync(unConsumo);

                if (!resultadoAccion)
                    throw new AppValidationException("Operación ejecutada pero no generó cambios en la DB");

                consumoExistente = await _consumoRepository
                .GetByBillingMonthAndServiceAsync(unConsumo.MesFacturacion!, unConsumo.Servicio!);
            }
            catch (DbOperationException)
            {
                throw;
            }

            return consumoExistente;
        }

        public async Task<string> RemoveAsync(Consumo unConsumo)
        {
            //Validamos primero si existe un periodo con ese mes de facturacion
            var periodoExistente = await _periodoRepository
                .GetByBillingMonthAsync(unConsumo.MesFacturacion!);

            //Si no existe, no se puede registrar consumo
            if (string.IsNullOrEmpty(periodoExistente.Id))
                throw new AppValidationException($"No existe un periodo para el mes de facturacion {unConsumo.MesFacturacion}.");

            //Validamos si existe un servicio con ese nombre 
            var servicioExistente = await _servicioRepository
                .GetByNameAsync(unConsumo.Servicio!);

            //Si no existe, no se puede registrar consumo
            if (string.IsNullOrEmpty(servicioExistente.Id))
                throw new AppValidationException($"No existe un servicio {unConsumo.Servicio} para el cual registrar consumo.");

            //Si no existe consumo para ese mes y servicio, no se puede actualizar
            var consumoExistente = await _consumoRepository
                .GetByBillingMonthAndServiceAsync(unConsumo.MesFacturacion!, unConsumo.Servicio!);

            if (string.IsNullOrEmpty(consumoExistente.MesFacturacion) || string.IsNullOrEmpty(consumoExistente.Servicio))
                throw new AppValidationException($"No existe un consumo para  {unConsumo.Servicio} en {unConsumo.MesFacturacion} que actualizar.");


            string consumoEliminado = $"{unConsumo.Servicio} en {unConsumo.MesFacturacion}";

            //Se completa los datos necesarios para hacer la inserción
            unConsumo.ServicioId = servicioExistente.Id;
            unConsumo.PeriodoId = periodoExistente.Id;

            try
            {
                bool resultadoAccion = await _consumoRepository
                    .RemoveAsync(unConsumo);

                if (!resultadoAccion)
                    throw new DbOperationException("Operación ejecutada pero no generó cambios en la DB");
            }
            catch (DbOperationException)
            {
                throw;
            }

            return consumoEliminado;
        }

        private static string EvaluateConsumptionDetailsAsync(Consumo unConsumo)
        {
            if (unConsumo.MesFacturacion!.Length == 0)
                return "No se puede insertar un consumo para un dato de mes de facturación nulo";

            if (unConsumo.Servicio!.Length == 0)
                return "No se puede insertar un consumo para un dato de servicio nulo";

            if (unConsumo.LecturaActual <= 0 || unConsumo.LecturaAnterior <= 0)
                return "No se puede insertar un consumo con datos de lectura menores o iguales a cero";

            if (unConsumo.Constante <= 0)
                return "No se puede insertar un consumo con dato de constante menor o igual a cero";

            return string.Empty;
        }
    }
}