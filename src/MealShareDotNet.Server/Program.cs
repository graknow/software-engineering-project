using MealShareDotNet.Core.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Services
var connString = builder.Configuration.GetConnectionString("Recipe");
if (connString is null)
{
    throw new Exception("Connection string not found in configuration settings.  Exiting...");
}

var fullConnString = connString
.Replace("{AppDir}", AppDomain.CurrentDomain.BaseDirectory)
.Replace("{Sep}", Path.DirectorySeparatorChar.ToString());

Console.Write(fullConnString);

builder.Services.AddSingleton<IRecipeRepository>(new DbRecipeRepository(fullConnString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
