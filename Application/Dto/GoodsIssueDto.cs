using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto
{
    public class GoodsIssueDto
    {
        public int IssueId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;      
        public string UserName { get; set; } = string.Empty; 
        public string ApprovedByUserName { get; set; } = string.Empty;//Ú©Ø³ÛŒ Ú©Ù‡ Ø­ÙˆØ§Ù„Ù‡ Ø±Ø§ ØªØ§ÛŒÛŒØ¯ Ù…ÛŒ Ú©Ù†Ø¯
        public int Quantity { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal? UnitSellingPrice { get; set; }
        public decimal TotalPrice => (UnitSellingPrice ?? 0) * Quantity;
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public DateTime IssueDate { get; set; }
        public string StatusText { get; set; } = string.Empty;          // ÙˆØ¶Ø¹ÛŒØª (Ø«Ø¨ØªØŒ ØªØ§ÛŒÛŒØ¯ØŒ Ù„ØºÙˆ)

        /// <summary>
        /// Ú©Ø¯ Ø¹Ø¯Ø¯ÛŒ ÙˆØ¶Ø¹ÛŒØª: 0 = Ø¯Ø± Ø§Ù†ØªØ¸Ø§Ø± ØªØ£ÛŒÛŒØ¯ØŒ 1 = ØªØ£ÛŒÛŒØ¯ Ø´Ø¯Ù‡ØŒ 2 = Ù„ØºÙˆ Ø´Ø¯Ù‡.
        /// Ù†Ù…Ø§ÛŒ ØµÙØ­Ù‡ Ø¨Ø±Ø§ÛŒ ØªØµÙ…ÛŒÙ… Ø¯Ø±Ø¨Ø§Ø±Ù‡ Ù†Ù…Ø§ÛŒØ´ Ø¯Ú©Ù…Ù‡ Â«ÙˆÛŒØ±Ø§ÛŒØ´Â» Ø¨Ù‡ Ø§ÛŒÙ† Ø¹Ø¯Ø¯ Ù†ÛŒØ§Ø² Ø¯Ø§Ø±Ø¯ØŒ
        /// Ù†Ù‡ Ø¨Ù‡ Ù…ØªÙ†.
        /// </summary>
        public byte Status { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TypeText { get; internal set; } = string.Empty;
        public DateTime? CreatedAt { get; internal set; }
    }
}
