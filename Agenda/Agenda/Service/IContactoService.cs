using Agenda.Models;
using CSharpFunctionalExtensions;
using GestionEsports.Esports.Error.Common;

namespace Agenda.Service;

public interface IContactoService {
    IEnumerable<Contacto> GetAll(int page, int pageSize);
    
    Contacto? GetById(int id);
    
    Contacto? GetByAlias(string alias);

    Result<Contacto, DomainError> Create(Contacto contacto);
    
    Result<Contacto, DomainError> Update(int id, Contacto contacto);

    Result<Contacto, DomainError> Delete(int id);
}