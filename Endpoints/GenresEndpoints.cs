using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Endpoints;

public static class GenresEndpoints
{
    public static IEndpointRouteBuilder MapGenres(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/genres");

        // GET /genres
        group.MapGet("/", async (MovieVaultContext db) =>
            await db.Genres.AsNoTracking().ToListAsync());

        // GET /genres/{id}
        group.MapGet("/{id:int}", async (int id, MovieVaultContext db) =>
        {
            var genre = await db.Genres.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);
            return genre is null ? Results.NotFound() : Results.Ok(genre);
        });

        // POST /genres
        group.MapPost("/", async (Genre genre, MovieVaultContext db) =>
        {
            if (string.IsNullOrWhiteSpace(genre.Name))
                return Results.BadRequest(new { error = "Name is required." });

            db.Genres.Add(genre);
            await db.SaveChangesAsync();
            return Results.Created($"/genres/{genre.Id}", genre);
        });

        // PUT /genres/{id}
        group.MapPut("/{id:int}", async (int id, Genre input, MovieVaultContext db) =>
        {
            var genre = await db.Genres.FindAsync(id);
            if (genre is null) return Results.NotFound();
            if (string.IsNullOrWhiteSpace(input.Name))
                return Results.BadRequest(new { error = "Name is required." });

            genre.Name = input.Name;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        // DELETE /genres/{id}
        group.MapDelete("/{id:int}", async (int id, MovieVaultContext db) =>
        {
            var genre = await db.Genres.FindAsync(id);
            if (genre is null) return Results.NotFound();

            db.Genres.Remove(genre);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return routes;
    }
}
