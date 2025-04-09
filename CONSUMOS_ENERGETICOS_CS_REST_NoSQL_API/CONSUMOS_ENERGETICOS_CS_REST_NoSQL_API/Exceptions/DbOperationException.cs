/*
DbOperationException:
Excepcion creada para enviar mensajes relacionados 
con la validación en la capa de persistencia de datos
*/

namespace CONSUMOS_ENERGETICOS_CS_REST_NoSQL_API.Exceptions
{
    public class DbOperationException(string message) : Exception(message)
    {
    }
}