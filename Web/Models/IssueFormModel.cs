using System.ComponentModel.DataAnnotations;
using Domain.Entity;

namespace Web.Models;

/// <summary>
/// فرم ثبت و ویرایش حواله خروج.
/// مانند رسید، وضعیت از فرم گرفته نمی‌شود و همیشه «در انتظار تأیید» است.
/// </summary>
public class IssueFormModel
{
    public int IssueId { get; set; }

    [Display(Name = "کالا")]
    [Required(ErrorMessage = "کالا را انتخاب کنید")]
    public Guid? ProductId { get; set; }

    [Display(Name = "تعداد خروج")]
    [Required(ErrorMessage = "تعداد خروج الزامی است")]
    [Range(1, int.MaxValue, ErrorMessage = "تعداد باید بزرگ‌تر از صفر باشد")]
    public int Quantity { get; set; }

    [Display(Name = "واحد شمارش")]
    [Required(ErrorMessage = "واحد شمارش الزامی است")]
    [StringLength(20)]
    public string Unit { get; set; } = string.Empty;

    [Display(Name = "قیمت فروش واحد")]
    [Range(0, double.MaxValue, ErrorMessage = "قیمت فروش نمی‌تواند منفی باشد")]
    public decimal? UnitSellingPrice { get; set; }

    [Display(Name = "نوع حواله")]
    public IssueType Type { get; set; } = IssueType.Sale;

    [Display(Name = "مشتری")]
    public int? CustomerId { get; set; }

    [Display(Name = "انبار مبدأ")]
    public int? WarehouseId { get; set; }

    [Display(Name = "شماره فاکتور")]
    [StringLength(50)]
    public string? InvoiceNumber { get; set; }

    [Display(Name = "توضیحات")]
    [StringLength(500)]
    public string? Description { get; set; }
}
