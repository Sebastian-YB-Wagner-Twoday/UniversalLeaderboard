using Microsoft.AspNetCore.Identity;

public class LeaderBoardUser : IdentityUser

{
    public ICollection<Guid> contestIds { get; set; } = [];

}


