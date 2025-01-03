using Microsoft.EntityFrameworkCore;

[Keyless]
public class ScoreEntry<T>
{

    public required User User {get; set;}
    
    public required T score {get; set;}

} 


