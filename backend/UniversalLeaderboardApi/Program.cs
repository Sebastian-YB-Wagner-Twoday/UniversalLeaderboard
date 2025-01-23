using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("AppDb"));
builder.Services.AddDbContext<UniversalLeaderboardDb>(opt => opt.UseInMemoryDatabase("LeaderBoardDb"));
builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<LeaderBoardUser>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "UniversalLeaderboardAPI";
    config.Title = "UniversalLeaderboardAPI v0.2";
    config.Version = "v0.2";
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "UniversalLeaderboardAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

app.MapIdentityApi<LeaderBoardUser>();


app.MapPost("/registerUsername", async (UserNameDTO userName, ApplicationDbContext appdb, UserManager<LeaderBoardUser> userManager, ClaimsPrincipal principal) =>
{
    var loggedInUser = await userManager.GetUserAsync(principal);
    var user = await appdb.Users.FindAsync(loggedInUser?.Id);

    if (user is not null)
    {
        user.UserName = userName.UserName;
        await appdb.SaveChangesAsync();
        return Results.Ok();
    }
    return Results.NotFound();

})
.RequireAuthorization();

// protection from cross-site request forgery (CSRF/XSRF) attacks with empty body
// form can't post anything useful so the body is null, the JSON call can pass
// an empty object {} but doesn't allow cross-site due to CORS.
app.MapPost("/logout", async (SignInManager<LeaderBoardUser> signInManager, [FromBody] object empty) =>
{
    if (empty is not null)
    {
        await signInManager.SignOutAsync();
        return Results.Ok();
    }
    return Results.NotFound();

}).RequireAuthorization();

var user = app.MapGroup("/user");

user.MapGet("", async (UserManager<LeaderBoardUser> userManager, ClaimsPrincipal principal) =>
    await userManager.GetUserAsync(principal)
        is LeaderBoardUser user
            ? Results.Ok(user)
            : Results.NotFound());

/**
* get list of all contests related to user
*/
user.MapGet("/contests/{pagination}", async (int pagination, UniversalLeaderboardDb db, UserManager<LeaderBoardUser> userManager, ClaimsPrincipal principal) =>
{
    var user = await userManager.GetUserAsync(principal);


    if (user != null)
    {
        var contests = db.Contests
                            .Where(contest => user.contestIds.Contains(contest.Id))
                            .OrderBy(contest => contest.CreatedDate)
                            .Skip(pagination * 10)
                            .Take(10)
                            .ToList();

        ICollection<ContestResponseModel> returnContests = [];
        foreach (var contest in contests)
        {


            returnContests.Add(new ContestResponseModel()
            {
                Name = contest.Name,
                Id = contest.Id,
                CreatedDate = contest.CreatedDate,
                Description = contest.Description,
                RankingType = contest.RankingType,
                RankingOrder = contest.RankingOrder,
                ScoreType = contest.ScoreType,
            });
        }
        ;

        return Results.Ok(returnContests);
    }
    else
    {
        return Results.NotFound();
    }

}).RequireAuthorization();


var contestItems = app.MapGroup("/contest");

contestItems.MapGet("/", async (UniversalLeaderboardDb db) =>
    await db.Contests.ToListAsync());


contestItems.MapGet("/{id}", async (string id, UniversalLeaderboardDb db) =>
{
    var contest = await db.Contests.FindAsync(new Guid(id));

    if (contest != null)
    {
        return Results.Ok(new ContestResponseModel()
        {
            Name = contest.Name,
            Id = contest.Id,
            CreatedDate = contest.CreatedDate,
            Description = contest.Description,
            RankingType = contest.RankingType,
            RankingOrder = contest.RankingOrder,
            ScoreType = contest.ScoreType
        });
    }
    else
    {
        return Results.NotFound();
    }
});

contestItems.MapGet("/{id}/scores", async (string id, UniversalLeaderboardDb db, ApplicationDbContext appdb) =>
{
    var contest = await db.Contests.FindAsync(new Guid(id));

    if (contest != null)
    {
        var displayedScores = db.ScoreEntries.Where(entry => contest.DisplayedScores.Contains(entry.Id));

        if (!displayedScores.Any())
        {
            return Results.Ok(displayedScores.ToList());
        }

        ICollection<ScoreEntryResponseModel> displayedScoresWithUserName = [];

        foreach (var displayedScore in displayedScores)
        {
            var user = await appdb.Users.FindAsync(displayedScore.UserId);
            displayedScoresWithUserName.Add(new ScoreEntryResponseModel()
            {
                Id = displayedScore.Id,
                ContestId = displayedScore.ContestId,
                UserId = displayedScore.UserId,
                Score = displayedScore.Score,
                Date = displayedScore.Date,
                UserName = user?.UserName ?? "N/A"
            });
        }

        if (contest.RankingOrder == RankingOrder.Ascending)
        {
            return Results.Ok(
                displayedScoresWithUserName
                    .OrderBy(contest => contest.Score)
                    .ToList()
            );
        }
        else
        {
            return Results.Ok(
                displayedScoresWithUserName
                    .OrderBy(contest => -contest.Score)
                    .ToList()
            );
        }

    }
    else
    {
        return Results.NotFound();
    }
});

contestItems.MapPost("/submitScore", async (ScoreEntryDTO scoreEntryDTO, UniversalLeaderboardDb db, UserManager<LeaderBoardUser> userManager, ClaimsPrincipal principal) =>
    {
        var contest = await db.Contests.FindAsync(new Guid(scoreEntryDTO.ContestId));
        var user = await userManager.GetUserAsync(principal);

        if (contest != null && user != null && user.UserName != null)
        {
            var scoreEntry = new ScoreEntry()
            {
                ContestId = new Guid(scoreEntryDTO.ContestId),
                UserId = user.Id,
                Score = scoreEntryDTO.Score,
                Date = new DateTime()
            };


            db.ScoreEntries.Add(scoreEntry);
            contest.ScoreEntries.Add(scoreEntry.Id);

            var displayedScore = await db.ScoreEntries
                .Where(entry => contest.DisplayedScores.Contains(entry.Id))
                .Where(entry => entry.UserId == user.Id).SingleOrDefaultAsync();

            if (displayedScore != null)
            {

                switch (contest.RankingType)
                {
                    case RankingType.HighScore:
                        if (contest.RankingOrder == RankingOrder.Descending)
                        {
                            if (displayedScore.Score > scoreEntryDTO.Score)
                            {
                                contest.DisplayedScores.Remove(displayedScore.Id);
                                contest.DisplayedScores.Add(scoreEntry.Id);
                            }
                        }

                        if (contest.RankingOrder == RankingOrder.Ascending)
                        {
                            if (displayedScore.Score < scoreEntryDTO.Score)
                            {
                                contest.DisplayedScores.Remove(displayedScore.Id);
                                contest.DisplayedScores.Add(scoreEntry.Id);
                            }
                        }
                        break;

                    case RankingType.Incremental:
                        displayedScore.Score += scoreEntry.Score;
                        displayedScore.RelatedScoreEntries.Add(scoreEntry.Id);
                        break;

                    case RankingType.Decremental:
                        displayedScore.Score -= scoreEntry.Score;
                        displayedScore.RelatedScoreEntries.Add(scoreEntry.Id);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                contest.DisplayedScores.Add(scoreEntry.Id);
            }

            await db.SaveChangesAsync();

            ScoreEntryResponseModel scoreEntryWithUserName = new()
            {
                Id = scoreEntry.Id,
                ContestId = scoreEntry.ContestId,
                UserId = scoreEntry.UserId,
                Score = scoreEntry.Score,
                Date = scoreEntry.Date,
                UserName = user?.UserName ?? "N/A"
            };

            return Results.Ok(scoreEntryWithUserName);
        }

        return Results.NotFound();

    }
).RequireAuthorization();

contestItems.MapPost("/", async (ContestCreateRequestModel contest, UniversalLeaderboardDb db, ApplicationDbContext appdb, UserManager<LeaderBoardUser> userManager, ClaimsPrincipal principal) =>
{
    var loggedInUser = await userManager.GetUserAsync(principal);
    var user = await appdb.Users.FindAsync(loggedInUser?.Id);

    if (user != null)
    {

        var newContest = new Contest
        {
            Name = contest.Name,
            Description = contest.Description,
            RankingOrder = (RankingOrder)contest.RankingOrder,
            RankingType = (RankingType)contest.RankingType,
            ScoreType = (ScoreType)contest.ScoreType,
            Active = false,
            CreatedDate = new DateTime()
        };

        newContest.Admin.Add(user.Id);

        db.Contests.Add(newContest);

        user.contestIds.Add(newContest.Id);

        await db.SaveChangesAsync();
        await appdb.SaveChangesAsync();


        return Results.Created($"/{newContest.Id}", newContest);
    }
    else
    {
        return Results.NotFound();
    }

}).RequireAuthorization();


contestItems.MapDelete("/{id}", async (string id, UniversalLeaderboardDb db) =>
{

    if (await db.Contests.FindAsync(new Guid(id)) is Contest contest)
    {
        db.Contests.Remove(contest);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();