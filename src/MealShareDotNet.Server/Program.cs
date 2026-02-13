using MealShareDotNet.Core.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Services
var connString = builder.Configuration.GetConnectionString("Recipe")!;
builder.Services.AddSingleton<IRecipeRepository>(new DbRecipeRepository(connString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
