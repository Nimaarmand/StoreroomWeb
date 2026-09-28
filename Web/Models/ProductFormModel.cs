using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class ProductFormModel
{
    public Guid ProductId { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "نام کالا الزامی است")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "دسته‌بندی")]
    public int? CategoryId { get; set; }

    [Required(ErrorMessage = "واحد شمارش الزامی است")]
    [StringLength(20)]
    public string BaseUnit { get; set; } = string.Empty;

    [Display(Name = "بارکد")]
    [StringLength(100)]
    public string? Barcode { get; set; }

    [Display(Name = "موجودی اولیه")]
    public int? Number { get; set; }

    [Display(Name = "حداقل موجودی")]
    public decimal? MinStockLevel { get; set; }

    [Display(Name = "حداکثر موجودی")]
    public decimal? MaxStockLevel { get; set; }

    [Display(Name = "وزن")]
    public decimal? Weight { get; set; }

    [Display(Name = "تاریخ تولید")]
    public DateTime? ProductionDate { get; set; }

    [Display(Name = "تاریخ انقضا")]
    public DateTime? ExpirationDate { get; set; }

    [Display(Name = "توضیحات")]
    [StringLength(500)]
    public string? Description { get; set; }
}
