using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class User

{
    [Key]
    public required string Email { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required string UserName { get; set; }


    public ICollection<Guid> contestIds { get; set; } = [];

}


