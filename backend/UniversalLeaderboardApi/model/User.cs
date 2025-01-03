using System.ComponentModel.DataAnnotations;


public class User{
    [Key]
    public required string UserName {get; set;}
    
    public  List<string> contestIds {get; } = [];

} 


