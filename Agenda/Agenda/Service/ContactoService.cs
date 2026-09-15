using Agenda.Error.Common;
using Agenda.Error.ContactoError;
using Agenda.Models;
using Agenda.Repository;
using CSharpFunctionalExtensions;
using Serilog;
using Vehiculos.Cache;

namespace Agenda.Service;
/// <inheritdoc />
public class ContactoService(IContactoRepository repository, ICached<int, Contacto> cache) : IContactoService {
    private readonly ILogger _logger = Log.ForContext<ContactoService>();
    /// <inheritdoc />
    public IEnumerable<Contacto> GetAll(int page = 1, int pageSize = 3) {
        _logger.Debug("Obteniendo todos los contactos");
        return repository.GetAll(page, pageSize);
    }
    /// <inheritdoc />
    public Result<Contacto, DomainError> GetById(int id) {
        _logger.Debug("Obteniendo contacto con id: {Id}", id);
        if (cache.Get(id) is { } cached) return Result.Success<Contacto, DomainError>(cached);
        return repository.GetById(id)
            .Tap(c => cache.Add(id, c));
    }
    /// <inheritdoc />
    public Contacto? GetByAlias(string alias) {
        _logger.Debug("Obteniendo contacto con alias: {Alias}", alias);
        var contacto = repository.GetByAlias(alias);
        return contacto ?? null;
    }
    /// <inheritdoc />
    public Result<Contacto, DomainError> Create(Contacto contacto) {
        _logger.Debug("Creando contacto con alias: {Alias}", contacto.Alias);
        return repository.Create(contacto)
            .Tap(t => cache.Add(t.Id, t));
    }
    /// <inheritdoc />
    public Result<Contacto, DomainError> Update(int id, Contacto contacto) {
        _logger.Debug("Actualizando contacto con id: {Id}", id);
        return repository.GetById(id)
            .Tap(t => { cache.Remove(id); })
            .Bind(c => repository.Update(id, contacto));
    }
    /// <inheritdoc />
    public Result<Contacto, DomainError> Delete(int id) {
        _logger.Debug("Eliminando contacto con id: {Id}", id);
        return ComprobarSiExisteContacto(id)
            .Tap(t => { cache.Remove(id); })
            .Bind(b => repository.Delete(id));
    }
    private Result<Contacto, DomainError> ComprobarSiExisteContacto(int id) {
        return repository.GetById(id);
    }
}