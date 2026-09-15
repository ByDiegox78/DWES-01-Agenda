using Agenda.Entity;
using Agenda.Error.Common;
using Agenda.Error.ContactoError;
using Agenda.Mapper;
using Agenda.Models;
using Agenda.Repository.Common;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Agenda.Repository;

/// <inheritdoc />
public class ContactoRepository : IContactoRepository {
    private readonly AppDbContext _context;
    private readonly ILogger _logger = Log.ForContext<ContactoRepository>();

    /// <summary>
    /// Crea el repositorio sobre un contexto EF Core.
    /// </summary>
    /// <param name="context">Contexto de base de datos a usar.</param>
    /// <param name="dropData">
    /// </param>
    public ContactoRepository(AppDbContext context, bool dropData = false) {
        _context = context;
        if (dropData) _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }
    /// <inheritdoc cref="ICrudRepository{TKey,Tvalue}.GetAll" />
    public IEnumerable<Contacto> GetAll(int page, int pageSize) {
        _logger.Debug("Obteniendo todos los contactos");
        try {
            var query = _context.Contacto.AsNoTracking();
            var entities = query
                .OrderBy(i => i.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            _logger.Debug("Se han obtenido {Count} contactos exitosamente", entities.Count);
            return entities.Select(i => i.ToModel());
        }
        catch (Exception e) {
            _logger.Error(e, "Error, no se pudieron obtener los contactos");
            return Enumerable.Empty<Contacto>();
        }
    }

    /// <inheritdoc />
    public Result<Contacto, DomainError> GetById(int id) {
        try {
            _logger.Debug("Obteniendo contacto con id: {Id}", id);
            var contacto = _context.Contacto.FirstOrDefault(i => i.Id == id)?.ToModel();
            if (contacto is null)
                return Result.Failure<Contacto, DomainError>(ContactoErrors.NotFound(id.ToString()));
            return Result.Success<Contacto, DomainError>(contacto);
        }
        catch (Exception e) {
            _logger.Error(e, "Error, no se encontro al contacto con ID: {Id}", id);
            return Result.Failure<Contacto, DomainError>(ContactoErrors.DatabaseError(e.Message));
        }
    }

    /// <inheritdoc />
    public Result<Contacto, DomainError> Create(Contacto contacto) {
        _logger.Debug("Creando contacto...");
        var exist = ExisteContacto(contacto.Alias);
        if (exist)
            return Result.Failure<Contacto, DomainError>(ContactoErrors.NameAlreadyExists(contacto.Nombre));
        contacto = contacto with {
            Id = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        try {
            var entity = contacto.ToEntity();
            _context.Contacto.Add(entity);
            _context.SaveChanges();
            _logger.Debug("Contacto creado correctamente");
            return Result.Success<Contacto, DomainError>(entity.ToModel());
        }
        catch (Exception e) {
            _logger.Error(e, "Error, no se pudo crear al contacto que introducistes");
            return Result.Failure<Contacto, DomainError>(ContactoErrors.DatabaseError(e.Message));
        }
    }

    /// <inheritdoc />
    public Result<Contacto, DomainError> Update(int id, Contacto contacto) {
        _logger.Debug("Actualizando contacto con id: {Id}", id);
        var entity = _context.Contacto.FirstOrDefault(i => i.Id == id);
        if (entity == null)
            return Result.Failure<Contacto, DomainError>(ContactoErrors.NotFound(id.ToString()));
        entity.Nombre = contacto.Nombre;
        entity.Alias = contacto.Alias;
        entity.Telefono = contacto.Telefono;
        entity.Email = contacto.Email;
        entity.UpdatedAt = DateTime.UtcNow;
        try {
            _context.SaveChanges();
            _logger.Debug("Contacto actualizado correctamente");
            return Result.Success<Contacto, DomainError>(entity.ToModel());
        }
        catch (Exception e) {
            _logger.Error(e, "Error, no se pudo actualizar el contacto con ID: {Id}", id);
            return Result.Failure<Contacto, DomainError>(ContactoErrors.DatabaseError(e.Message));
        }
    }

    /// <inheritdoc />
    public Result<Contacto, DomainError> Delete(int id) {
        try {
            _logger.Debug("Eliminando contacto con id: {Id}", id);
            var entity = _context.Contacto.FirstOrDefault(i => i.Id == id);
            if (entity == null) return Result.Failure<Contacto, DomainError>(ContactoErrors.NotFound(id.ToString()));
            _context.Contacto.Remove(entity);
            _context.SaveChanges();
            _logger.Debug("Contacto eliminado correctamente");
            return Result.Success<Contacto, DomainError>(entity.ToModel());
        }
        catch (Exception e) {
            _logger.Error(e, "Error, no se pudo eliminar el contacto con ID: {Id}", id);
            return Result.Failure<Contacto, DomainError>(ContactoErrors.DatabaseError(e.Message));
        }
    }
    /// <inheritdoc />
    public Contacto? GetByAlias(string alias) {
        try {
            _logger.Debug("Obteniendo contacto con alias: {Alias}", alias);
            return _context.Contacto.FirstOrDefault(i => i.Alias == alias)?.ToModel();
        }
        catch (Exception e) {
            _logger.Error(e, "Error, no se encontro al contacto con alias: {Alias}", alias);
            return null;
        }
    }
    private bool ExisteContacto(string alias) {
        _logger.Debug("Verificando si existe contacto con alias: {alias}", alias);
        return _context.Contacto.Any(i => i.Alias == alias);
    }
}