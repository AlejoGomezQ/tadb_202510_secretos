using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Services
{
    public class ServicioService(IServicioRepository servicioRepository)
    {
        private readonly IServicioRepository _servicioRepository = servicioRepository;

        public async Task<List<Servicio>> GetAllAsync()
        {
            return await _servicioRepository
            .GetAllAsync();
        }

        public async Task<Servicio> GetByGuidAsync(Guid servicio_id)
        {
            Servicio unServicio = await _servicioRepository
                .GetByGuidAsync(servicio_id);

            if (unServicio.Id == Guid.Empty)
                throw new AppValidationException($"Servicio no encontrado con el ID {servicio_id}");

            return unServicio;
        }

        public async Task<List<Componente>> GetAssociatedComponentsAsync(Guid servicio_id)
        {
            Servicio unServicio = await _servicioRepository
                .GetByGuidAsync(servicio_id);

            if (unServicio.Id == Guid.Empty)
                throw new AppValidationException($"Servicio no encontrado con el id {servicio_id}");

            var componentesAsociados = await _servicioRepository
                .GetAssociatedComponentsAsync(servicio_id);

            if (componentesAsociados.Count == 0)
                throw new AppValidationException($"Servicio {unServicio.Nombre} no tiene componentes asociados");

            return componentesAsociados;
        }

        public async Task<List<Consumo>> GetAssociatedConsumptionAsync(Guid servicio_id)
        {
            Servicio unServicio = await _servicioRepository
                .GetByGuidAsync(servicio_id);

            if (unServicio.Id == Guid.Empty)
                throw new AppValidationException($"Servicio no encontrado con el id {servicio_id}");

            var consumosAsociados = await _servicioRepository
                .GetAssociatedConsumptionAsync(servicio_id);

            if (consumosAsociados.Count == 0)
                throw new AppValidationException($"Servicio {unServicio.Nombre} no tiene consumos asociados");

            return consumosAsociados;
        }

        public async Task<Servicio> CreateAsync(Servicio unServicio)
        {
            string resultadoValidacion = EvaluateServiceDetailsAsync(unServicio);

            if (!string.IsNullOrEmpty(resultadoValidacion))
                throw new AppValidationException(resultadoValidacion);

            //Validamos primero si existe con ese nombre y con otra unidad de medida
            var servicioExistente = await _servicioRepository
                .GetByNameAsync(unServicio.Nombre!);

            // Si existe, pero con otra unidad de medida, no se puede insertar
            if (servicioExistente.Id != Guid.Empty && servicioExistente.UnidadMedida != unServicio.UnidadMedida)
                throw new AppValidationException($"Ya existe un servicio {unServicio.Nombre} " +
                    $"pero con unidad de medida {servicioExistente.UnidadMedida}");

            //Si existe y los datos son iguales, se retorna el objeto para garantizar idempotencia
            if (servicioExistente.Nombre == unServicio.Nombre && servicioExistente.UnidadMedida == unServicio.UnidadMedida)
                return servicioExistente;

            try
            {
                bool resultadoAccion = await _servicioRepository
                    .CreateAsync(unServicio);

                if (!resultadoAccion)
                    throw new AppValidationException("Operación ejecutada pero no generó cambios en la DB");

                servicioExistente = await _servicioRepository
                    .GetByNameAsync(unServicio.Nombre!);
            }
            catch (DbOperationException)
            {
                throw;
            }

            return servicioExistente;
        }

        public async Task<Servicio> UpdateAsync(Servicio unServicio)
        {
            string resultadoValidacion = EvaluateServiceDetailsAsync(unServicio);

            if (!string.IsNullOrEmpty(resultadoValidacion))
                throw new AppValidationException(resultadoValidacion);

            //Primero, validamos si existe un servicio con ese Guid
            var servicioExistente = await _servicioRepository
                .GetByGuidAsync(unServicio.Id);

            if (servicioExistente.Id == Guid.Empty)
                throw new AppValidationException($"No existe un servicio con el Guid {unServicio.Id} que se pueda actualizar");

            //Si existe y los datos son iguales, se retorna el objeto para garantizar idempotencia
            if (servicioExistente.Equals(unServicio))
                return servicioExistente;

            try
            {
                bool resultadoAccion = await _servicioRepository
                    .UpdateAsync(unServicio);

                if (!resultadoAccion)
                    throw new AppValidationException("Operación ejecutada pero no generó cambios en la DB");

                servicioExistente = await _servicioRepository
                    .GetByGuidAsync(unServicio.Id);
            }
            catch (DbOperationException)
            {
                throw;
            }

            return servicioExistente;
        }

        public async Task<string> RemoveAsync(Guid servicio_id)
        {
            Servicio unServicio = await _servicioRepository
                .GetByGuidAsync(servicio_id);

            if (unServicio.Id == Guid.Empty)
                throw new AppValidationException($"Servicio no encontrado con el id {servicio_id}");

            var totalComponentesPorServicio = await _servicioRepository
                .GetTotalComponentsByServiceGuidAsync(servicio_id);

            if (totalComponentesPorServicio > 0)
                throw new AppValidationException($"No se puede eliminar {unServicio.Nombre} porque " +
                    $"tiene {totalComponentesPorServicio} componentes asociados");

            var totalConsumosPorServicio = await _servicioRepository
                .GetTotalConsumptionByServiceGuidAsync(servicio_id);

            if (totalConsumosPorServicio > 0)
                throw new AppValidationException($"No se puede eliminar {unServicio.Nombre} porque " +
                    $"tiene consumos en {totalConsumosPorServicio} meses de facturación");

            string nombreServicioEliminado = unServicio.Nombre!;

            try
            {
                bool resultadoAccion = await _servicioRepository
                    .RemoveAsync(servicio_id);

                if (!resultadoAccion)
                    throw new DbOperationException("Operación ejecutada pero no generó cambios en la DB");
            }
            catch (DbOperationException)
            {
                throw;
            }

            return nombreServicioEliminado;
        }

        private static string EvaluateServiceDetailsAsync(Servicio unServicio)
        {
            if (unServicio.Nombre!.Length == 0)
                return "No se puede insertar un servicio con nombre nulo";

            if (unServicio.UnidadMedida!.Length == 0)
                return "No se puede insertar un servicio con una unidad de medida nula.";

            return string.Empty;
        }
    }
}
