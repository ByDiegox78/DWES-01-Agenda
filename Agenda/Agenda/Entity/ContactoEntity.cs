using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agenda.Entity;

[Table("Contacto")]

public class ContactoEntity {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Nombre { get; set; }
    [Required]
    [MaxLength(50)]
    public string Alias { get; set; }
    [Required]
    public int Telefono { get; set; }
    [Required]  
    [MaxLength(100)]
    public string Email { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public bool IsDeleted { get; set; }
}