namespace Agenda.Models;

public record Contacto() {
    public int Id { get; set; }
    public string Nombre { get; init; } = string.Empty;
    public string Alias { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public bool IsDeleted { get; init; }
}