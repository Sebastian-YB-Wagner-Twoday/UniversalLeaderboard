using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class Contest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public ICollection<Guid> Admin { get; set; } = [];

    public DateTime CreatedDate { get; set; }

    public string? Description { get; set; }

    public bool? Active { get; set; }

    public RankingType RankingType { get; set; }

    public RankingOrder RankingOrder { get; set; }

    public ICollection<Guid> Contestants { get; set; } = [];

    public ICollection<ScoreEntry<int>> DisplayedScores { get; set; } = [];

    public ICollection<ScoreEntry<int>> ScoreEntries { get; set; } = [];

}


