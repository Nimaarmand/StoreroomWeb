using System.ComponentModel.DataAnnotations;

namespace Web.Models;

/// <summary>
/// فرم ثبت و ویرایش رسید ورودی.
/// وضعیت از فرم گرفته نمی‌شود: هر رسیدی که از این فرم ثبت شود «در انتظار تأیید» است.
/// تغییر وضعیت فقط از مسیر تأیید انجام می‌شود، چون همان‌جاست که موجودی تغییر می‌کند.
/// </summary>
public class ReceiptFormModel
{
    public int ReceiptId { get; set; }

    [Display(Name = "کالا")]
    [Required(ErrorMessage = "کالا را انتخاب کنید")]
    public Guid? ProductId { get; set; }

    [Display(Name = "تعداد")]
    [Required(ErrorMessage = "تعداد الزامی است")]
    [Range(1, int.MaxValue, ErrorMessage = "تعداد باید بزرگ‌تر از صفر باشد")]
    public int Quantity { get; set; }

    [Display(Name = "قیمت واحد")]
    [Range(0, double.MaxValue, ErrorMessage = "قیمت واحد نمی‌تواند منفی باشد")]
    public decimal UnitPrice { get; set; }

    [Display(Name = "درصد مالیات")]
    [Range(0, 100, ErrorMessage = "درصد مالیات باید بین ۰ تا ۱۰۰ باشد")]
    public decimal TaxRate { get; set; }

    [Display(Name = "واحد شمارش")]
    [Required(ErrorMessage = "واحد شمارش الزامی است")]
    [StringLength(20)]
    public string Unit { get; set; } = string.Empty;

    [Display(Name = "انبار مقصد")]
    public int? WarehouseId { get; set; }

    // [Required] عمداً ندارد: با Required خطا با کلید خالی می‌رفت و فقط در
    // خلاصه‌ی بالای فرم دیده می‌شد. اعتبار سنجی در کنترلر انجام می‌شود تا پیام
    // کنار خود فیلد تأمین‌کننده بنشیند.
    [Display(Name = "تأمین‌کننده")]
    public int? SupplierId { get; set; }

    [Display(Name = "شماره فاکتور")]
    [Required(ErrorMessage = "شماره فاکتور الزامی است")]
    [StringLength(50, ErrorMessage = "شماره فاکتور نباید بیشتر از ۵۰ نویسه باشد")]
    public string InvoiceNumber { get; set; } = string.Empty;

    [Display(Name = "تاریخ فاکتور")]
    [Required(ErrorMessage = "تاریخ فاکتور الزامی است")]
    [DataType(DataType.Date)]
    public DateTime InvoiceDate { get; set; } = DateTime.Today;

    [Display(Name = "بارکد اسکن‌شده")]
    [StringLength(50)]
    public string? ScannedBarcode { get; set; }

    [Display(Name = "محل قفسه")]
    [StringLength(100)]
    public string? ShelfLocation { get; set; }

    [Display(Name = "شماره بچ")]
    [StringLength(50)]
    public string? BatchNumber { get; set; }

    [Display(Name = "توضیحات")]
    [StringLength(500)]
    public string? Description { get; set; }
}
