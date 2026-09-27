using Application.Features.Definition.GenericRepository;
using Application.Dto;
using Application.Features.Definition.GenericRepository;
using Domain.Entity;

namespace Application.Features.Definition.GoodsReceipt_Service
{
    /// <summary>
    /// سرویس رسیدهای ورودی انبار.
    /// منطق تأیید رسید و افزایش موجودی در <see cref="ApproveReceiptAsync"/> قرار دارد
    /// و باید همیشه داخل تراکنش دیتابیسی اجرا شود.
    /// </summary>
    public interface IGoodsReceiptService : IGenericRepository<GoodsReceipt>
    {
        Task<GoodsReceipt> GetByInvoiceNumberAsync(string invoiceNumber);

        Task<IEnumerable<GoodsReceipt>> GetByProductIdAsync(Guid productId);

        Task<IEnumerable<GoodsReceipt>> GetBySupplierIdAsync(int supplierId);

        Task<IEnumerable<GoodsReceipt>> GetByWarehouseIdAsync(int warehouseId);

        Task<IEnumerable<GoodsReceipt>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<List<GoodsReceiptDto>> GetTopApprovedReceiptsAsync(int topCount);

        Task<GoodsReceipt> GetReceiptWithDetailsAsync(int receiptId);

        Task<decimal> GetTotalNetPriceByProductAsync(Guid productId);

        /// <summary>
        /// تأیید رسید و افزایش موجودی محصول و انبار.
        /// موجودی با دستور SQL اتمیک به‌روزرسانی می‌شود تا تأیید همزمان دو کاربر
        /// باعث گم شدن یکی از تغییرات نشود.
        /// </summary>
        Task<(bool Success, string Message)> ApproveReceiptAsync(int receiptId, string currentUserId);

        Task<bool> CancelReceiptAsync(int receiptId);

        Task<List<GoodsReceiptDto>> GetTopReceiptsWithDetailsAsync(int topCount);

        /// <summary>جستجو در رسیدهای در انتظار تأیید (وضعیت 0).</summary>
        Task<List<GoodsReceiptDto>> SearchReceiptsAsync(string keyword, int topCount = 50);

        /// <summary>جستجو در رسیدهای تأییدشده (وضعیت 1).</summary>
        Task<List<GoodsReceiptDto>> SearchReceipts(string keyword, int topCount = 50);
    }
}
