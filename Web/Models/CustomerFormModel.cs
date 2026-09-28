using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class CustomerFormModel
{
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "نام مشتری الزامی است")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "تلفن")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Display(Name = "نام بازار")]
    [StringLength(200)]
    public string? MarketName { get; set; }

    [Display(Name = "نشانی")]
    [StringLength(500)]
    public string? Address { get; set; }
}
