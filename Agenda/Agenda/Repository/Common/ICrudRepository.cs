using CSharpFunctionalExtensions;
using GestionEsports.Esports.Error.Common;

namespace Agenda.Repository.Common;

public interface ICrudRepository<TKey, Tvalue> {
    IEnumerable<Tvalue?> GetAll(int page, int pageSize, bool isDeleted);
    
    Tvalue? GetById(TKey id);

    Result<Tvalue, DomainError> Create(Tvalue jugador);
    
    Result<Tvalue, DomainError> Update(TKey id, Tvalue jugador);

    Tvalue? Delete(TKey id, bool isLogic);
}