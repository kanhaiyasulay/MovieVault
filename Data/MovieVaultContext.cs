using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Entities;

namespace MovieVault.Api.Data;

public class MovieVaultContext(DbContextOptions<MovieVaultContext> options) : DbContext(options)
{
    public DbSet<Genre> Genres => Set<Genre>();
    public DbSet<Movie> Movies => Set<Movie>();
}
