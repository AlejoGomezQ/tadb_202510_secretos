using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Exceptions;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Models;

namespace CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Services
{
    public class ComponenteService(IComponenteRepository componenteRepository)
    {
        private readonly IComponenteRepository _componenteRepository = componenteRepository;

        public async Task<List<Componente>> GetAllAsync()
        {
            return await _componenteRepository
                .GetAllAsync();
        }

        public async Task<Componente> GetByGuidAsync(Guid componente_id)
        {
            Componente unComponente = await _componenteRepository
                .GetByGuidAsync(componente_id);

            if (unComponente.Id == Guid.Empty)
                throw new AppValidationException($"Componente no encontrado con el Guid {componente_id}");

            return unComponente;
        }

        //TODO: Crear el método para obtener - Componentes por Periodo
        public async Task<Componente> CreateAsync(Componente unComponente)
        {
            string resultadoValidacion = EvaluateComponentDetailsAsync(unComponente);

            if (!string.IsNullOrEmpty(resultadoValidacion))
                throw new AppValidationException(resultadoValidacion);

            //Validamos primero si existe con ese nombre y ese servicio
            var componenteExistente = await _componenteRepository
                .GetByNameAndServiceAsync(unComponente.Nombre!, unComponente.Servicio!);

            // Si existe, no se puede insertar
            if (componenteExistente.Id != Guid.Empty)
                throw new AppValidationException($"Ya existe un componente {unComponente.Nombre} " +
                    $"asociado al servicio {unComponente.Servicio}");

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

            //Validamos primero si existe con ese Guid
            var componenteExistente = await _componenteRepository
                .GetByGuidAsync(unComponente.Id);

            if (componenteExistente.Id == Guid.Empty)
                throw new AppValidationException($"No existe un componente con el Guid {unComponente.Id} que se pueda actualizar");

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
                    .GetByGuidAsync(unComponente.Id);
            }
            catch (DbOperationException)
            {
                throw;
            }

            return componenteExistente;
        }

        public async Task<string> RemoveAsync(Guid componente_id)
        {
            Componente unComponente = await _componenteRepository
                .GetByGuidAsync(componente_id);

            if (unComponente.Id == Guid.Empty)
                throw new AppValidationException($"Componente no encontrado con el id {componente_id}");

            string nombreComponenteEliminado = unComponente.Nombre!;

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

            return nombreComponenteEliminado;
        }
        private static string EvaluateComponentDetailsAsync(Componente unComponente)
        {
            if (unComponente.Nombre!.Length == 0)
                return "No se puede insertar un componente con nombre nulo";

            if (unComponente.Servicio!.Length == 0)
                return "No se puede insertar un componente con un servicio nulo.";

            return string.Empty;
        }
    }
}