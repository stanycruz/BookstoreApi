using System.ComponentModel.DataAnnotations;

namespace BookstoreApi.Domain.Entities;

public class Grocery
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;
    public Guid OwnerId { get; set; }

    public User? Owner { get; set; }

    public ICollection<User>? Maintainers { get; set; }
}
