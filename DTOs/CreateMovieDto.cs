using System.ComponentModel.DataAnnotations;
namespace MovieVault.Api.DTOs;

public class CreateMovieDto
{
    [Required, StringLength(100)]
    public string Title { get; set; } = "";

    [Range(0, 10000)]
    public decimal Price { get; set; }

    [Required]
    public DateOnly ReleaseDate { get; set; }

    [Required]
    public int GenreId { get; set; }
}