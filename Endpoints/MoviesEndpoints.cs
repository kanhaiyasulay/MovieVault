using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.DTOs;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Endpoints;

public static class MoviesEndpoints
{
    public static IEndpointRouteBuilder MapMovies(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/movies");

        // GET /movies?search=&genreId=&sort=price|-price|date|-date&page=1&pageSize=10
        group.MapGet("/", async (
            MovieVaultContext db,
            string? search,
            int? genreId,
            string? sort,
            int page = 1,
            int pageSize = 10
        ) =>
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var query = db.Movies
                .AsNoTracking()
                .Include(m => m.Genre)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(m => m.Title.Contains(search));

            if (genreId is not null)
                query = query.Where(m => m.GenreId == genreId);

            query = sort switch
            {
                "price"   => query.OrderBy(m => m.Price),
                "-price"  => query.OrderByDescending(m => m.Price),
                "date"    => query.OrderBy(m => m.ReleaseDate),
                "-date"   => query.OrderByDescending(m => m.ReleaseDate),
                _         => query.OrderBy(m => m.Id)
            };

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MovieDto(
                    m.Id, m.Title, m.Price, m.ReleaseDate, m.GenreId, m.Genre!.Name
                ))
                .ToListAsync();

            return Results.Ok(new { total, page, pageSize, items });
        });

        group.MapGet("/{id:int}", async (int id, MovieVaultContext db) =>
        {
            var m = await db.Movies.AsNoTracking().Include(x => x.Genre).FirstOrDefaultAsync(x => x.Id == id);
            return m is null
                ? Results.NotFound()
                : Results.Ok(new MovieDto(m.Id, m.Title, m.Price, m.ReleaseDate, m.GenreId, m.Genre!.Name));
        });

        group.MapPost("/", async (CreateMovieDto dto, MovieVaultContext db) =>
        {
            // Optional: ensure Genre exists
            var genreExists = await db.Genres.AnyAsync(g => g.Id == dto.GenreId);
            if (!genreExists) return Results.BadRequest(new { error = "Invalid GenreId." });

            var movie = new Movie
            {
                Title = dto.Title,
                Price = dto.Price,
                ReleaseDate = dto.ReleaseDate,
                GenreId = dto.GenreId
            };
            db.Movies.Add(movie);
            await db.SaveChangesAsync();

            return Results.Created($"/movies/{movie.Id}", new { movie.Id });
        });

        group.MapPut("/{id:int}", async (int id, UpdateMovieDto dto, MovieVaultContext db) =>
        {
            var movie = await db.Movies.FindAsync(id);
            if (movie is null) return Results.NotFound();

            movie.Title = dto.Title;
            movie.Price = dto.Price;
            movie.ReleaseDate = dto.ReleaseDate;
            movie.GenreId = dto.GenreId;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/{id:int}", async (int id, MovieVaultContext db) =>
        {
            var movie = await db.Movies.FindAsync(id);
            if (movie is null) return Results.NotFound();

            db.Movies.Remove(movie);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        return routes;
    }
}
