namespace Web.Models;

/// <summary>
/// ورودی دکمه‌ی حذف مشترک در فهرست‌ها.
/// </summary>
/// <param name="Id">شناسه‌ی رکورد</param>
/// <param name="ConfirmText">متن تأیید حذف؛ نام رکورد داخل آن قرار می‌گیرد تا کاربر بداند دقیقاً چه چیزی حذف می‌شود.</param>
public record DeleteButtonModel(object Id, string ConfirmText);
