using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class CategoryFormModel
{
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "نام دسته‌بندی الزامی است")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "دسته والد")]
    public int? ParentId { get; set; }

    [Display(Name = "توضیحات")]
    [StringLength(500)]
    public string? Description { get; set; }
}
