namespace Web.Infrastructure;

/// <summary>
/// ساعت سیستم به صورت یک وابستگی تزریق‌شده. وجودش باعث می‌شود منطق
/// موجودی قابل تست باشد بدون اینکه مجبور شویم ساعت واقعی را شبیه‌سازی کنیم.
/// </summary>
public interface IClock
{
    DateTime Now { get; }
    DateTime Today { get; }
}

public sealed class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
    public DateTime Today => DateTime.Today;
}
