using Application.Features.Definition.GenericRepository;
using Application.Dto;
using Domain.Entity;

namespace Application.Features.Definition.GoodsReceipt_Service
{
    /// <summary>
    /// Ø³Ø±ÙˆÛŒØ³ Ø±Ø³ÛŒØ¯Ù‡Ø§ÛŒ ÙˆØ±ÙˆØ¯ÛŒ Ø§Ù†Ø¨Ø§Ø±.
    /// Ù…Ù†Ø·Ù‚ ØªØ£ÛŒÛŒØ¯ Ø±Ø³ÛŒØ¯ Ùˆ Ø§ÙØ²Ø§ÛŒØ´ Ù…ÙˆØ¬ÙˆØ¯ÛŒ Ø¯Ø± <see cref="ApproveReceiptAsync"/> Ù‚Ø±Ø§Ø± Ø¯Ø§Ø±Ø¯
    /// Ùˆ Ø¨Ø§ÛŒØ¯ Ù‡Ù…ÛŒØ´Ù‡ Ø¯Ø§Ø®Ù„ ØªØ±Ø§Ú©Ù†Ø´ Ø¯ÛŒØªØ§Ø¨ÛŒØ³ÛŒ Ø§Ø¬Ø±Ø§ Ø´ÙˆØ¯.
    /// </summary>
    public interface IGoodsReceiptService : IGenericRepository<GoodsReceipt>
    {
        Task<GoodsReceipt?> GetByInvoiceNumberAsync(string invoiceNumber);

        Task<IEnumerable<GoodsReceipt>> GetByProductIdAsync(Guid productId);

        Task<IEnumerable<GoodsReceipt>> GetBySupplierIdAsync(int supplierId);

        Task<IEnumerable<GoodsReceipt>> GetByWarehouseIdAsync(int warehouseId);

        Task<IEnumerable<GoodsReceipt>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<List<GoodsReceiptDto>> GetTopApprovedReceiptsAsync(int topCount);

        Task<GoodsReceipt?> GetReceiptWithDetailsAsync(int receiptId);

        Task<decimal> GetTotalNetPriceByProductAsync(Guid productId);

        /// <summary>
        /// ØªØ£ÛŒÛŒØ¯ Ø±Ø³ÛŒØ¯ Ùˆ Ø§ÙØ²Ø§ÛŒØ´ Ù…ÙˆØ¬ÙˆØ¯ÛŒ Ù…Ø­ØµÙˆÙ„ Ùˆ Ø§Ù†Ø¨Ø§Ø±.
        /// Ù…ÙˆØ¬ÙˆØ¯ÛŒ Ø¨Ø§ Ø¯Ø³ØªÙˆØ± SQL Ø§ØªÙ…ÛŒÚ© Ø¨Ù‡â€ŒØ±ÙˆØ²Ø±Ø³Ø§Ù†ÛŒ Ù…ÛŒâ€ŒØ´ÙˆØ¯ ØªØ§ ØªØ£ÛŒÛŒØ¯ Ù‡Ù…Ø²Ù…Ø§Ù† Ø¯Ùˆ Ú©Ø§Ø±Ø¨Ø±
        /// Ø¨Ø§Ø¹Ø« Ú¯Ù… Ø´Ø¯Ù† ÛŒÚ©ÛŒ Ø§Ø² ØªØºÛŒÛŒØ±Ø§Øª Ù†Ø´ÙˆØ¯.
        /// </summary>
        Task<(bool Success, string Message)> ApproveReceiptAsync(int receiptId, string currentUserId);

        Task<bool> CancelReceiptAsync(int receiptId);

        Task<List<GoodsReceiptDto>> GetTopReceiptsWithDetailsAsync(int topCount);

        /// <summary>Ø¬Ø³ØªØ¬Ùˆ Ø¯Ø± Ø±Ø³ÛŒØ¯Ù‡Ø§ÛŒ Ø¯Ø± Ø§Ù†ØªØ¸Ø§Ø± ØªØ£ÛŒÛŒØ¯ (ÙˆØ¶Ø¹ÛŒØª 0).</summary>
        Task<List<GoodsReceiptDto>> SearchReceiptsAsync(string keyword, int topCount = 50);

        /// <summary>Ø¬Ø³ØªØ¬Ùˆ Ø¯Ø± Ø±Ø³ÛŒØ¯Ù‡Ø§ÛŒ ØªØ£ÛŒÛŒØ¯Ø´Ø¯Ù‡ (ÙˆØ¶Ø¹ÛŒØª 1).</summary>
        Task<List<GoodsReceiptDto>> SearchReceipts(string keyword, int topCount = 50);
    }
}
