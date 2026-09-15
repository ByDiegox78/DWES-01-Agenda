using Agenda.Models;
using Agenda.Repository.Common;

namespace Agenda.Repository;

/// <summary>
/// Repositorio de contactos. Implementa el CRUD generico
/// <see cref="ICrudRepository{TKey,Tvalue}"/> con clave <see langword="int"/>
/// y modelo <see cref="Contacto"/>, y anyade las consultas especificas de la entidad.
/// </summary>
public interface IContactoRepository : ICrudRepository<int, Contacto> {
    /// <summary>
    /// Busca un contacto por su alias.
    /// </summary>
    /// <param name="alias">Alias a buscar.</param>
    Contacto? GetByAlias(string alias);
}