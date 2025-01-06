using System.ComponentModel.DataAnnotations;


public class User
{
    public required string UserName { get; set; }

    [Key]
    public required string Email { get; set; }

    public List<string> contestIds { get; } = [];

}


