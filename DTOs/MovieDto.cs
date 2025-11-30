using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.DTOs;

public record MovieDto
(
    int Id,
    string Title,
    decimal Price,
    DateOnly ReleaseDate,
    int GenreId,
    string GenreName
);
