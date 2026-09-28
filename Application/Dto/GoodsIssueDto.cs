using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dto
{
    public class GoodsIssueDto
    {
        public int IssueId { get; set; }
        public string ProductName { get; set; }
        public string WarehouseName { get; set; }
        public string CustomerName { get; set; }      
        public string UserName { get; set; } 
        public string ApprovedByUserName { get; set; }//کسی که حواله را تایید می کند
        public int Quantity { get; set; }
        public string Unit { get; set; }
        public decimal? UnitSellingPrice { get; set; }
        public decimal TotalPrice => (UnitSellingPrice ?? 0) * Quantity;
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime IssueDate { get; set; }
        public string StatusText { get; set; }          // وضعیت (ثبت، تایید، لغو)

        /// <summary>
        /// کد عددی وضعیت: 0 = در انتظار تأیید، 1 = تأیید شده، 2 = لغو شده.
        /// نمای صفحه برای تصمیم درباره نمایش دکمه «ویرایش» به این عدد نیاز دارد،
        /// نه به متن.
        /// </summary>
        public byte Status { get; set; }
        public string BatchNumber { get; set; }
        public string Description { get; set; }
        public string TypeText { get; internal set; }
        public DateTime? CreatedAt { get; internal set; }
    }
}
