using Application.Features.Definition.Context;

namespace Application.Features.Implementation.Common
{
    /// <summary>
    /// پیاده‌سازی <see cref="IDbLock"/> بر پایه یک نمونه SemaphoreSlim.
    /// این کلاس باید به صورت Scoped ثبت شود، نه Singleton.
    /// </summary>
    public class DbLock : IDbLock
    {
        public SemaphoreSlim Semaphore { get; } = new SemaphoreSlim(1, 1);
    }
}
