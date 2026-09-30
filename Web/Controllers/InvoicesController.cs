using Application.Dto;
using Application.Features.Definition.GoodsIssue_Service;
using Application.Features.Definition.GoodsReceipt_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Infrastructure;
using Web.Models;

namespace Web.Controllers;

/// <summary>
/// فاکتورهای اسناد تأییدشده. فقط اسنادی نمایش داده می‌شوند که واقعاً موجودی را
/// جابه‌جا کرده‌اند (Status == Approved)، چون فاکتور باید مبنای مالی و
/// انبارداری باشد نه پیش‌نویس.
/// </summary>
[Authorize]
public class InvoicesController : Controller
{
    /// <summary>سقف تعداد ردیف در هر بارگذاری، تا صفحه با دادهٔ زیاد کند نشود.</summary>
    private const int MaxRows = 500;

    private readonly IGoodsReceiptService _receipts;
    private readonly IGoodsIssueService _issues;
    private readonly IClock _clock;

    public InvoicesController(IClock clock, IGoodsReceiptService receipts, IGoodsIssueService issues)
    {
        _clock = clock;
        _receipts = receipts;
        _issues = issues;
    }

    /// <summary>فاکتور ورودی: رسیدهای تأییدشده.</summary>
    public async Task<IActionResult> Incoming(string? keyword, int days = 30)
    {
        // متدهای Search* فقط اسناد در انتظار را برمی‌گردانند، پس برای فاکتور که
        // مبنای مالی است باید از متد مخصوص اسناد تأییدشده استفاده کرد.
        var found = await _receipts.GetTopApprovedReceiptsAsync(MaxRows);
        IEnumerable<GoodsReceiptDto> query = found;

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = keyword.Trim();
            query = query.Where(r =>
                (r.InvoiceNumber ?? "").Contains(k, StringComparison.OrdinalIgnoreCase) ||
                (r.ProductName ?? "").Contains(k, StringComparison.OrdinalIgnoreCase) ||
                (r.SupplierName ?? "").Contains(k, StringComparison.OrdinalIgnoreCase));
        }

        var approved = query.ToList();

        // days صفر یعنی «همهٔ بازه»؛ مقدار پیش‌فرض فقط ۳۰ روز اخیر را نشان می‌دهد.
        if (days > 0)
        {
            var from = _clock.Today.AddDays(-days);
            approved = approved.Where(r => r.ReceiptDate.Date >= from).ToList();
        }

        ViewData["Keyword"] = keyword;
        ViewData["Days"] = days;
        return View(approved
            .OrderByDescending(r => r.ReceiptDate)
            .ThenByDescending(r => r.ReceiptId)
            .ToList());
    }

    /// <summary>فاکتور خروجی: حواله‌های تأییدشده.</summary>
    public async Task<IActionResult> Outgoing(string? keyword, int days = 30)
    {
        var found = await _issues.GetApprovedIssuesDtoAsync(MaxRows);
        IEnumerable<GoodsIssueDto> query = found;

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var k = keyword.Trim();
            query = query.Where(i =>
                (i.InvoiceNumber ?? "").Contains(k, StringComparison.OrdinalIgnoreCase) ||
                (i.ProductName ?? "").Contains(k, StringComparison.OrdinalIgnoreCase) ||
                (i.CustomerName ?? "").Contains(k, StringComparison.OrdinalIgnoreCase));
        }

        var approved = query.ToList();

        if (days > 0)
        {
            var from = _clock.Today.AddDays(-days);
            approved = approved.Where(i => i.IssueDate.Date >= from).ToList();
        }

        ViewData["Keyword"] = keyword;
        ViewData["Days"] = days;
        return View(approved
            .OrderByDescending(i => i.IssueDate)
            .ThenByDescending(i => i.IssueId)
            .ToList());
    }

    /// <summary>
    /// چاپ یک فاکتور ورودی تکی. قالب مستقل از Layout است تا روی کاغذ تمیز چاپ شود.
    /// </summary>
    public async Task<IActionResult> PrintIncoming(int id)
    {
        var receipt = await _receipts.GetReceiptWithDetailsAsync(id);
        if (receipt is null || receipt.Status != 1) return NotFound();

        return View("Print", new InvoicePrintModel
        {
            Title = "فاکتور ورودی کالا",
            Number = receipt.InvoiceNumber,
            Date = receipt.ReceiptDate,
            Counterparty = receipt.Supplier?.Name,
            Warehouse = receipt.Warehouse?.Name,
            ProductName = receipt.Product?.Name,
            Quantity = receipt.Quantity,
            Unit = receipt.Unit,
            UnitPrice = receipt.UnitPrice,
            TaxRate = receipt.TaxRate,
            Total = receipt.NetPrice,
            ApprovedBy = receipt.ApprovedByFullName,
            Description = receipt.Description
        });
    }

    /// <summary>چاپ یک فاکتور خروجی تکی.</summary>
    public async Task<IActionResult> PrintOutgoing(int id)
    {
        var issue = await _issues.GetIssueWithDetailsAsync(id);
        if (issue is null || issue.Status != 1) return NotFound();

        return View("Print", new InvoicePrintModel
        {
            Title = "فاکتور خروجی کالا",
            Number = issue.InvoiceNumber,
            // IssueDate روی entity اختیاری است و اگر خالی باشد تاریخ ثبت استفاده می‌شود.
            Date = issue.IssueDate ?? issue.CreatedAt,
            // برای حواله‌های غیر فروش، مشتری خالی است و نوع سند جای آن را می‌گیرد.
            Counterparty = issue.Customer?.Name,
            Warehouse = issue.Warehouse?.Name,
            ProductName = issue.Product?.Name,
            Quantity = issue.Quantity,
            Unit = issue.Unit,
            UnitPrice = issue.UnitSellingPrice ?? 0,
            Total = (issue.UnitSellingPrice ?? 0) * issue.Quantity,
            ApprovedBy = issue.ApprovedByFullName,
            Description = issue.Description,
            Note = issue.Type.ToString()
        });
    }
}
