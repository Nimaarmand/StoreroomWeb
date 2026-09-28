using System.ComponentModel.DataAnnotations;

namespace Web.Models;

/// <summary>
/// فرم ایجاد یا ویرایش کاربر به همراه نقش‌های انتخابی.
/// </summary>
public class UserFormModel
{
    public string? Id { get; set; }

    [Display(Name = "نام کاربری")]
    [Required(ErrorMessage = "نام کاربری الزامی است")]
    [StringLength(50, ErrorMessage = "نام کاربری نباید بیشتر از ۵۰ نویسه باشد")]
    public string UserName { get; set; } = string.Empty;

    [Display(Name = "نام و نام خانوادگی")]
    [Required(ErrorMessage = "نام و نام خانوادگی الزامی است")]
    [StringLength(50, ErrorMessage = "نام نباید بیشتر از ۵۰ نویسه باشد")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "ایمیل")]
    [EmailAddress(ErrorMessage = "ایمیل معتبر نیست")]
    [StringLength(150, ErrorMessage = "ایمیل نباید بیشتر از ۱۵۰ نویسه باشد")]
    public string? Email { get; set; }

    [Display(Name = "شماره تماس")]
    [Phone(ErrorMessage = "شماره تماس معتبر نیست")]
    [StringLength(20, ErrorMessage = "شماره تماس نباید بیشتر از ۲۰ نویسه باشد")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "وضعیت")]
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// نقش‌های انتخاب‌شده. فقط در ویرایش خوانده می‌شود؛ هنگام ایجاد، مقدار پیش‌فرض
    /// از <see cref="DefaultRole"/> می‌آید.
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// نقش‌ها به شکل تک‌مقداری می‌آیند، پس چک‌باکس‌ها با نام یکسان چند مقدار
    /// می‌فرستند و مدل‌باندینگ آن‌ها را به لیست نمی‌ریزد. این پراپرتی
    /// فقط برای پر کردن دستی همین لیست به کار می‌رود.
    /// </summary>
    public string[]? RolesSubmitted { get; set; }

    /// <summary>نقش‌های موجود برای نمایش در چک‌باکس‌ها.</summary>
    public List<string> AvailableRoles { get; set; } = new();

    /// <summary>رمز عبور؛ فقط هنگام ایجاد کاربر الزامی است.</summary>
    [Display(Name = "رمز عبور")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "رمز عبور باید بین ۶ تا ۱۰۰ نویسه باشد")]
    public string? Password { get; set; }

    [Display(Name = "تکرار رمز عبور")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "رمز عبور و تکرار آن یکسان نیست")]
    public string? ConfirmPassword { get; set; }

    /// <summary>کاربر در حال ویرایش است یا ایجاد.</summary>
    public bool IsEdit => !string.IsNullOrEmpty(Id);

    /// <summary>نقش پیش‌فرض برای کاربر جدید.</summary>
    public const string DefaultRole = "User";
}
