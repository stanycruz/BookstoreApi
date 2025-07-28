using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreApi.Domain.Entities;

public class Grocery
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;

    [ForeignKey("Owner")]
    public Guid OwnerId { get; set; }

    public User Owner { get; set; } = null!;

    public ICollection<User>? Maintainers { get; set; }
}
