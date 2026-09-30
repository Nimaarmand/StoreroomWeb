namespace Web.Models;

/// <summary>
/// دادهٔ یک فاکتور تکی برای چاپ. عمداً ساده و مستقل از DTOهای دیتابیس است تا
/// قالب چاپ به ساختار موجودی وابسته نشود.
/// </summary>
public class InvoicePrintModel
{
    public string Title { get; set; } = string.Empty;
    public string? Number { get; set; }
    public DateTime Date { get; set; }

    /// <summary>طرف مقابل معامله: تأمین‌کننده برای ورود، مشتری برای خروج.</summary>
    public string? Counterparty { get; set; }

    public string? Warehouse { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Total { get; set; }
    public string? ApprovedBy { get; set; }
    public string? Description { get; set; }

    /// <summary>یادداشت اختیاری، مثلاً نوع سند خروجی.</summary>
    public string? Note { get; set; }
}
