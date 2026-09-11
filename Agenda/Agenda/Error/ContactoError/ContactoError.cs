using GestionEsports.Esports.Error.Common;

namespace Agenda.Error.ContactoError;

public abstract record ContactoError(string Message) : DomainError(Message) {
    public sealed record NotFound(string Id)
        : ContactoError($"No se ha encontrado ningun contacto con id: {Id}");
    public sealed record DatabaseError(string Details)
        : ContactoError($"Error de base de datos: {Details}");
    public sealed record AliasNotFound(string Alias)
        : ContactoError($"No se ha encontrado ningun contacto con alias: {Alias}");
    public sealed record NameAlreadyExists(string Name)
        : ContactoError($"El nombre {Name} ya existe");
}

public static class ContactoErrors {
    public static DomainError NotFound(string id) {
        return new ContactoError.NotFound(id);
    }
    public static DomainError DatabaseError(string details) {
        return new ContactoError.DatabaseError(details);
    }
    public static DomainError AliasNotFound(string alias) {
        return new ContactoError.AliasNotFound(alias);
    }
    public static DomainError NameAlreadyExists(string name) {
        return new ContactoError.NameAlreadyExists(name);
    }
}