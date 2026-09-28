namespace Web.Models;

/// <summary>گزینه لیست‌های کشویی (انبار، تأمین‌کننده، مشتری).</summary>
public class LookupItem
{
    public int Value { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool Disabled { get; set; }
}

/// <summary>
/// گزینه لیست کالاها. موجودی و واحد پایه همراهش می‌آید تا فرم بدون
/// درخواست اضافه به سمت سرور، واحد و موجودی را به کاربر نشان دهد.
/// </summary>
public class ProductLookupItem
{
    public Guid Value { get; set; }
    public string Text { get; set; } = string.Empty;
    public string BaseUnit { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public int? Stock { get; set; }
    public bool IsActive { get; set; } = true;
}
