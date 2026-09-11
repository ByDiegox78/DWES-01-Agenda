using Agenda.Models;
using Agenda.Repository.Common;

namespace Agenda.Repository;

public interface IContactoRepository : ICrudRepository<int, Contacto> {
    Contacto? GetByAlias(string alias);
}