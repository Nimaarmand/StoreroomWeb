using Microsoft.AspNetCore.Mvc;

namespace Web.Infrastructure
{
    /// <summary>
    /// کمک‌کننده برای ریدایرکت امن به بازگشت.
    /// </summary>
    public static class LocalUrl
    {
        /// <summary>
        /// اگر <paramref name="returnUrl"/> یک آدرس محلی معتبر باشد همان برگردانده می‌شود،
        /// در غیر این صورت آدرس اکشن fallback استفاده می‌شود.
        /// هرگز مقدار null یا خالی برنمی‌گرداند تا <c>Redirect</c> با استثنا مواجه نشود.
        /// </summary>
        public static string Resolve(IUrlHelper url, string? returnUrl, string action, string? controller = null)
        {
            if (!string.IsNullOrEmpty(returnUrl) && url.IsLocalUrl(returnUrl))
                return returnUrl;

            return url.Action(action, controller) ?? "/";
        }
    }
}
