using Agenda.Entity;
using Agenda.Models;

namespace Agenda.Mapper;

public static class ContactoMapper {
    public static Contacto ToModel(this ContactoEntity entity) {
        return new Contacto {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Alias = entity.Alias,
            Telefono = entity.Telefono,
            Email = entity.Email,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsDeleted = entity.IsDeleted
        };
    } 
    public static ContactoEntity ToEntity(this Contacto model) {
        return new ContactoEntity {
            Id = model.Id,
            Nombre = model.Nombre,
            Alias = model.Alias,
            Telefono = model.Telefono,
            Email = model.Email,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt,
            IsDeleted = model.IsDeleted
        };
    }
}