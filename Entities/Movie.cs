namespace MovieVault.Api.Entities;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public decimal Price { get; set; }
    public DateOnly ReleaseDate { get; set; }

    public int GenreId { get; set; }

    // Genre -> foreign key
    public Genre? Genre { get; set; }
}
