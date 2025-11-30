using System.Linq;
using MovieVault.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace MovieVault.Api.Data;

public static class Seed
{
    public static void Initialize(MovieVaultContext db)
    {
        if (!db.Genres.Any())
        {
            db.Genres.AddRange(
                new Genre { Name = "Action" },
                new Genre { Name = "Drama" },
                new Genre { Name = "Sci-Fi" }
            );
            db.SaveChanges();
        }

        if (!db.Movies.Any())
        {
            var action = db.Genres.First(g => g.Name == "Action");
            var drama  = db.Genres.First(g => g.Name == "Drama");

            db.Movies.AddRange(
                new Movie { Title = "Inception",       Price = 299, ReleaseDate = new DateOnly(2010, 7, 16), GenreId = action.Id },
                new Movie { Title = "The Dark Knight",  Price = 349, ReleaseDate = new DateOnly(2008, 7, 18), GenreId = action.Id },
                new Movie { Title = "The Godfather",    Price = 249, ReleaseDate = new DateOnly(1972, 3, 24), GenreId = drama.Id }
            );
            db.SaveChanges();
        }
    }
}
