namespace Agenda.Models;

public record Contacto() {
    public int Id { get; set; }
    public string Nombre { get; init; }
    public string Alias { get; init; }
    public int Telefono { get; init; }
    public string Email { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public bool IsDeleted { get; init; }
}