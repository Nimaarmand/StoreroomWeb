using Application.Features.Definition.GenericRepository;
using Application.Dto;
using Application.Features.Definition.GenericRepository;
using Domain.Entity;

namespace Application.Features.Definition.GoodsIssue_Service
{
    /// <summary>
    /// سرویس حواله‌های خروجی انبار.
    /// منطق تأیید حواله و کاهش موجودی در <see cref="ApproveIssueAsync"/> قرار دارد.
    /// </summary>
    public interface IGoodsIssueService : IGenericRepository<GoodsIssue>
    {
        Task<GoodsIssue> GetByInvoiceNumberAsync(string invoiceNumber);

        Task<IEnumerable<GoodsIssue>> GetByProductIdAsync(Guid productId);

        Task<IEnumerable<GoodsIssue>> GetByCustomerIdAsync(int customerId);

        Task<IEnumerable<GoodsIssue>> GetByWarehouseIdAsync(int warehouseId);

        Task<IEnumerable<GoodsIssue>> GetByTypeAsync(IssueType type);

        Task<IEnumerable<GoodsIssue>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task<GoodsIssue> GetIssueWithDetailsAsync(int issueId);

        Task<decimal> GetTotalQuantityByProductAsync(Guid productId);

        /// <summary>
        /// تأیید حواله و کاهش موجودی محصول و انبار.
        /// موجودی با دستور SQL اتمیک به‌روزرسانی می‌شود و پیش از کسر، کافی بودن
        /// موجودی در همان تراکنش بررسی می‌گردد.
        /// </summary>
        Task<(bool Success, string Message)> ApproveIssueAsync(int issueId, string currentUserId);

        Task<bool> CancelIssueAsync(int issueId);

        Task<List<GoodsIssueDto>> GetApprovedIssuesDtoAsync(int topCount = 50);

        Task<List<GoodsIssueDto>> GetTopPendingIssuesAsync(int topCount);

        Task<List<GoodsIssueDto>> GetTopIssuesWithDetailsAsync(int topCount);

        /// <summary>جستجو در حواله‌های در انتظار تأیید.</summary>
        Task<List<GoodsIssueDto>> SearchIssuesAsync(string keyword, int topCount = 50);

        /// <summary>جستجو در حواله‌های تأییدشده.</summary>
        Task<List<GoodsIssueDto>> SearchIssues(string keyword, int topCount = 50);

        Task<List<GoodsIssueDto>> SearchPendingIssuesAsync(string keyword, int topCount = 50);
    }
}
