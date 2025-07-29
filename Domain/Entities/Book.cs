using System.ComponentModel.DataAnnotations;

namespace BookstoreApi.Domain.Entities;

public class Book
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Genre { get; set; } = null!;

    public Guid CreatedBy { get; set; }
}
