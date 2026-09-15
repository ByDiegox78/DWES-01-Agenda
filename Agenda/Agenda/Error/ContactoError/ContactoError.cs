using Agenda.Error.Common;

namespace Agenda.Error.ContactoError;

/// <summary>
/// Errores de dominio especificos de la entidad <c>Contacto</c>.
/// Extienden <see cref="DomainError"/> y aportan un <see cref="Message"/>
/// legible listo para mostrar al usuario o loguear.
/// </summary>
public abstract record ContactoError(string Message) : DomainError(Message) {
    /// <summary>
    /// El contacto con el id indicado no existe en la base de datos.
    /// </summary>
    /// <param name="Id">Identificador del contacto buscado.</param>
    public sealed record NotFound(string Id)
        : ContactoError($"No se ha encontrado ningun contacto con id: {Id}");

    /// <summary>
    /// Se produjo una excepcion tecnica al acceder a la base de datos.
    /// </summary>
    /// <param name="Details">Detalle del mensaje de la excepcion original.</param>
    public sealed record DatabaseError(string Details)
        : ContactoError($"Error de base de datos: {Details}");

    /// <summary>
    /// Ya existe un contacto con el mismo nombre, por lo que la creacion es un conflicto.
    /// </summary>
    /// <param name="Name">Nombre del contacto que se intenta duplicar.</param>
    public sealed record NameAlreadyExists(string Name)
        : ContactoError($"El nombre {Name} ya existe");
}
/// <summary>
/// Fabrica de errores de <c>Contacto</c>. Devuelve los subtipos como
/// <see cref="DomainError"/> para que el repositorio/servicio solo dependa
/// del tipo base.
/// </summary>
public static class ContactoErrors {
    /// <summary>Crea un error de contacto no encontrado.</summary>
    /// <param name="id">Id del contacto inexistente.</param>
    public static DomainError NotFound(string id) {
        return new ContactoError.NotFound(id);
    }
    /// <summary>Crea un error de acceso a base de datos.</summary>
    /// <param name="details">Detalle de la excepcion.</param>
    public static DomainError DatabaseError(string details) {
        return new ContactoError.DatabaseError(details);
    }
    /// <summary>Crea un error de nombre de contacto duplicado.</summary>
    /// <param name="name">Nombre duplicado.</param>
    public static DomainError NameAlreadyExists(string name) {
        return new ContactoError.NameAlreadyExists(name);
    }
}