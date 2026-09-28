using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Web.Infrastructure;

/// <summary>
/// بایند تاریخ میلادی.
/// <para>
/// رابط کاربری فارسی است و <c>RequestLocalization</c> هم <c>fa-IR</c> را به‌عنوان
/// Culture جاری می‌گذارد. تقویم <c>fa-IR</c> شمسی است، بنابراین مدل‌بایندینگ پیش‌فرض
/// مقدار ISO یعنی <c>2026-09-28</c> را به‌عنوان سال ۱۴۰۵ شمسی می‌خواند و به سال ۲۶۴۵
/// میلادی تبدیل می‌کند. نتیجه: هر تاریخی که کاربر در <c>&lt;input type="date"&gt;</c>
/// انتخاب می‌کند «تاریخ آینده» یا نامعتبر تلقی می‌شد.
/// </para>
/// <para>
/// <c>&lt;input type="date"&gt;</c> همیشه ISO-8601 می‌فرستد، پس اول با Culture ثابت
/// Invariant خوانده می‌شود و فقط اگر شکست خورد به Culture جاری fallback می‌کنیم تا
/// تاریخ‌های شمسیِ تایپ‌شده دستی هم کار کنند.
/// </para>
/// </summary>
public sealed class GregorianDateModelBinder : IModelBinder
{
    private static readonly string[] AcceptedFormats =
    {
        "yyyy-MM-dd",
        "yyyy-MM-ddTHH:mm",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss.fff",
        "yyyy/MM/dd",
    };

    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueResult == ValueProviderResult.None)
            return Task.CompletedTask;

        var raw = valueResult.FirstValue;
        if (string.IsNullOrWhiteSpace(raw))
            return Task.CompletedTask;

        var text = raw.Trim();

        if (DateTime.TryParseExact(text, AcceptedFormats, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces, out var exact))
        {
            bindingContext.Result = ModelBindingResult.Success(exact);
            return Task.CompletedTask;
        }

        if (DateTime.TryParse(text, CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces, out var gregorian))
        {
            bindingContext.Result = ModelBindingResult.Success(gregorian);
            return Task.CompletedTask;
        }

        if (DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.None, out var current))
        {
            bindingContext.Result = ModelBindingResult.Success(current);
            return Task.CompletedTask;
        }

        bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
            "تاریخ وارد شده معتبر نیست.");
        return Task.CompletedTask;
    }
}

public sealed class GregorianDateModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        var type = context.Metadata.UnderlyingOrModelType;
        return type == typeof(DateTime) || type == typeof(DateTime?)
            ? new GregorianDateModelBinder()
            : null;
    }
}
