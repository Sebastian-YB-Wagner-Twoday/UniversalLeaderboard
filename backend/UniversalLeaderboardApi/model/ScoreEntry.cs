using Microsoft.EntityFrameworkCore;

[Keyless]
public class ScoreEntry<T>
{

    public required Guid UserId { get; set; }

    public required T score { get; set; }

}


