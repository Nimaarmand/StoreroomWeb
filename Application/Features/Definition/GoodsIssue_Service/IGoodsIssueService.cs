using Application.Features.Definition.GenericRepository;
using Application.Dto;
using Domain.Entity;

namespace Application.Features.Definition.GoodsIssue_Service
{
    /// <summary>
    /// Ø³Ø±ÙˆÛŒØ³ Ø­ÙˆØ§Ù„Ù‡â€ŒÙ‡Ø§ÛŒ Ø®Ø±ÙˆØ¬ÛŒ Ø§Ù†Ø¨Ø§Ø±.
    /// Ù…Ù†Ø·Ù‚ ØªØ£ÛŒÛŒØ¯ Ø­ÙˆØ§Ù„Ù‡ Ùˆ Ú©Ø§Ù‡Ø´ Ù…ÙˆØ¬ÙˆØ¯ÛŒ Ø¯Ø± <see cref="ApproveIssueAsync"/> Ù‚Ø±Ø§Ø± Ø¯Ø§Ø±Ø¯.
    /// </summary>
    public interface IGoodsIssueService : IGenericRepository<GoodsIssue>
    {
        Task<GoodsIssue?> GetByInvoiceNumberAsync(string invoiceNumber);

        Task<IEnumerable<GoodsIssue>> GetByProductIdAsync(Guid productId);

        Task<IEnumerable<GoodsIssue>> GetByCustomerIdAsync(int customerId);

        Task<IEnumerable<GoodsIssue>> GetByWarehouseIdAsync(int warehouseId);

        Task<IEnumerable<GoodsIssue>> GetByTypeAsync(IssueType type);

        Task<IEnumerable<GoodsIssue>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<GoodsIssue?> GetIssueWithDetailsAsync(int issueId);

        Task<decimal> GetTotalQuantityByProductAsync(Guid productId);

        /// <summary>
        /// ØªØ£ÛŒÛŒØ¯ Ø­ÙˆØ§Ù„Ù‡ Ùˆ Ú©Ø§Ù‡Ø´ Ù…ÙˆØ¬ÙˆØ¯ÛŒ Ù…Ø­ØµÙˆÙ„ Ùˆ Ø§Ù†Ø¨Ø§Ø±.
        /// Ù…ÙˆØ¬ÙˆØ¯ÛŒ Ø¨Ø§ Ø¯Ø³ØªÙˆØ± SQL Ø§ØªÙ…ÛŒÚ© Ø¨Ù‡â€ŒØ±ÙˆØ²Ø±Ø³Ø§Ù†ÛŒ Ù…ÛŒâ€ŒØ´ÙˆØ¯ Ùˆ Ù¾ÛŒØ´ Ø§Ø² Ú©Ø³Ø±ØŒ Ú©Ø§ÙÛŒ Ø¨ÙˆØ¯Ù†
        /// Ù…ÙˆØ¬ÙˆØ¯ÛŒ Ø¯Ø± Ù‡Ù…Ø§Ù† ØªØ±Ø§Ú©Ù†Ø´ Ø¨Ø±Ø±Ø³ÛŒ Ù…ÛŒâ€ŒÚ¯Ø±Ø¯Ø¯.
        /// </summary>
        Task<(bool Success, string Message)> ApproveIssueAsync(int issueId, string currentUserId);

        Task<bool> CancelIssueAsync(int issueId);

        Task<List<GoodsIssueDto>> GetApprovedIssuesDtoAsync(int topCount = 50);

        Task<List<GoodsIssueDto>> GetTopPendingIssuesAsync(int topCount);

        Task<List<GoodsIssueDto>> GetTopIssuesWithDetailsAsync(int topCount);

        /// <summary>Ø¬Ø³ØªØ¬Ùˆ Ø¯Ø± Ø­ÙˆØ§Ù„Ù‡â€ŒÙ‡Ø§ÛŒ Ø¯Ø± Ø§Ù†ØªØ¸Ø§Ø± ØªØ£ÛŒÛŒØ¯.</summary>
        Task<List<GoodsIssueDto>> SearchIssuesAsync(string keyword, int topCount = 50);

        /// <summary>Ø¬Ø³ØªØ¬Ùˆ Ø¯Ø± Ø­ÙˆØ§Ù„Ù‡â€ŒÙ‡Ø§ÛŒ ØªØ£ÛŒÛŒØ¯Ø´Ø¯Ù‡.</summary>
        Task<List<GoodsIssueDto>> SearchIssues(string keyword, int topCount = 50);

        Task<List<GoodsIssueDto>> SearchPendingIssuesAsync(string keyword, int topCount = 50);
    }
}
