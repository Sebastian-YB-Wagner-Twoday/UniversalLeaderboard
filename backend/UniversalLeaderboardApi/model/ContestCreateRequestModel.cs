
public class ContestCreateRequestModel
{

    public required string Name { get; set; }

    public required string Description { get; set; }

    public required string AdminId { get; set; }

    public int RankingType { get; set; }

    public int RankingOrder { get; set; }

    public int ScoreType { get; set; }

}


