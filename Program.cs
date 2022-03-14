using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("MyTodoList"));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

app.MapGet("/", () => "Home Route");

app.MapGet("/todoitems", async (TodoDb db) =>
    await db.Todoos.ToListAsync());

app.MapGet("/todoitems/complete", async (TodoDb db) =>
    await db.Todoos.Where(x => x.IsComplete == true).ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDb db) =>
{
    var found = await db.Todoos.FindAsync(id);
    if (found == null) return Results.NotFound();
    return Results.Ok(found);
});
//await db.Todos.FindAsync(id)
//        is Todo todo
//            ? Results.Ok(todo)
//            : Results.NotFound());


app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
{
    db.Todoos.Add(todo);
    await db.SaveChangesAsync();

    // return Results.Created($"/todoitems/{todo.Id}", todo);
    return Results.Ok($"item {todo.Name} is added");
});

app.MapPut("/todoitems/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    var found = await db.Todoos.FindAsync(id);
    if (found == null) return Results.NotFound();

    found.Name = inputTodo.Name;
    found.IsComplete = inputTodo.IsComplete;
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
    var found = await db.Todoos.FindAsync(id);
    if (found is null) return Results.NotFound();

    db.Todoos.Remove(found);
    await db.SaveChangesAsync();
    return Results.Ok(found);
});

app.Run();


class Todo
{
    public int Id { get; set; }
    public string? Name { get; set; } = "Default Value";
    public bool IsComplete { get; set; }
}

class TodoDb : DbContext
{
    public TodoDb(DbContextOptions<TodoDb> options)
        : base(options) { }
    public DbSet<Todo> Todoos { get; set; }
    // public DbSet<Todo> Todoos => Set<Todo>();
}


