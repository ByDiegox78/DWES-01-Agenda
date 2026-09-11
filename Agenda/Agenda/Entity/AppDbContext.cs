using Microsoft.EntityFrameworkCore;

namespace Agenda.Entity;

public class AppDbContext  : DbContext {
    public DbSet<ContactoEntity> Contacto { get; set; } = null!;
    private readonly string _connectionString;

    public AppDbContext(string connectionString) {
        _connectionString = connectionString;
    }

    public AppDbContext(DbContextOptions<AppDbContext> options) {
        _connectionString = "";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlite(_connectionString);
    }

    public void EnsurceCreated() {
        Database.EnsureCreated();
    }
} 