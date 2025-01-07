using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Email), nameof(Id))]
public class User

{
    public required string Email { get; set; }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required string UserName { get; set; }


    public ICollection<Guid> contestIds { get; set; } = [];

}


