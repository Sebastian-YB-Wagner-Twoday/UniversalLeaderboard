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

users.MapGet("/{id}", async (int id, UniversalLeaderboardDb db) =>
    await db.Users.FindAsync(id)
        is User user
            ? Results.Ok(user)
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

contestItems.MapGet("/{id}", async (int id, UniversalLeaderboardDb db) =>
    await db.Contests.FindAsync(id)
        is Contest contest
            ? Results.Ok(contest)
            : Results.NotFound());


contestItems.MapPost("/", async (ContestDTO contest, UniversalLeaderboardDb db) =>
{

    var newContest = new Contest
    {
        Name = contest.Name,
        Description = contest.Description,
        RankingOrder = contest.RankingOrder,
        RankingType = contest.RankingType,
    };

    db.Contests.Add(newContest);
    await db.SaveChangesAsync();

    return Results.Created($"/{newContest.Id}", newContest);

});


contestItems.MapDelete("/{id}", async (int id, UniversalLeaderboardDb db) =>
{
    if (await db.Contests.FindAsync(id) is Contest contest)
    {
        db.Contests.Remove(contest);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();