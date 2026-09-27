namespace Web.Infrastructure;

/// <summary>
/// نام سیاست‌های مجوزدهی در یک جا جمع شده تا اشتباه تایپی در
/// [Authorize(Policy = ...)] پیش نیاید.
/// </summary>
public static class AuthPolicies
{
    /// <summary>فقط مدیر سیستم</summary>
    public const string AdminOnly = "AdminOnly";

    /// <summary>مدیر سیستم یا سرپرست</summary>
    public const string ManagerOrAdmin = "ManagerOrAdmin";

    /// <summary>ثبت رسید و حواله - همه نقش‌ها</summary>
    public const string CanManageInventory = "CanManageInventory";

    /// <summary>ویرایش و حذف اطلاعات پایه - مدیر و سرپرست</summary>
    public const string CanEditMasterData = "CanEditMasterData";

    /// <summary>تأیید نهایی عملیات مالی - مدیر و سرپرست</summary>
    public const string CanApprove = "CanApprove";
}
