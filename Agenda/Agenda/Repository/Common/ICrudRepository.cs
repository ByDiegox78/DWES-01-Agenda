using CSharpFunctionalExtensions;
using GestionEsports.Esports.Error.Common;

namespace Agenda.Repository.Common;

public interface ICrudRepository<TKey, Tvalue> {
    IEnumerable<Tvalue> GetAll(int page, int pageSize);
    
    Result<Tvalue, DomainError> GetById(TKey id);

    Result<Tvalue, DomainError> Create(Tvalue contacto);
    
    Result<Tvalue, DomainError> Update(TKey id, Tvalue contacto);

    Result<Tvalue, DomainError> Delete(TKey id);
}