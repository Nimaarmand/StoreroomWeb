namespace Web.Infrastructure;

/// <summary>
/// تبدیل ارقام فارسی ↔ لاتین.
/// این کلاس عمداً static است: تبدیل رقم وضعیت دارد و به هیچ وابستگی‌ای نیاز
/// ندارد، بنابراین ثبت آن در DI فقط پیچیدگی اضافه می‌کرد.
/// </summary>
public static class PersianNumberConverter
{
    private const string PersianDigits = "۰۱۲۳۴۵۶۷۸۹";
    private const string ArabicDigits = "٠١٢٣٤٥٦٧٨٩";

    public static string ToPersianDigits(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var chars = value.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] >= '0' && chars[i] <= '9')
                chars[i] = PersianDigits[chars[i] - '0'];
            else
            {
                int a = ArabicDigits.IndexOf(chars[i]);
                if (a >= 0)
                    chars[i] = PersianDigits[a];
            }
        }
        return new string(chars);
    }

    public static string ToPersianDigits(decimal value, int decimals = 0)
        => ToPersianDigits(value.ToString($"N{decimals}", System.Globalization.CultureInfo.InvariantCulture));

    public static string ToPersianDigits(int value) => ToPersianDigits(value.ToString());

    /// <summary>تبدیل رقم‌های فارسی/عربی به لاتین، برای ذخیره در دیتابیس</summary>
    public static string ToEnglishDigits(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var chars = value.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            int p = PersianDigits.IndexOf(chars[i]);
            if (p >= 0)
                chars[i] = (char)('0' + p);
            else
            {
                int a = ArabicDigits.IndexOf(chars[i]);
                if (a >= 0)
                    chars[i] = (char)('0' + a);
            }
        }
        return new string(chars);
    }

    /// <summary>
    /// پاک‌سازی ورودی عددی کاربر: ارقام فارسی به لاتین، حذف جداکننده هزارگان و کاما.
    /// برای فیلدهایی که کاربر ممکن است با ارقام فارسی یا کاما پر کند.
    /// </summary>
    public static string NormalizeNumericInput(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var latin = ToEnglishDigits(value);

        var builder = new System.Text.StringBuilder(latin.Length);
        foreach (char c in latin)
        {
            if (char.IsDigit(c) || c == '.' || c == '-')
                builder.Append(c);
        }
        return builder.ToString();
    }
}
