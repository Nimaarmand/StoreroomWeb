using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class WarehouseFormModel
{
    public int WarehouseId { get; set; }

    [Required(ErrorMessage = "نام انبار الزامی است")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "محل")]
    [StringLength(200)]
    public string? Location { get; set; }

    [Display(Name = "حداقل ظرفیت")]
    public int Min { get; set; }

    [Display(Name = "حداکثر ظرفیت")]
    public int Max { get; set; }

    [Display(Name = "وضعیت")]
    [StringLength(50)]
    public string? Status { get; set; }
}
