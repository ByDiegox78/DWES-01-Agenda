using Agenda.Error.Common;
using CSharpFunctionalExtensions;

namespace Agenda.Repository.Common;

/// <summary>
/// Operaciones CRUD genericas para un repositorio de dominio.
/// Se parametriza con el tipo de la clave (<typeparamref name="TKey"/>) y
/// el tipo del valor/modelo (<typeparamref name="Tvalue"/>).
/// </summary>
public interface ICrudRepository<TKey, Tvalue> {
    /// <summary>
    /// Devuelve los registros paginados y ordenados por clave primaria.
    /// </summary>
    /// <param name="page">Numero de pagina (empieza en 1).</param>
    IEnumerable<Tvalue> GetAll(int page, int pageSize);

    /// <summary>
    /// Busca un registro por su clave primaria.
    /// </summary>
    /// <param name="id">Clave primaria del registro.</param>
    Result<Tvalue, DomainError> GetById(TKey id);

    /// <summary>
    /// Inserta un nuevo registro.
    /// </summary>
    /// <param name="contacto">Modelo a dar de alta.</param>
    Result<Tvalue, DomainError> Create(Tvalue contacto);

    /// <summary>
    /// Actualiza un registro existente.
    /// </summary>
    /// <param name="id">Clave primaria del registro a actualizar.</param>
    /// <param name="contacto">Datos nuevos a aplicar.</param>
    Result<Tvalue, DomainError> Update(TKey id, Tvalue contacto);

    /// <summary>
    /// Elimina un registro existente.
    /// </summary>
    /// <param name="id">Clave primaria del registro a borrar.</param>
    Result<Tvalue, DomainError> Delete(TKey id);
}