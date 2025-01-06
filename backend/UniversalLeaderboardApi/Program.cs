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
    await db.Users.FindAsync(id)
        is User user
            ? Results.Ok(user)
            : Results.NotFound());

/**
* get list of all contests related to user
*/
users.MapGet("/{id}/contests/{pagination}", async (string id, int pagination, UniversalLeaderboardDb db) =>
    await db.Users.FindAsync(id)
        is User user
            ? Results.Ok(
                db.Contests
                .Where(contest => user.contestIds.Contains(contest.Id))
                .OrderBy(contest => contest.CreatedDate)
                .Skip(pagination * 10)
                .Take(10)
                .ToList())
            : Results.NotFound());


users.MapPost("/", async (UserDTO userDTO, UniversalLeaderboardDb db) =>
{
    var user = await db.Users.FindAsync(userDTO.UserName);

    if (user != null)
    {
        return Results.Ok(user);
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

    await db.Contests.FindAsync(new Guid(id))
        is Contest contest
            ? Results.Ok(contest)
            : Results.NotFound());


contestItems.MapPost("/", async (ContestDTO contest, UniversalLeaderboardDb db) =>
{
    var user = await db.Users.FindAsync(contest.AdminEmail);

    if (user != null)
    {
        Console.WriteLine(contest);


        var newContest = new Contest
        {
            Name = contest.Name,
            Description = contest.Description,
            RankingOrder = (RankingOrder)contest.RankingOrder,
            RankingType = (RankingType)contest.RankingType,
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