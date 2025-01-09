using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UniversalLeaderboardDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "UniversalLeaderboardAPI";
    config.Title = "UniversalLeaderboardAPI v0.1";
    config.Version = "v0.1";
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

var users = app.MapGroup("/users");

users.MapGet("/", async (UniversalLeaderboardDb db) =>
    await db.Users.ToListAsync());

users.MapGet("/{id}", async (string id, UniversalLeaderboardDb db) =>
    await db.Users.FindAsync(new Guid(id))
        is User user
            ? Results.Ok(user)
            : Results.NotFound());

/**
* get list of all contests related to user
*/
users.MapGet("/{id}/contests/{pagination}", async (string id, int pagination, UniversalLeaderboardDb db) =>
{
    var user = await db.Users.FindAsync(new Guid(id));

    var contests = db.Contests
                        .Where(contest => user.contestIds.Contains(contest.Id))
                        .OrderBy(contest => contest.CreatedDate)
                        .Skip(pagination * 10)
                        .Take(10)
                        .ToList();
    if (user != null)
    {
        ICollection<ContestResponseModel> returnContests = [];
        foreach (var contest in contests)
        {

            var displayedScores = db.ScoreEntries.Where(entry => contest.DisplayedScores.Contains(entry.Id)).ToList();

            returnContests.Add(new ContestResponseModel()
            {
                Name = contest.Name,
                Id = contest.Id,
                CreatedDate = contest.CreatedDate,
                Description = contest.Description,
                RankingType = contest.RankingType,
                RankingOrder = contest.RankingOrder,
                ScoreType = contest.ScoreType,
                DisplayedScores = displayedScores

            });
        };

        return Results.Ok(returnContests);
    }
    else
    {
        return Results.NotFound();
    }

});


users.MapPost("/", async (UserDTO userDTO, UniversalLeaderboardDb db) =>
{
    var hasUser = await db.Users.AnyAsync(user => user.Email == userDTO.Email);


    if (hasUser)
    {
        return Results.Ok(await db.Users.FirstAsync(user => user.Email == userDTO.Email));
    }
    else
    {
        var newUser =
        new User
        {
            UserName = userDTO.UserName,
            Email = userDTO.Email
        };

        db.Users.Add(newUser);
        await db.SaveChangesAsync();
        return Results.Created($"/{newUser.UserName}", newUser);
    }

});



var contestItems = app.MapGroup("/contest");

contestItems.MapGet("/", async (UniversalLeaderboardDb db) =>
    await db.Contests.ToListAsync());

contestItems.MapGet("/{id}", async (string id, UniversalLeaderboardDb db) =>
{
    var contest = await db.Contests.FindAsync(new Guid(id));

    if (contest != null)
    {
        var displayedScores = db.ScoreEntries.Where(entry => contest.DisplayedScores.Contains(entry.Id)).ToList();

        return Results.Ok(new ContestResponseModel()
        {
            Name = contest.Name,
            Id = contest.Id,
            CreatedDate = contest.CreatedDate,
            Description = contest.Description,
            RankingType = contest.RankingType,
            RankingOrder = contest.RankingOrder,
            DisplayedScores = displayedScores
        });
    }
    else
    {
        return Results.NotFound();
    }
});

contestItems.MapPost("/submitScore", async (ScoreEntryDTO scoreEntryDTO, UniversalLeaderboardDb db) =>
    {
        var contest = await db.Contests.FindAsync(new Guid(scoreEntryDTO.ContestId));
        var user = await db.Users.FindAsync(new Guid(scoreEntryDTO.UserId));

        if (contest != null && user != null)
        {
            var scoreEntry = new ScoreEntry()
            {
                ContestId = new Guid(scoreEntryDTO.ContestId),
                UserId = new Guid(scoreEntryDTO.UserId),
                UserName = user.UserName,
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
                        displayedScore.Score += scoreEntry.Score;
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

            return Results.Ok();
        }

        return Results.NotFound();

    }
);

contestItems.MapPost("/", async (ContestCreateRequestModel contest, UniversalLeaderboardDb db) =>
{
    var user = await db.Users.FindAsync(new Guid(contest.AdminId));

    if (user != null)
    {
        Console.WriteLine(contest);


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

        return Results.Created($"/{newContest.Id}", newContest);
    }
    else
    {
        return Results.NotFound();
    }

});


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