using CSharpFunctionalExtensions;
using GestionEsports.Esports.Error.Common;

namespace Agenda.Repository.Common;

public interface ICrudRepository<TKey, Tvalue> {
    IEnumerable<Tvalue> GetAll(int page, int pageSize);
    
    Tvalue? GetById(TKey id);

    Result<Tvalue, DomainError> Create(Tvalue contacto);
    
    Result<Tvalue, DomainError> Update(TKey id, Tvalue contacto);

    Tvalue? Delete(TKey id);
}