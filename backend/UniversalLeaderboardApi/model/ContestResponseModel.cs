
public class ContestResponseModel
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public ICollection<Guid> Admin { get; set; } = [];

    public DateTime CreatedDate { get; set; }

    public string? Description { get; set; }

    public bool? Active { get; set; }

    public RankingType RankingType { get; set; }

    public RankingOrder RankingOrder { get; set; }

    public ScoreType ScoreType { get; set; }

    public ICollection<Guid> Contestants { get; set; } = [];

}
