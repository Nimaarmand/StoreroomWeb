using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class SupplierFormModel
{
    public int SupplierId { get; set; }

    [Required(ErrorMessage = "نام تأمین‌کننده الزامی است")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "تلفن")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Display(Name = "کد اقتصادی")]
    [StringLength(20)]
    public string? EconomicCode { get; set; }

    [Display(Name = "نشانی")]
    [StringLength(500)]
    public string? Address { get; set; }
}
