using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class ScoreEntry<T>
{

    public required User User {get; set;}
    
    public required T score {get; set;}

} 


