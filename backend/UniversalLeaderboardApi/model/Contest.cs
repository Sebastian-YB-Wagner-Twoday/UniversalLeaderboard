using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Contest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id {get; set;}

    public required string Name  {get; set;}

    public List<User> Admin {get; }= [];

    public string? Description  {get; set;}

    public bool? Active  {get; set;}

    public RankingType RankingType  {get; set;}

    public RankingOrder RankingOrder  {get; set;}

    public  List<User> Contestants {get; } = [];

    public  List<ScoreEntry<int>> ScoreEntries {get; } = []; 
    
}


