using Agenda.Error.ContactoError;
using Agenda.Models;
using Agenda.Repository;
using CSharpFunctionalExtensions;
using GestionEsports.Esports.Error.Common;
using Serilog;
using Vehiculos.Cache;

namespace Agenda.Service;

public class ContactoService(IContactoRepository repository, ICached<int, Contacto> cache) : IContactoService {
    private readonly ILogger _logger = Log.ForContext<ContactoService>();

    public IEnumerable<Contacto> GetAll(int page = 1, int pageSize = 3) {
        _logger.Debug("Obteniendo todos los contactos");
        return repository.GetAll(page, pageSize);
    }

    public Contacto? GetById(int id) {
        _logger.Debug("Obteniendo contacto con id: {Id}", id);
        if (cache.Get(id) is { } cached) return cached;
        if (repository.GetById(id) is { } c) {
            cache.Add(id, c);
            return c;
        }
        return null;
    }

    public Contacto? GetByAlias(string alias) {
        _logger.Debug("Obteniendo contacto con alias: {Alias}", alias);
        var contacto = repository.GetByAlias(alias);
        return contacto ?? null;
    }

    public Result<Contacto, DomainError> Create(Contacto contacto) {
        
    }

    public Result<Contacto, DomainError> Update(int id, Contacto contacto) {
        _logger.Debug("Actualizando contacto con id: {Id}", id);
        return ComprobarSiExisteContacto(id)
            .Tap(t => { cache.Remove(id); })
            .Bind(c => repository.Update(id, contacto));
    }

    public Result<Contacto, DomainError> Delete(int id) {
        return ComprobarSiExisteContacto(id)
            .Tap(t => { cache.Remove(id); })
            .Map(b => repository.Delete(id));
    }
    private Result<Contacto, DomainError> ComprobarSiExisteContacto(int id) {
        return repository.GetById(id) is { } c
            ? Result.Success<Contacto, DomainError>(c)
            : Result.Failure<Contacto, DomainError>(ContactoErrors.NotFound(id.ToString()));
    }
}