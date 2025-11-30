using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Endpoints;   // <-- needed for MapGenres/MapMovies

var builder = WebApplication.CreateBuilder(args);

// EF Core
builder.Services.AddDbContext<MovieVaultContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "MovieVault API is running!");

// your endpoints
app.MapGenres();
app.MapMovies();

// auto-migrate + seed
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<MovieVaultContext>();
db.Database.Migrate();
Seed.Initialize(db);

app.Run();
