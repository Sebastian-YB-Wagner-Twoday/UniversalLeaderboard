using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<StackDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "StackbagAPI";
    config.Title = "StackbagAPI v0.1";
    config.Version = "v0.1";
});
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "StackbagAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

var techItems = app.MapGroup("/techitems");

techItems.MapGet("/", async (StackDb db) =>
    await db.Techs.ToListAsync());

techItems.MapGet("/frontend", async (StackDb db) =>
    await db.Techs.Where(t => t.TechType == TechType.Frontend).ToListAsync());

techItems.MapGet("/{id}", async (int id, StackDb db) =>
    await db.Techs.FindAsync(id)
        is Tech tech
            ? Results.Ok(tech)
            : Results.NotFound());

techItems.MapPost("/", async (Tech tech, StackDb db) =>
{
    db.Techs.Add(tech);
    await db.SaveChangesAsync();

    return Results.Created($"/{tech.Id}", tech);
});

techItems.MapPut("/{id}", async (int id, Tech inputTodo, StackDb db) =>
{
    var tech = await db.Techs.FindAsync(id);

    if (tech is null) return Results.NotFound();

    tech.Name = inputTodo.Name;
    tech.TechType = inputTodo.TechType;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

techItems.MapDelete("/{id}", async (int id, StackDb db) =>
{
    if (await db.Techs.FindAsync(id) is Tech tech)
    {
        db.Techs.Remove(tech);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    return Results.NotFound();
});

app.Run();