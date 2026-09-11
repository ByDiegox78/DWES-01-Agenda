using Agenda.Entity;
using Agenda.Models;
using CSharpFunctionalExtensions;
using GestionEsports.Esports.Error.Common;
using Serilog;

namespace Agenda.Repository;

public class ContactoRepository : IContactoRepository{
    private readonly AppDbContext _context;
    private readonly ILogger _logger = Log.ForContext<ContactoRepository>();
    public ContactoRepository(AppDbContext context, bool dropData = false) {
        _context = context;
        if (dropData) _context.Database.EnsureDeleted();
        _context.EnsurceCreated();
    }
    
    
    public IEnumerable<Contacto?> GetAll(int page, int pageSize, bool isDeleted) {
        throw new NotImplementedException();
    }

    public Contacto? GetById(int id) {
        try {
            _logger.Debug("Obteniendo contacto con id: {Id}", id);
            return _context.Contacto.FirstOrDefault(i => i.Id == id)?.tom;
        } catch (Exception e) {
            Console.WriteLine(e);
            throw;
        }
    }

    public Result<Contacto, DomainError> Create(Contacto jugador) {
        throw new NotImplementedException();
    }

    public Result<Contacto, DomainError> Update(int id, Contacto jugador) {
        throw new NotImplementedException();
    }

    public Contacto? Delete(int id, bool isLogic) {
        throw new NotImplementedException();
    }

    public Contacto? GetByAlias(string alias) {
        throw new NotImplementedException();
    }
}