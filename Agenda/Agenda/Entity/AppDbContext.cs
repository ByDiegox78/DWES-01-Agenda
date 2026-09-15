using Microsoft.EntityFrameworkCore;

namespace Agenda.Entity;

/// <summary>
/// Contexto de base de datos de la aplicacion Agenda.
/// Gestiona el mapeo de las entidades
/// a las tablas de la base de datos SQLite y expone el acceso a las mismas
/// a traves de DbSets.
/// </summary>
public class AppDbContext : DbContext {
    /// <summary>
    /// Coleccion de contactos mapeada a la tabla "Contacto".
    /// Permite consultar y modificar registros mediante LINQ.
    /// </summary>
    public DbSet<ContactoEntity> Contacto { get; set; } = null!;

    /// <summary>
    /// Cadena de conexion a SQLite usada por <see cref="OnConfiguring"/>
    /// cuando el contexto se construye sin opciones externas.
    /// </summary>
    private readonly string _connectionString;

    /// <summary>
    /// Crea un contexto apuntando a una base de datos SQLite mediante su
    /// cadena de conexion
    /// </summary>
    /// <param name="connectionString">Cadena de conexion de SQLite.</param>
    public AppDbContext(string connectionString) {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Crea un contexto a partir de opciones externas ya configuradas
    /// </summary>
    /// <param name="options">Opciones de configuracion del contexto.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        _connectionString = "";
    }

    /// <summary>
    /// Configura la conexion a SQLite si no se han inyectado opciones externas.
    /// </summary>
    /// <param name="optionsBuilder">Constructor de opciones del contexto.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlite(_connectionString);
    }

    /// <summary>
    /// Crea la base de datos y sus tablas si aun no existen.
    /// Sin efecto si la base de datos ya esta creada.
    /// </summary>
    public void EnsurceCreated() {
        Database.EnsureCreated();
    }
}