using Web.Infrastructure;

namespace Web.Models;

/// <summary>
/// نگاشت کد وضعیت عددی به متن و رنگ.
/// در حال حاضر 0 = در انتظار تأیید، 1 = تأیید شده، 2 = لغو شده.
/// (تبدیل به enum در فاز بعدی انجام می‌شود.)
/// </summary>
public static class StatusBadge
{
    public static (string Text, string CssClass) Of(byte status) => status switch
    {
        0 => ("در انتظار تأیید", "badge-status-0"),
        1 => ("تأیید شده", "badge-status-1"),
        2 => ("لغو شده", "badge-status-2"),
        _ => ("نامشخص", "bg-secondary"),
    };

    public static string Text(byte status) => Of(status).Text;
    public static string Css(byte status) => Of(status).CssClass;
}
