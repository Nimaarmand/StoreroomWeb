using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "نام کاربری را وارد کنید.")]
    [Display(Name = "نام کاربری")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "رمز عبور را وارد کنید.")]
    [Display(Name = "رمز عبور")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "مرا به خاطر بسپار")]
    public bool RememberMe { get; set; }
}
