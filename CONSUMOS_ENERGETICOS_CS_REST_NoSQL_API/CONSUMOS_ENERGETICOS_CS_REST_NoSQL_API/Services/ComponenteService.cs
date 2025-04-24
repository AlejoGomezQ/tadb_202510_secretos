using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Services
{
    public class ComponenteService(IComponenteRepository componenteRepository,
                                    IServicioRepository servicioRepository)
    {
        private readonly IComponenteRepository _componenteRepository = componenteRepository;
        private readonly IServicioRepository _servicioRepository = servicioRepository;

        public async Task<List<Componente>> GetAllAsync()
        {
            return await _componenteRepository
                .GetAllAsync();
        }

        public async Task<Componente> GetByIdAsync(string componente_id)
        {
            Componente unComponente = await _componenteRepository
                .GetByIdAsync(componente_id);

            if (string.IsNullOrEmpty(unComponente.Id))
                throw new AppValidationException($"Componente no encontrado con el Guid {componente_id}");

            return unComponente;
        }

        //TODO: Crear el método para obtener - Componentes por Periodo
        public async Task<Componente> CreateAsync(Componente unComponente)
        {
            string resultadoValidacion = EvaluateComponentDetailsAsync(unComponente);

            if (!string.IsNullOrEmpty(resultadoValidacion))
                throw new AppValidationException(resultadoValidacion);

            var servicioExistente = await _servicioRepository
                .GetByIdAsync(unComponente.ServicioId!);

            if (string.IsNullOrEmpty(servicioExistente.Id))
                throw new AppValidationException($"No existe un servicio asociado al ID {unComponente.ServicioId} ");

            if (servicioExistente.Nombre != unComponente.Servicio)
                throw new AppValidationException($"Los datos de servicio suministrados no coinciden.");


            //Validamos primero si existe ese componente con ese nombre y ese servicio
            var componenteExistente = await _componenteRepository
                .GetByNameAndServiceAsync(unComponente.Nombre!, unComponente.Servicio!);

            //Si existe y los datos son iguales, se retorna el objeto para garantizar idempotencia
            if (componenteExistente.Nombre == unComponente.Nombre && componenteExistente.Servicio == unComponente.Servicio)
                return componenteExistente;

            try
            {
                bool resultadoAccion = await _componenteRepository
                    .CreateAsync(unComponente);

                if (!resultadoAccion)
                    throw new AppValidationException("Operación ejecutada pero no generó cambios en la DB");

                componenteExistente = await _componenteRepository
                    .GetByNameAndServiceAsync(unComponente.Nombre!, unComponente.Servicio!);
            }
            catch (DbOperationException)
            {
                throw;
            }

            return componenteExistente;
        }

        public async Task<Componente> UpdateAsync(Componente unComponente)
        {
            string resultadoValidacion = EvaluateComponentDetailsAsync(unComponente);

            if (!string.IsNullOrEmpty(resultadoValidacion))
                throw new AppValidationException(resultadoValidacion);

            var componenteExistente = await _componenteRepository
                .GetByIdAsync(unComponente.Id!);

            if (string.IsNullOrEmpty(componenteExistente.Id))
                throw new AppValidationException($"No existe un componente con el Guid {unComponente.Id} que se pueda actualizar");

            var servicioExistente = await _servicioRepository
                .GetByIdAsync(unComponente.ServicioId!);

            if (string.IsNullOrEmpty(servicioExistente.Id))
                throw new AppValidationException($"No existe un servicio asociado al ID {unComponente.ServicioId} ");

            if (servicioExistente.Nombre != unComponente.Servicio)
                throw new AppValidationException($"Los datos de servicio suministrados no coinciden.");

            //Si existe y los datos son iguales, se retorna el objeto para garantizar idempotencia
            if (componenteExistente.Equals(unComponente))
                return componenteExistente;

            try
            {
                bool resultadoAccion = await _componenteRepository
                    .UpdateAsync(unComponente);

                if (!resultadoAccion)
                    throw new AppValidationException("Operación ejecutada pero no generó cambios en la DB");

                componenteExistente = await _componenteRepository
                    .GetByIdAsync(unComponente.Id!);
            }
            catch (DbOperationException)
            {
                throw;
            }

            return componenteExistente;
        }

        public async Task<Componente> RemoveAsync(string componente_id)
        {
            Componente unComponente = await _componenteRepository
                .GetByIdAsync(componente_id);

            if (string.IsNullOrEmpty(unComponente.Id))
                throw new AppValidationException($"Componente no encontrado con el id {componente_id}");


            try
            {
                bool resultadoAccion = await _componenteRepository
                    .RemoveAsync(componente_id);

                if (!resultadoAccion)
                    throw new DbOperationException("Operación ejecutada pero no generó cambios en la DB");
            }
            catch (DbOperationException)
            {
                throw;
            }

            return unComponente;
        }
        private static string EvaluateComponentDetailsAsync(Componente unComponente)
        {
            if (unComponente.Nombre!.Length == 0)
                return "No se puede insertar un componente con nombre nulo";

            if (unComponente.Servicio!.Length == 0)
                return "No se puede insertar un componente con un servicio nulo.";

            if (unComponente.ServicioId!.Length == 0)
                return "No se puede insertar un componente con un ID de servicio nulo.";

            return string.Empty;
        }
    }
}