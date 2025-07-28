using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookstoreApi.Domain.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string KeycloakId { get; set; } = null!; // sub do token

    public string Role { get; set; } = null!;

    public string? Name { get; set; }
    public string? Email { get; set; }

    public Guid? GroceryId { get; set; }
    public Grocery? Grocery { get; set; }
}
