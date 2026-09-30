using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Dto
{
    /// <summary>
    /// Ù…Ø¯Ù„ Ù†Ù…Ø§ÛŒØ´ÛŒ Ø¨Ø±Ø§ÛŒ Ø±Ø³ÛŒØ¯ Ø§Ù†Ø¨Ø§Ø± (GoodsReceipt) - Ø¬Ù‡Øª Ø§Ø³ØªÙØ§Ø¯Ù‡ Ø¯Ø± DataGridView Ùˆ Ú¯Ø²Ø§Ø±Ø´Ø§Øª
    /// </summary>
    public class GoodsReceiptDto
    {
        /// <summary>
        /// Ø´Ù†Ø§Ø³Ù‡ ÛŒÚ©ØªØ§ÛŒ Ø±Ø³ÛŒØ¯ (Ø¨Ø±Ø§ÛŒ Ø§Ø³ØªÙØ§Ø¯Ù‡ Ø¯Ø± Ù¾Ø´Øª ØµØ­Ù†Ù‡ØŒ Ù…Ø¹Ù…ÙˆÙ„Ø§Ù‹ Ù…Ø®ÙÛŒ Ù…ÛŒâ€ŒØ´ÙˆØ¯)
        /// </summary>
        public int ReceiptId { get; set; }

        /// <summary>
        /// Ù†Ø§Ù… Ù…Ø­ØµÙˆÙ„ (Ø¬Ø§ÛŒÚ¯Ø²ÛŒÙ† ProductId)
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Ù†Ø§Ù… ØªØ£Ù…ÛŒÙ†â€ŒÚ©Ù†Ù†Ø¯Ù‡ (Ø¬Ø§ÛŒÚ¯Ø²ÛŒÙ† SupplierId)
        /// </summary>
        public string SupplierName { get; set; } = string.Empty;

        /// <summary>
        /// Ù†Ø§Ù… Ø§Ù†Ø¨Ø§Ø± Ù…Ù‚ØµØ¯ (Ø¬Ø§ÛŒÚ¯Ø²ÛŒÙ† WarehouseId)
        /// </summary>
        public string WarehouseName { get; set; } = string.Empty;

        /// <summary>
        /// Ù†Ø§Ù… Ú©Ø§Ø±Ø¨Ø± Ø«Ø¨Øªâ€ŒÚ©Ù†Ù†Ø¯Ù‡ (Ø¬Ø§ÛŒÚ¯Ø²ÛŒÙ† UserId)
        /// </summary>
        public string UserFullName { get; set; } = string.Empty;
        /// <summary>
        /// Ø´Ù†Ø§Ø³Ù‡ Ú©Ø§Ø±Ø¨Ø± ØªØ£ÛŒÛŒØ¯Ú©Ù†Ù†Ø¯Ù‡ Ø±Ø³ÛŒØ¯
        /// </summary>
        public string? ApprovedByUserId { get; set; }

        /// <summary>
        /// Ù†Ø§Ù… Ú©Ø§Ù…Ù„ Ú©Ø§Ø±Ø¨Ø± ØªØ£ÛŒÛŒØ¯Ú©Ù†Ù†Ø¯Ù‡
        /// </summary>
        [MaxLength(100)]
        public string? ApprovedByFullName { get; set; }

        /// <summary>
        /// Ø¨Ø§Ø±Ú©Ø¯ Ø§Ø³Ú©Ù† Ø´Ø¯Ù‡ (Ø¯Ø± ØµÙˆØ±Øª ÙˆØ¬ÙˆØ¯)
        /// </summary>
        public string ScannedBarcode { get; set; } = string.Empty;

        /// <summary>
        /// ØªØ¹Ø¯Ø§Ø¯ ÙˆØ±ÙˆØ¯ÛŒ
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// ÙˆØ§Ø­Ø¯ Ø§Ù†Ø¯Ø§Ø²Ù‡â€ŒÚ¯ÛŒØ±ÛŒ (Ø¹Ø¯Ø¯ØŒ Ú©ÛŒÙ„ÙˆÚ¯Ø±Ù…ØŒ Ù…ØªØ± Ùˆ ...)
        /// </summary>
        public string Unit { get; set; } = string.Empty;

        /// <summary>
        /// Ù‚ÛŒÙ…Øª ÙˆØ§Ø­Ø¯ (Ø¨Ø¯ÙˆÙ† Ù…Ø§Ù„ÛŒØ§Øª)
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Ù‚ÛŒÙ…Øª Ú©Ù„ (Ù…Ø­Ø§Ø³Ø¨Ù‡ Ø´Ø¯Ù‡ = ØªØ¹Ø¯Ø§Ø¯ * Ù‚ÛŒÙ…Øª ÙˆØ§Ø­Ø¯)
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Ù†Ø±Ø® Ù…Ø§Ù„ÛŒØ§Øª (Ø¯Ø±ØµØ¯)
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// Ù‚ÛŒÙ…Øª Ù†Ù‡Ø§ÛŒÛŒ Ø¨Ø§ Ø§Ø­ØªØ³Ø§Ø¨ Ù…Ø§Ù„ÛŒØ§Øª
        /// </summary>
        public decimal NetPrice { get; set; }

        /// <summary>
        /// Ø´Ù…Ø§Ø±Ù‡ ÙØ§Ú©ØªÙˆØ± Ø®Ø±ÛŒØ¯
        /// </summary>
        public string InvoiceNumber { get; set; } = string.Empty;

        /// <summary>
        /// ØªØ§Ø±ÛŒØ® ÙØ§Ú©ØªÙˆØ±
        /// </summary>
        public DateTime InvoiceDate { get; set; }

        /// <summary>
        /// Ù…ÙˆÙ‚Ø¹ÛŒØª Ù‚ÙØ³Ù‡ Ø¯Ø± Ø§Ù†Ø¨Ø§Ø±
        /// </summary>
        public string ShelfLocation { get; set; } = string.Empty;

        /// <summary>
        /// ØªØ§Ø±ÛŒØ® ÙˆØ±ÙˆØ¯ Ú©Ø§Ù„Ø§ Ø¨Ù‡ Ø§Ù†Ø¨Ø§Ø± (ReceiptDate)
        /// </summary>
        public DateTime ReceiptDate { get; set; }

        /// <summary>
        /// ØªØ§Ø±ÛŒØ® Ø«Ø¨Øª Ø±Ú©ÙˆØ±Ø¯ Ø¯Ø± Ø³ÛŒØ³ØªÙ… (CreatedAt)
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// ÙˆØ¶Ø¹ÛŒØª Ø±Ø³ÛŒØ¯ (Ù…ØªÙ† ÙØ§Ø±Ø³ÛŒ: Ø¯Ø± Ø§Ù†ØªØ¸Ø§Ø± ØªØ£ÛŒÛŒØ¯ØŒ ØªØ£ÛŒÛŒØ¯ Ø´Ø¯Ù‡ØŒ Ù„ØºÙˆ Ø´Ø¯Ù‡)
        /// </summary>
        public string StatusText { get; set; } = string.Empty;

        /// <summary>
        /// Ú©Ø¯ Ø¹Ø¯Ø¯ÛŒ ÙˆØ¶Ø¹ÛŒØª: 0 = Ø¯Ø± Ø§Ù†ØªØ¸Ø§Ø± ØªØ£ÛŒÛŒØ¯ØŒ 1 = ØªØ£ÛŒÛŒØ¯ Ø´Ø¯Ù‡ØŒ 2 = Ù„ØºÙˆ Ø´Ø¯Ù‡.
        /// Ø¨Ø¯ÙˆÙ† Ø§ÛŒÙ† ÙÛŒÙ„Ø¯ØŒ Ù†Ù…Ø§ÛŒ ØµÙØ­Ù‡ Ù†Ø§Ú†Ø§Ø± Ø¨ÙˆØ¯ ÙˆØ¶Ø¹ÛŒØª Ø±Ø§ Ø«Ø§Ø¨Øª 0 ÙØ±Ø¶ Ú©Ù†Ø¯ Ùˆ
        /// Ø±Ø³ÛŒØ¯ ØªØ£ÛŒÛŒØ¯Ø´Ø¯Ù‡ Ù‡Ù… Â«Ø¯Ø± Ø§Ù†ØªØ¸Ø§Ø± ØªØ£ÛŒÛŒØ¯Â» Ù†Ø´Ø§Ù† Ø¯Ø§Ø¯Ù‡ Ù…ÛŒâ€ŒØ´Ø¯.
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// ØªØ§Ø±ÛŒØ® Ø§Ù†Ù‚Ø¶Ø§ÛŒ Ú©Ø§Ù„Ø§ (Ø¯Ø± ØµÙˆØ±Øª ÙˆØ¬ÙˆØ¯)
        /// </summary>
        public DateTime? ExpiryDate { get; set; }

        /// <summary>
        /// Ø´Ù…Ø§Ø±Ù‡ Ø¯Ø³ØªÙ‡ (Batch) ÛŒØ§ Ù„Ø§Øª
        /// </summary>
        public string BatchNumber { get; set; } = string.Empty;

        /// <summary>
        /// ØªÙˆØ¶ÛŒØ­Ø§Øª Ø§Ø¶Ø§ÙÛŒ
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }
}
