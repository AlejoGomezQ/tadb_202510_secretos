using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;
using System.Security.Claims;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Services
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

        public async Task<Servicio> CreateAsync(Servicio unServicio)
        {
            string resultadoValidacion = EvaluateServiceDetailsAsync(unServicio);

            if (!string.IsNullOrEmpty(resultadoValidacion))
                throw new AppValidationException(resultadoValidacion);

            //Validamos primero si existe con ese nombre y con otra unidad de medida
            var servicioExistente = await _servicioRepository
                .GetByNameAsync(unServicio.Nombre!);

            // Si existe, pero con otra unidad de medida, no se puede insertar
            if(servicioExistente.Id != Guid.Empty && servicioExistente.UnidadMedida != unServicio.UnidadMedida)
                throw new AppValidationException($"Ya existe un servicio {unServicio.Nombre} " +
                    $"pero con unidad de medida {servicioExistente.UnidadMedida}");

            //Si existe y los datos son iguales, se retorna el objeto para garantizar idempotencia
            if (servicioExistente.Nombre ==  unServicio.Nombre && servicioExistente.UnidadMedida == unServicio.UnidadMedida)
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

        //TODO: Crear el método para actualiza - Servicio
        //TODO: Crear el método para borrar - Servicio


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
