public record User(int Id, string Name, string Email);
public record Order(int Id, int UserId, string Product, decimal Price);
public record UserOrderJoin(int? Id, string? Name, string? Email, int? OrderId, int? UserId, string? Product, decimal? Price);

public class Database
{
    public List<User> Users { get; set; } = [];
    public List<Order> Orders { get; set; } = [];
}