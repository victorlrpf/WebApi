namespace BackendExample.Models;

public record RegisterRequest(string Email, string Password, string? FullName);
public record LoginRequest(string Email, string Password);

public class UserRecord
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!; // stored hashed
    public string? FullName { get; set; }
    public DateTime CreatedAt { get; set; }
}
