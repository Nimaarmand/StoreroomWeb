namespace Web.Models;

/// <summary>
/// یک ردیف از فهرست کاربران، شامل نقش‌های کاربر برای نمایش به صورت badge.
/// </summary>
public class UserListItem
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
}
