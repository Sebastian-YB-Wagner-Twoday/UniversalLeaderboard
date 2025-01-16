
public class ScoreEntryResponseModel
{
    public Guid Id { get; set; }

    public required string UserId { get; set; }

    public required Guid ContestId { get; set; }
    public required string UserName { get; set; }

    public required double Score { get; set; }

    public ICollection<Guid> RelatedScoreEntries { get; set; } = [];

    public required DateTime Date { get; set; }

}


