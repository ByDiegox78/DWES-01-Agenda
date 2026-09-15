using Agenda.Error.Common;
using Agenda.Models;
using CSharpFunctionalExtensions;

namespace Agenda.Service;

/// <summary>
/// Contrato de la capa de servicio de contactos: la orquestacion entre el repositorio
/// (acceso a datos) y la cache LRU (consulta rapida y limpado de entradas).
/// </summary>
/// <remarks>
/// <para>
/// Los metodos que pueden fallar de forma esperada devuelven
/// <see cref="Result{T,E}"/> (<c>Success</c>/<c>Failure</c> con un <see cref="DomainError"/>),
/// en lugar de lanzar excepciones.
/// </para>
/// <para>
/// Esta interface permite desacoplar el consumidor (consola, futura API...) de la
/// implementacion concreta <see cref="ContactoService"/>, lo que tambien facilita
/// sustituir el servicio por un mock en los tests.
/// </para>
/// </remarks>
public interface IContactoService {
    /// <summary>
    /// Devuelve los contactos paginados, delegando directamente en el repositorio.
    /// </summary>
    /// <param name="page">Numero de pagina (empieza en 1).</param>
    /// <param name="pageSize">Contactos por pagina.</param>
    /// <returns>Los contactos de la pagina solicitada. Lista vacia si no hay resultados.</returns>
    IEnumerable<Contacto> GetAll(int page, int pageSize);

    /// <summary>
    /// Busca un contacto por id, consultando primero la cache y, si no esta,
    /// el repositorio (y guardando el resultado en cache).
    /// </summary>
    /// <param name="id">Identificador del contacto.</param>
    Result<Contacto, DomainError> GetById(int id);

    /// <summary>
    /// Busca un contacto por su alias.
    /// </summary>
    /// <param name="alias">Alias a buscar.</param>
    Contacto? GetByAlias(string alias);

    /// <summary>
    /// Crea un contacto nuevo y lo guarda en cache.
    /// </summary>
    /// <param name="contacto">Contacto a crear.</param>
    Result<Contacto, DomainError> Create(Contacto contacto);

    /// <summary>
    /// Actualiza un contacto existente, limpiando su entrada de cache.
    /// Comprueba antes que el contacto existe.
    /// </summary>
    /// <param name="id">Identificador del contacto a actualizar.</param>
    /// <param name="contacto">Nuevos datos.</param>
    Result<Contacto, DomainError> Update(int id, Contacto contacto);

    /// <summary>
    /// Elimina un contacto existente, limpiando su entrada de cache.
    /// Comprueba antes que el contacto existe.
    /// </summary>
    /// <param name="id">Identificador del contacto a borrar.</param>
    Result<Contacto, DomainError> Delete(int id);
}