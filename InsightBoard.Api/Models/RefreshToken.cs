namespace InsightBoard.Api.Models;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = Guid.NewGuid().ToString();
    public string JwtId { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    public DateTime ExpiryDate { get; set; }
    public bool Used { get; set; } = false;
    public bool Invalidated { get; set; } = false;
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;
}