public class Tech
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public TechType TechType { get; set; }
    public string? Description { get; set; }
}


public enum TechType {
    Frontend,
    Backend,
    Fullstack,
    Tool,
    Other
}