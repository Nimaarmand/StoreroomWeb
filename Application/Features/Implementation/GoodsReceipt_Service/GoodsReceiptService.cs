using Application.Features.Definition.Context;
using Application.Dto;
using Application.Features.Definition.UserManagement_Service;
using Application.Features.Definition.GenericRepository;
using Application.Features.Definition.GoodsReceipt_Service;
using Application.Features.Implementation.GenericRepository_Service;
using Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Application.Features.Implementation.GoodsReceipt_Service
{
    public class GoodsReceiptService : GenericRepository<GoodsReceipt>, IGoodsReceiptService
    {
        private readonly IUserManagement _usermanagementService;
        private readonly UserManager<ApplicationUser> _userManager;

        public GoodsReceiptService(IApplicationDbContext context,
        IUserManagement usermanagementService,
            UserManager<ApplicationUser> userManager
            )
            : base(context)
        {
            _usermanagementService = usermanagementService;
            _userManager = userManager;
        }

        // ========== Ã™â€¦Ã˜ÂªÃ˜Â¯Ã™â€¡Ã˜Â§Ã›Å’ Ã˜Â§Ã˜Â®Ã˜ÂªÃ˜ÂµÃ˜Â§Ã˜ÂµÃ›Å’ (Ã™â€¡Ã™â€¦ÃšÂ¯Ã›Å’ Ã˜Â¨Ã˜Â§ Ã™â€šÃ™ÂÃ™â€ž) ==========
        
        
        /// <summary>
        /// Ã˜Â¯Ã˜Â±Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª Ã›Å’ÃšÂ© Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯ Ã˜Â¨Ã˜Â± Ã˜Â§Ã˜Â³Ã˜Â§Ã˜Â³ Ã˜Â´Ã™â€¦Ã˜Â§Ã˜Â±Ã™â€¡ Ã™ÂÃ˜Â§ÃšÂ©Ã˜ÂªÃ™Ë†Ã˜Â± (Ã˜Â­Ã˜Â³Ã˜Â§Ã˜Â³ Ã˜Â¨Ã™â€¡ Ã˜Â­Ã˜Â±Ã™Ë†Ã™Â Ã˜Â¨Ã˜Â²Ã˜Â±ÃšÂ¯ Ã™Ë† ÃšÂ©Ã™Ë†Ãšâ€ ÃšÂ© Ã™â€ Ã›Å’Ã˜Â³Ã˜Âª)
        /// </summary>
        /// <param name="invoiceNumber">Ã˜Â´Ã™â€¦Ã˜Â§Ã˜Â±Ã™â€¡ Ã™ÂÃ˜Â§ÃšÂ©Ã˜ÂªÃ™Ë†Ã˜Â±</param>
        /// <returns>Ã˜Â´Ã›Å’Ã˜Â¡ GoodsReceipt Ã˜Â¯Ã˜Â± Ã˜ÂµÃ™Ë†Ã˜Â±Ã˜Âª Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã˜Å’ Ã˜Â¯Ã˜Â± Ã˜ÂºÃ›Å’Ã˜Â± Ã˜Â§Ã›Å’Ã™â€  Ã˜ÂµÃ™Ë†Ã˜Â±Ã˜Âª null</returns>
        public async Task<GoodsReceipt?> GetByInvoiceNumberAsync(string invoiceNumber)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber)) return null;
            return await _dbSet.FirstOrDefaultAsync(r => r.InvoiceNumber == invoiceNumber);
        }

        /// <summary>
        /// Ã˜Â¯Ã˜Â±Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª Ã˜ÂªÃ™â€¦Ã˜Â§Ã™â€¦ Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯Ã™â€¡Ã˜Â§Ã›Å’ Ã™â€¦Ã˜Â±Ã˜Â¨Ã™Ë†Ã˜Â· Ã˜Â¨Ã™â€¡ Ã›Å’ÃšÂ© Ã™â€¦Ã˜Â­Ã˜ÂµÃ™Ë†Ã™â€ž Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ
        /// </summary>
        /// <param name="productId">Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã™â€¦Ã˜Â­Ã˜ÂµÃ™Ë†Ã™â€ž (Guid)</param>
        /// <returns>Ã™â€žÃ›Å’Ã˜Â³Ã˜ÂªÃ›Å’ Ã˜Â§Ã˜Â² Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯Ã™â€¡Ã˜Â§ Ã˜Â¨Ã™â€¡ Ã™â€¡Ã™â€¦Ã˜Â±Ã˜Â§Ã™â€¡ Ã˜Â§Ã˜Â·Ã™â€žÃ˜Â§Ã˜Â¹Ã˜Â§Ã˜Âª Ã™â€¦Ã˜Â­Ã˜ÂµÃ™Ë†Ã™â€ž Ã™Ë† Ã˜ÂªÃ˜Â£Ã™â€¦Ã›Å’Ã™â€ Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã™â€ Ã˜Â¯Ã™â€¡</returns>
        public async Task<IEnumerable<GoodsReceipt>> GetByProductIdAsync(Guid productId)
        {
            return await _dbSet
                .Where(r => r.ProductId == productId)
                .Include(r => r.Product)
                .Include(r => r.Supplier)
                .ToListAsync();
        }

        /// <summary>
        /// Ã˜Â¯Ã˜Â±Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª Ã˜ÂªÃ™â€¦Ã˜Â§Ã™â€¦ Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯Ã™â€¡Ã˜Â§Ã›Å’ Ã™â€¦Ã˜Â±Ã˜Â¨Ã™Ë†Ã˜Â· Ã˜Â¨Ã™â€¡ Ã›Å’ÃšÂ© Ã˜ÂªÃ˜Â£Ã™â€¦Ã›Å’Ã™â€ Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â®Ã˜Â§Ã˜Âµ
        /// </summary>
        /// <param name="supplierId">Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã˜ÂªÃ˜Â£Ã™â€¦Ã›Å’Ã™â€ Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã™â€ Ã˜Â¯Ã™â€¡</param>
        /// <returns>Ã™â€žÃ›Å’Ã˜Â³Ã˜Âª Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯Ã™â€¡Ã˜Â§ Ã˜Â¨Ã™â€¡ Ã™â€¡Ã™â€¦Ã˜Â±Ã˜Â§Ã™â€¡ Ã˜Â§Ã˜Â·Ã™â€žÃ˜Â§Ã˜Â¹Ã˜Â§Ã˜Âª Ã˜ÂªÃ˜Â£Ã™â€¦Ã›Å’Ã™â€ Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã™â€ Ã˜Â¯Ã™â€¡</returns>
        public async Task<IEnumerable<GoodsReceipt>> GetBySupplierIdAsync(int supplierId)
        {
            return await _dbSet
                .Where(r => r.SupplierId == supplierId)
                .Include(r => r.Supplier)
                .ToListAsync();
        }

        /// <summary>
        /// Ã˜Â¯Ã˜Â±Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª Ã˜ÂªÃ™â€¦Ã˜Â§Ã™â€¦ Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯Ã™â€¡Ã˜Â§Ã›Å’ Ã™â€¦Ã˜Â±Ã˜Â¨Ã™Ë†Ã˜Â· Ã˜Â¨Ã™â€¡ Ã›Å’ÃšÂ© Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â± Ã˜Â®Ã˜Â§Ã˜Âµ
        /// </summary>
        /// <param name="warehouseId">Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â±</param>
        /// <returns>Ã™â€žÃ›Å’Ã˜Â³Ã˜Âª Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯Ã™â€¡Ã˜Â§ Ã˜Â¨Ã™â€¡ Ã™â€¡Ã™â€¦Ã˜Â±Ã˜Â§Ã™â€¡ Ã˜Â§Ã˜Â·Ã™â€žÃ˜Â§Ã˜Â¹Ã˜Â§Ã˜Âª Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â±</returns>
        public async Task<IEnumerable<GoodsReceipt>> GetByWarehouseIdAsync(int warehouseId)
        {
            return await _dbSet
                .Where(r => r.WarehouseId == warehouseId)
                .Include(r => r.Warehouse)
                .ToListAsync();
        }

        /// <summary>
        /// Ã˜Â¯Ã˜Â±Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯Ã™â€¡Ã˜Â§Ã›Å’ Ã˜Â«Ã˜Â¨Ã˜ÂªÃ¢â‚¬Å’Ã˜Â´Ã˜Â¯Ã™â€¡ Ã˜Â¯Ã˜Â± Ã›Å’ÃšÂ© Ã˜Â¨Ã˜Â§Ã˜Â²Ã™â€¡ Ã˜Â²Ã™â€¦Ã˜Â§Ã™â€ Ã›Å’ Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ
        /// </summary>
        /// <param name="startDate">Ã˜ÂªÃ˜Â§Ã˜Â±Ã›Å’Ã˜Â® Ã˜Â´Ã˜Â±Ã™Ë†Ã˜Â¹ (Ã˜Â´Ã™â€¦Ã˜Â³Ã›Å’ Ã¢â‚¬â€œ Ã™â€¦Ã›Å’Ã™â€žÃ˜Â§Ã˜Â¯Ã›Å’ Ã˜Â¯Ã˜Â± Ã˜Â¯Ã›Å’Ã˜ÂªÃ˜Â§Ã˜Â¨Ã›Å’Ã˜Â³)</param>
        /// <param name="endDate">Ã˜ÂªÃ˜Â§Ã˜Â±Ã›Å’Ã˜Â® Ã™Â¾Ã˜Â§Ã›Å’Ã˜Â§Ã™â€ </param>
        /// <returns>Ã™â€žÃ›Å’Ã˜Â³Ã˜Âª Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯Ã™â€¡Ã˜Â§ Ã™â€¦Ã˜Â±Ã˜ÂªÃ˜Â¨ Ã˜Â´Ã˜Â¯Ã™â€¡ Ã˜Â¨Ã˜Â± Ã˜Â§Ã˜Â³Ã˜Â§Ã˜Â³ Ã˜ÂªÃ˜Â§Ã˜Â±Ã›Å’Ã˜Â® Ã˜Â¯Ã˜Â±Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª (Ã™â€šÃ˜Â¯Ã›Å’Ã™â€¦Ã¢â‚¬Å’Ã˜ÂªÃ˜Â±Ã›Å’Ã™â€  Ã˜Â§Ã™Ë†Ã™â€ž)</returns>
        public async Task<IEnumerable<GoodsReceipt>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(r => r.ReceiptDate >= startDate && r.ReceiptDate <= endDate)
                .Include(r => r.Product)
                .Include(r => r.Supplier)
                .OrderBy(r => r.ReceiptDate)
                .ToListAsync();
        }
        /// <summary>
        /// Ã˜Â¯Ã˜Â±Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª Ã˜ÂªÃ˜Â¹Ã˜Â¯Ã˜Â§Ã˜Â¯ Ã™â€¦Ã˜Â´Ã˜Â®Ã˜ÂµÃ›Å’ Ã˜Â§Ã˜Â² Ã˜Â¢Ã˜Â®Ã˜Â±Ã›Å’Ã™â€  Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯Ã™â€¡Ã˜Â§Ã›Å’ **Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â´Ã˜Â¯Ã™â€¡** (Ã™Ë†Ã˜Â¶Ã˜Â¹Ã›Å’Ã˜Âª 1) Ã˜Â¨Ã™â€¡ Ã™â€¡Ã™â€¦Ã˜Â±Ã˜Â§Ã™â€¡ Ã˜Â¬Ã˜Â²Ã˜Â¦Ã›Å’Ã˜Â§Ã˜Âª (DTO)
        /// </summary>
        public async Task<List<GoodsReceiptDto>> GetTopApprovedReceiptsAsync(int topCount)
        {
            if (topCount <= 0) topCount = 20;

            var receipts = await _dbSet
                .Include(r => r.Product)
                .Include(r => r.Supplier)
                .Include(r => r.Warehouse)
                .Where(r => r.Status == 1)
                .OrderByDescending(r => r.ReceiptDate)
                .Take(topCount)
                .AsNoTracking()
                .ToListAsync();
            if (receipts == null || !receipts.Any())
                return new List<GoodsReceiptDto>();
            var userIds = receipts.Where(r => !string.IsNullOrEmpty(r.UserId)).Select(r => r.UserId).Distinct().ToList();
            Dictionary<string, string> userNames = new Dictionary<string, string>();
            if (userIds.Any())
            {
                var users = await _userManager.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => new { u.Id, u.FullName })
                    .ToListAsync();
                userNames = users.ToDictionary(u => u.Id, u => u.FullName);
            }
            var result = new List<GoodsReceiptDto>();
            foreach (var receipt in receipts)
            {
                string userName = (!string.IsNullOrEmpty(receipt.UserId) && userNames.ContainsKey(receipt.UserId))
                    ? userNames[receipt.UserId]
                    : "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ";
                result.Add(new GoodsReceiptDto
                {
                    ReceiptId = receipt.ReceiptId,
                    ProductName = receipt.Product?.Name ?? "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ",
                    SupplierName = receipt.Supplier?.Name ?? "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ",
                    WarehouseName = receipt.Warehouse?.Name ?? "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ",
                    UserFullName = userName,
                    Quantity = receipt.Quantity,
                    Unit = receipt.Unit,
                    UnitPrice = receipt.UnitPrice,
                    TotalPrice = receipt.Quantity * receipt.UnitPrice,
                    InvoiceNumber = receipt.InvoiceNumber,
                    InvoiceDate = receipt.InvoiceDate,
                    ReceiptDate = receipt.ReceiptDate,
                    StatusText = "Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â´Ã˜Â¯Ã™â€¡",
                    Status = receipt.Status,
                    BatchNumber = receipt.BatchNumber ?? string.Empty,
                    Description = receipt.Description ?? string.Empty,
                    ApprovedByFullName = receipt.ApprovedByFullName ?? "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ"   // Ã¢Å“â€¦ Ã˜Â§Ã˜Â¶Ã˜Â§Ã™ÂÃ™â€¡ Ã˜Â´Ã˜Â¯
                });
            }
            return result;
        }

        /// <summary>
        /// Ã˜Â¯Ã˜Â±Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª Ã›Å’ÃšÂ© Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯ Ã˜Â¨Ã™â€¡ Ã™â€¡Ã™â€¦Ã˜Â±Ã˜Â§Ã™â€¡ Ã˜ÂªÃ™â€¦Ã˜Â§Ã™â€¦ Ã˜Â¬Ã˜Â²Ã˜Â¦Ã›Å’Ã˜Â§Ã˜Âª Ã™â€¦Ã˜Â±Ã˜ÂªÃ˜Â¨Ã˜Â· (Ã™â€¦Ã˜Â­Ã˜ÂµÃ™Ë†Ã™â€žÃ˜Å’ Ã˜ÂªÃ˜Â£Ã™â€¦Ã›Å’Ã™â€ Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã™â€ Ã˜Â¯Ã™â€¡Ã˜Å’ Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â±)
        /// </summary>
        /// <param name="receiptId">Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯</param>
        /// <returns>Ã˜Â´Ã›Å’Ã˜Â¡ GoodsReceipt Ã˜Â¨Ã˜Â§ Ã˜Â§Ã˜Â·Ã™â€žÃ˜Â§Ã˜Â¹Ã˜Â§Ã˜Âª Ã™Ë†Ã˜Â§Ã˜Â¨Ã˜Â³Ã˜ÂªÃ™â€¡</returns>
        public async Task<GoodsReceipt?> GetReceiptWithDetailsAsync(int receiptId)
        {
            return await _dbSet
                .Include(r => r.Product)
                .Include(r => r.Supplier)
                .Include(r => r.Warehouse)
                .FirstOrDefaultAsync(r => r.ReceiptId == receiptId);
        }

        /// <summary>
        /// Ã™â€¦Ã˜Â­Ã˜Â§Ã˜Â³Ã˜Â¨Ã™â€¡ Ã™â€šÃ›Å’Ã™â€¦Ã˜Âª Ã˜Â®Ã˜Â§Ã™â€žÃ˜Âµ ÃšÂ©Ã™â€ž (Ã˜Â¨Ã˜Â§ Ã˜Â§Ã˜Â­Ã˜ÂªÃ˜Â³Ã˜Â§Ã˜Â¨ Ã™â€¦Ã˜Â§Ã™â€žÃ›Å’Ã˜Â§Ã˜Âª) Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã›Å’ÃšÂ© Ã™â€¦Ã˜Â­Ã˜ÂµÃ™Ë†Ã™â€ž Ã˜Â®Ã˜Â§Ã˜Âµ Ã˜Â¨Ã˜Â± Ã˜Â§Ã˜Â³Ã˜Â§Ã˜Â³ Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯Ã™â€¡Ã˜Â§Ã›Å’ Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â´Ã˜Â¯Ã™â€¡
        /// </summary>
        /// <param name="productId">Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã™â€¦Ã˜Â­Ã˜ÂµÃ™Ë†Ã™â€ž</param>
        /// <returns>Ã˜Â¬Ã™â€¦Ã˜Â¹ Ã™â€šÃ›Å’Ã™â€¦Ã˜Âª Ã˜Â®Ã˜Â§Ã™â€žÃ˜Âµ (decimal)</returns>
        public async Task<decimal> GetTotalNetPriceByProductAsync(Guid productId)
        {
            var receipts = await _dbSet
                .Where(r => r.ProductId == productId && r.Status == 1)
                .ToListAsync();
            return receipts.Sum(r => r.NetPrice);
        }

        /// <summary>
        /// Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯ Ã™Ë†Ã˜Â±Ã™Ë†Ã˜Â¯Ã›Å’ Ã™Ë† Ã˜Â§Ã™ÂÃ˜Â²Ã˜Â§Ã›Å’Ã˜Â´ Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã™â€¦Ã˜Â­Ã˜ÂµÃ™Ë†Ã™â€ž Ã™Ë† Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â±.
        ///
        /// Ã™â€ ÃšÂ©Ã˜ÂªÃ™â€¡ Ã™â€¦Ã™â€¡Ã™â€¦ Ã˜Â¯Ã˜Â±Ã˜Â¨Ã˜Â§Ã˜Â±Ã™â€¡ Ã™â€¡Ã™â€¦Ã˜Â²Ã™â€¦Ã˜Â§Ã™â€ Ã›Å’: Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â¨Ã˜Â§ Ã‚Â«Ã˜Â®Ã™Ë†Ã˜Â§Ã™â€ Ã˜Â¯Ã™â€ Ã˜Å’ Ã˜Â¬Ã™â€¦Ã˜Â¹ ÃšÂ©Ã˜Â±Ã˜Â¯Ã™â€ Ã˜Å’ Ã™â€ Ã™Ë†Ã˜Â´Ã˜ÂªÃ™â€ Ã‚Â» Ã˜Â¨Ã™â€¡Ã¢â‚¬Å’Ã˜Â±Ã™Ë†Ã˜Â²Ã˜Â±Ã˜Â³Ã˜Â§Ã™â€ Ã›Å’ Ã™â€ Ã™â€¦Ã›Å’Ã¢â‚¬Å’Ã˜Â´Ã™Ë†Ã˜Â¯.
        /// Ã™â€¡Ã˜Â± Ã˜Â³Ã™â€¡ Ã˜ÂªÃ˜ÂºÃ›Å’Ã›Å’Ã˜Â± Ã˜Â¨Ã˜Â§ ExecuteUpdateAsync Ã˜Â§Ã™â€ Ã˜Â¬Ã˜Â§Ã™â€¦ Ã™â€¦Ã›Å’Ã¢â‚¬Å’Ã˜Â´Ã™Ë†Ã˜Â¯ ÃšÂ©Ã™â€¡ Ã˜Â¨Ã™â€¡ Ã›Å’ÃšÂ© Ã˜Â¯Ã˜Â³Ã˜ÂªÃ™Ë†Ã˜Â± UPDATE Ã˜Â§Ã˜ÂªÃ™â€¦Ã›Å’ÃšÂ©
        /// Ã˜Â¯Ã˜Â± Ã˜Â¯Ã›Å’Ã˜ÂªÃ˜Â§Ã˜Â¨Ã›Å’Ã˜Â³ Ã˜ÂªÃ˜Â¨Ã˜Â¯Ã›Å’Ã™â€ž Ã™â€¦Ã›Å’Ã¢â‚¬Å’Ã˜Â´Ã™Ë†Ã˜Â¯. Ã˜Â¨Ã™â€ Ã˜Â§Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’Ã™â€  Ã˜Â§ÃšÂ¯Ã˜Â± Ã˜Â¯Ã™Ë† ÃšÂ©Ã˜Â§Ã˜Â±Ã˜Â¨Ã˜Â± Ã™â€¡Ã™â€¦Ã˜Â²Ã™â€¦Ã˜Â§Ã™â€  Ã˜Â¯Ã™Ë† Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯ Ã˜Â±Ã˜Â§ Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯ ÃšÂ©Ã™â€ Ã™â€ Ã˜Â¯Ã˜Å’
        /// Ã™â€¡Ã˜Â± Ã˜Â¯Ã™Ë† Ã˜Â§Ã™ÂÃ˜Â²Ã˜Â§Ã›Å’Ã˜Â´ Ã˜Â¯Ã™â€šÃ›Å’Ã™â€šÃ˜Â§Ã™â€¹ Ã›Å’ÃšÂ© Ã˜Â¨Ã˜Â§Ã˜Â± Ã˜Â§Ã˜Â¹Ã™â€¦Ã˜Â§Ã™â€ž Ã™â€¦Ã›Å’Ã¢â‚¬Å’Ã˜Â´Ã™Ë†Ã˜Â¯ Ã™Ë† Ã™â€¡Ã›Å’Ãšâ€  Ã˜ÂªÃ˜ÂºÃ›Å’Ã›Å’Ã˜Â±Ã›Å’ ÃšÂ¯Ã™â€¦ Ã™â€ Ã™â€¦Ã›Å’Ã¢â‚¬Å’Ã˜Â´Ã™Ë†Ã˜Â¯.
        /// Ã˜Â§Ã›Å’Ã™â€  Ã˜ÂªÃ˜Â¶Ã™â€¦Ã›Å’Ã™â€  Ã˜Â¨Ã›Å’Ã™â€  Ãšâ€ Ã™â€ Ã˜Â¯ Ã˜Â³Ã˜Â±Ã™Ë†Ã˜Â± Ã™Ë† Ã˜Â¨Ã›Å’Ã™â€  Ã™â€ Ã˜Â³Ã˜Â®Ã™â€¡ Ã™Ë†Ã˜Â¨ Ã™Ë† Ã˜Â¯Ã˜Â³ÃšÂ©Ã˜ÂªÃ˜Â§Ã™Â¾ Ã™â€¡Ã™â€¦ Ã˜Â¨Ã˜Â±Ã™â€šÃ˜Â±Ã˜Â§Ã˜Â± Ã˜Â§Ã˜Â³Ã˜Âª.
        /// </summary>
        public async Task<(bool Success, string Message)> ApproveReceiptAsync(int receiptId, string currentUserId)
        {
            // 1. Ã˜Â§Ã˜Â¹Ã˜ÂªÃ˜Â¨Ã˜Â§Ã˜Â±Ã˜Â³Ã™â€ Ã˜Â¬Ã›Å’ Ã™Ë†Ã˜Â±Ã™Ë†Ã˜Â¯Ã›Å’
            if (string.IsNullOrEmpty(currentUserId))
                return (false, "ÃšÂ©Ã˜Â§Ã˜Â±Ã˜Â¨Ã˜Â± Ã˜Â¬Ã˜Â§Ã˜Â±Ã›Å’ Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã˜Â§Ã›Å’Ã›Å’ Ã™â€ Ã˜Â´Ã˜Â¯. Ã™â€žÃ˜Â·Ã™ÂÃ˜Â§Ã™â€¹ Ã™Ë†Ã˜Â§Ã˜Â±Ã˜Â¯ Ã˜Â´Ã™Ë†Ã›Å’Ã˜Â¯.");

            // 2. Ã˜Â®Ã™Ë†Ã˜Â§Ã™â€ Ã˜Â¯Ã™â€  Ã˜Â§Ã˜Â·Ã™â€žÃ˜Â§Ã˜Â¹Ã˜Â§Ã˜Âª Ã™â€žÃ˜Â§Ã˜Â²Ã™â€¦ (Ã˜Â¨Ã˜Â¯Ã™Ë†Ã™â€  Ã™â€šÃ™ÂÃ™â€ž Ã™Ë† Ã™ÂÃ™â€šÃ˜Â· Ã˜Â¨Ã˜Â±Ã˜Â§Ã›Å’ Ã˜Â§Ã˜Â¹Ã˜ÂªÃ˜Â¨Ã˜Â§Ã˜Â±Ã˜Â³Ã™â€ Ã˜Â¬Ã›Å’ Ã˜Â§Ã™Ë†Ã™â€žÃ›Å’Ã™â€¡)
            var receipt = await _dbSet.FirstOrDefaultAsync(r => r.ReceiptId == receiptId);

            if (receipt == null)
                return (false, "Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯ Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª Ã™â€ Ã˜Â´Ã˜Â¯.");

            if (receipt.Status == 1)
                return (false, "Ã˜Â§Ã›Å’Ã™â€  Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯ Ã™â€šÃ˜Â¨Ã™â€žÃ˜Â§Ã™â€¹ Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â´Ã˜Â¯Ã™â€¡ Ã˜Â§Ã˜Â³Ã˜Âª.");

            if (!receipt.WarehouseId.HasValue)
                return (false, "Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â± Ã˜Â§Ã›Å’Ã™â€  Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯ Ã˜Â§Ã™â€ Ã˜ÂªÃ˜Â®Ã˜Â§Ã˜Â¨ Ã™â€ Ã˜Â´Ã˜Â¯Ã™â€¡ Ã˜Â§Ã˜Â³Ã˜Âª.");

            // 3. Ã˜Â¯Ã˜Â±Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª ÃšÂ©Ã˜Â§Ã˜Â±Ã˜Â¨Ã˜Â± Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯ÃšÂ©Ã™â€ Ã™â€ Ã˜Â¯Ã™â€¡
            var user = await _userManager.FindByIdAsync(currentUserId)
                       ?? await _usermanagementService.FindUserByIdAsync(currentUserId);

            if (user == null)
                return (false, "ÃšÂ©Ã˜Â§Ã˜Â±Ã˜Â¨Ã˜Â± Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯ÃšÂ©Ã™â€ Ã™â€ Ã˜Â¯Ã™â€¡ Ã˜Â¯Ã˜Â± Ã˜Â¯Ã›Å’Ã˜ÂªÃ˜Â§Ã˜Â¨Ã›Å’Ã˜Â³ Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª Ã™â€ Ã˜Â´Ã˜Â¯.");

            var quantity = receipt.Quantity;
            var productId = receipt.ProductId;
            var warehouseId = receipt.WarehouseId.Value;
            var approverName = user.FullName ?? user.UserName ?? "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ";

            // Ã˜Â¯Ã˜Â± Ã˜ÂµÃ™Ë†Ã˜Â±Ã˜Âª Ã˜Â®Ã˜Â·Ã˜Â§Ã›Å’ Ã™â€¦Ã™Ë†Ã™â€šÃ˜Âª Ã˜Â¯Ã›Å’Ã˜ÂªÃ˜Â§Ã˜Â¨Ã›Å’Ã˜Â³Ã˜Å’ ÃšÂ©Ã™â€ž Ã˜Â¹Ã™â€¦Ã™â€žÃ›Å’Ã˜Â§Ã˜Âª Ã˜Â¨Ã™â€¡Ã¢â‚¬Å’Ã˜ÂµÃ™Ë†Ã˜Â±Ã˜Âª Ã˜Â®Ã™Ë†Ã˜Â¯ÃšÂ©Ã˜Â§Ã˜Â± Ã˜ÂªÃšÂ©Ã˜Â±Ã˜Â§Ã˜Â± Ã™â€¦Ã›Å’Ã¢â‚¬Å’Ã˜Â´Ã™Ë†Ã˜Â¯.
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                // 4. Ã˜ÂªÃ˜ÂºÃ›Å’Ã›Å’Ã˜Â± Ã™Ë†Ã˜Â¶Ã˜Â¹Ã›Å’Ã˜Âª Ã˜Â¨Ã˜Â§ Ã˜Â´Ã˜Â±Ã˜Â· Status = 0.
                // Ã˜Â§Ã›Å’Ã™â€  Ã˜Â´Ã˜Â±Ã˜Â· Ã˜ÂªÃ˜Â¶Ã™â€¦Ã›Å’Ã™â€  Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã˜Â¯ Ã˜Â¯Ã™Ë† ÃšÂ©Ã˜Â§Ã˜Â±Ã˜Â¨Ã˜Â± Ã™â€¡Ã™â€¦Ã˜Â²Ã™â€¦Ã˜Â§Ã™â€  Ã™ÂÃ™â€šÃ˜Â· Ã›Å’ÃšÂ©Ã›Å’ Ã˜Â¨Ã˜ÂªÃ™Ë†Ã˜Â§Ã™â€ Ã˜Â¯ Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯ Ã˜Â±Ã˜Â§ Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯ ÃšÂ©Ã™â€ Ã˜Â¯.
                var statusRows = await _dbSet
                    .Where(r => r.ReceiptId == receiptId && r.Status == 0)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(r => r.Status, (byte)1)
                        .SetProperty(r => r.ApprovedByUserId, currentUserId)
                        .SetProperty(r => r.ApprovedByFullName, approverName));
                if (statusRows == 0)
                {
                    await transaction.RollbackAsync();
                    return (false, "Ã˜Â§Ã›Å’Ã™â€  Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯ Ã™â€šÃ˜Â¨Ã™â€žÃ˜Â§Ã™â€¹ Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â´Ã˜Â¯Ã™â€¡ Ã˜Â§Ã˜Â³Ã˜Âª.");
                }
                // 5. Ã˜Â§Ã™ÂÃ˜Â²Ã˜Â§Ã›Å’Ã˜Â´ Ã˜Â§Ã˜ÂªÃ™â€¦Ã›Å’ÃšÂ© Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã™â€¦Ã˜Â­Ã˜ÂµÃ™Ë†Ã™â€ž
                await _context.Set<Product>()
                    .Where(p => p.ProductId == productId)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.Number, p => (p.Number ?? 0) + quantity));
                // 6. Ã˜Â§Ã™ÂÃ˜Â²Ã˜Â§Ã›Å’Ã˜Â´ Ã˜Â§Ã˜ÂªÃ™â€¦Ã›Å’ÃšÂ© Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’ Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â±
                await _context.Set<Warehouse>()
                    .Where(w => w.WarehouseId == warehouseId)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(w => w.Number, w => w.Number + quantity));
                await transaction.CommitAsync();
                return (true, "Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯ Ã˜Â¨Ã˜Â§ Ã™â€¦Ã™Ë†Ã™ÂÃ™â€šÃ›Å’Ã˜Âª Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â´Ã˜Â¯ Ã™Ë† Ã™â€¦Ã™Ë†Ã˜Â¬Ã™Ë†Ã˜Â¯Ã›Å’Ã¢â‚¬Å’Ã™â€¡Ã˜Â§ Ã˜Â¨Ã™â€¡Ã¢â‚¬Å’Ã˜Â±Ã™Ë†Ã˜Â²Ã˜Â±Ã˜Â³Ã˜Â§Ã™â€ Ã›Å’ Ã˜Â´Ã˜Â¯Ã™â€ Ã˜Â¯.");
            });
        }

        /// <summary>
        /// Ã™â€žÃ˜ÂºÃ™Ë† Ã›Å’ÃšÂ© Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯ (Ã˜ÂªÃ˜ÂºÃ›Å’Ã›Å’Ã˜Â± Ã™Ë†Ã˜Â¶Ã˜Â¹Ã›Å’Ã˜Âª Ã˜Â¨Ã™â€¡ 2)
        /// </summary>
        /// <param name="receiptId">Ã˜Â´Ã™â€ Ã˜Â§Ã˜Â³Ã™â€¡ Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯</param>
        /// <returns>true Ã˜Â¯Ã˜Â± Ã˜ÂµÃ™Ë†Ã˜Â±Ã˜Âª Ã™â€¦Ã™Ë†Ã™ÂÃ™â€šÃ›Å’Ã˜ÂªÃ˜Å’ false Ã˜Â§ÃšÂ¯Ã˜Â± Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯ Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª Ã™â€ Ã˜Â´Ã˜Â¯</returns>
        public async Task<bool> CancelReceiptAsync(int receiptId)
        {
            var receipt = await GetByIdAsync(receiptId);
            if (receipt == null) return false;
            receipt.Status = 2;
            await UpdateAsync(receipt);
            return true;
        }

        /// <summary>
        /// Ã˜Â¨Ã™â€¡Ã¢â‚¬Å’Ã˜Â±Ã™Ë†Ã˜Â²Ã˜Â±Ã˜Â³Ã˜Â§Ã™â€ Ã›Å’ Ã›Å’ÃšÂ© Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯.
        /// Ã™â€šÃ™ÂÃ™â€ž Ã˜Â±Ã˜Â§ <c>GenericRepository.UpdateAsync</c> Ã™â€¦Ã›Å’Ã¢â‚¬Å’ÃšÂ¯Ã›Å’Ã˜Â±Ã˜Â¯Ã˜â€º ÃšÂ¯Ã˜Â±Ã™ÂÃ˜ÂªÃ™â€  Ã˜Â¯Ã™Ë†Ã˜Â¨Ã˜Â§Ã˜Â±Ã™â€¡Ã¢â‚¬Å’Ã›Å’
        /// Ã˜Â¢Ã™â€  Ã˜Â±Ã™Ë†Ã›Å’ SemaphoreSlim(1) Ã˜Â¨Ã˜Â§Ã˜Â¹Ã˜Â« Ã˜Â¨Ã™â€ Ã¢â‚¬Å’Ã˜Â¨Ã˜Â³Ã˜Âª Ã™â€¦Ã›Å’Ã¢â‚¬Å’Ã˜Â´Ã˜Â¯.
        /// </summary>
        /// <param name="entity">Ã˜Â´Ã›Å’Ã˜Â¡ GoodsReceipt Ã˜Â¨Ã˜Â§ Ã™â€¦Ã™â€šÃ˜Â§Ã˜Â¯Ã›Å’Ã˜Â± Ã˜Â¬Ã˜Â¯Ã›Å’Ã˜Â¯</param>
        /// <returns>Ã˜Â´Ã›Å’Ã˜Â¡ Ã˜Â¨Ã™â€¡Ã¢â‚¬Å’Ã˜Â±Ã™Ë†Ã˜Â² Ã˜Â´Ã˜Â¯Ã™â€¡</returns>
        public override async Task<GoodsReceipt> UpdateAsync(GoodsReceipt entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return await base.UpdateAsync(entity);
        }

        // ========== Ã™â€¦Ã˜ÂªÃ˜Â¯Ã™â€¡Ã˜Â§Ã›Å’ DTO (Ã™â€ Ã›Å’Ã˜Â§Ã˜Â² Ã˜Â¨Ã™â€¡ Ã™â€šÃ™ÂÃ™â€ž Ã˜Â¯Ã˜Â§Ã˜Â±Ã™â€ Ã˜Â¯ Ãšâ€ Ã™Ë†Ã™â€  Ã˜Â¨Ã™â€¡ Ã˜Â¯Ã›Å’Ã˜ÂªÃ˜Â§Ã˜Â¨Ã›Å’Ã˜Â³ Ã˜Â¯Ã˜Â³Ã˜ÂªÃ˜Â±Ã˜Â³Ã›Å’ Ã˜Â¯Ã˜Â§Ã˜Â±Ã™â€ Ã˜Â¯) ==========

        /// <summary>
        /// Ã˜Â¯Ã˜Â±Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª Ã˜ÂªÃ˜Â¹Ã˜Â¯Ã˜Â§Ã˜Â¯ Ã™â€¦Ã˜Â´Ã˜Â®Ã˜ÂµÃ›Å’ Ã˜Â§Ã˜Â² Ã˜Â¢Ã˜Â®Ã˜Â±Ã›Å’Ã™â€  Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯Ã™â€¡Ã˜Â§ Ã˜Â¨Ã™â€¡ Ã™â€¡Ã™â€¦Ã˜Â±Ã˜Â§Ã™â€¡ Ã˜Â¬Ã˜Â²Ã˜Â¦Ã›Å’Ã˜Â§Ã˜Âª (Ã™â€ Ã˜Â§Ã™â€¦ Ã™â€¦Ã˜Â­Ã˜ÂµÃ™Ë†Ã™â€žÃ˜Å’ Ã˜ÂªÃ˜Â£Ã™â€¦Ã›Å’Ã™â€ Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã™â€ Ã˜Â¯Ã™â€¡Ã˜Å’ Ã˜Â§Ã™â€ Ã˜Â¨Ã˜Â§Ã˜Â±Ã˜Å’ ÃšÂ©Ã˜Â§Ã˜Â±Ã˜Â¨Ã˜Â± Ã™Ë† ...)
        /// </summary>
        /// <param name="topCount">Ã˜ÂªÃ˜Â¹Ã˜Â¯Ã˜Â§Ã˜Â¯ Ã˜Â±ÃšÂ©Ã™Ë†Ã˜Â±Ã˜Â¯ Ã™â€¦Ã™Ë†Ã˜Â±Ã˜Â¯ Ã™â€ Ã˜Â¸Ã˜Â± (Ã™Â¾Ã›Å’Ã˜Â´Ã¢â‚¬Å’Ã™ÂÃ˜Â±Ã˜Â¶ Ã›Â²Ã›Â°)</param>
        /// <returns>Ã™â€žÃ›Å’Ã˜Â³Ã˜ÂªÃ›Å’ Ã˜Â§Ã˜Â² GoodsReceiptDto</returns>
        public async Task<List<GoodsReceiptDto>> GetTopReceiptsWithDetailsAsync(int topCount)
        {
            if (topCount <= 0) topCount = 20;

            // (Ã˜Â§Ã˜Â®Ã˜ÂªÃ›Å’Ã˜Â§Ã˜Â±Ã›Å’) Ã˜Â¯Ã˜Â± Ã˜ÂµÃ™Ë†Ã˜Â±Ã˜Âª Ã™â€ Ã›Å’Ã˜Â§Ã˜Â² Ã™â€šÃ™ÂÃ™â€ž Ã˜Â±Ã˜Â§ Ã™ÂÃ˜Â¹Ã˜Â§Ã™â€ž ÃšÂ©Ã™â€ Ã›Å’Ã˜Â¯
            var receipts = await _dbSet
                .Include(r => r.Product)
                .Include(r => r.Supplier)
                .Include(r => r.Warehouse)
                .Where(r => r.Status == 0)
                .OrderByDescending(r => r.ReceiptDate)
                .Take(topCount)
                .AsNoTracking()
                .ToListAsync();
            if (receipts == null || !receipts.Any())
                return new List<GoodsReceiptDto>();
            var userIds = receipts.Where(r => !string.IsNullOrEmpty(r.UserId)).Select(r => r.UserId).Distinct().ToList();
            Dictionary<string, string> userNames = new Dictionary<string, string>();
            if (userIds.Any())
            {
               
                var users = await _userManager.Users.Where(u => userIds.Contains(u.Id)).Select(u => new { u.Id, u.FullName }).ToListAsync();
                userNames = users.ToDictionary(u => u.Id, u => u.FullName);
            }
            var result = new List<GoodsReceiptDto>();
            foreach (var receipt in receipts)
            {
                string userName = (!string.IsNullOrEmpty(receipt.UserId) && userNames.ContainsKey(receipt.UserId))
                    ? userNames[receipt.UserId]
                    : "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ";
                result.Add(new GoodsReceiptDto
                {
                    ReceiptId = receipt.ReceiptId,
                    ProductName = receipt.Product?.Name ?? "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ",
                    SupplierName = receipt.Supplier?.Name ?? "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ",
                    WarehouseName = receipt.Warehouse?.Name ?? "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ",
                    UserFullName = userName,
                    Quantity = receipt.Quantity,
                    Unit = receipt.Unit,
                    UnitPrice = receipt.UnitPrice,
                    TotalPrice = receipt.Quantity * receipt.UnitPrice,
                    InvoiceNumber = receipt.InvoiceNumber,
                    InvoiceDate = receipt.InvoiceDate,
                    ReceiptDate = receipt.ReceiptDate,
                    StatusText = receipt.Status == 0 ? "Ã˜Â¯Ã˜Â± Ã˜Â§Ã™â€ Ã˜ÂªÃ˜Â¸Ã˜Â§Ã˜Â± Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯" : (receipt.Status == 1 ? "Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â´Ã˜Â¯Ã™â€¡" : "Ã™â€žÃ˜ÂºÃ™Ë† Ã˜Â´Ã˜Â¯Ã™â€¡"),
                    Status = receipt.Status,
                    BatchNumber = receipt.BatchNumber ?? string.Empty,
                    Description = receipt.Description ?? string.Empty,
                    ApprovedByFullName= receipt.ApprovedByFullName,
                });
            }
            return result;
        }

        /// <summary>
        /// Ã˜Â¬Ã˜Â³Ã˜ÂªÃ˜Â¬Ã™Ë†Ã›Å’ Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯Ã™â€¡Ã˜Â§ Ã˜Â¨Ã˜Â± Ã˜Â§Ã˜Â³Ã˜Â§Ã˜Â³ ÃšÂ©Ã™â€žÃ™â€¦Ã™â€¡ ÃšÂ©Ã™â€žÃ›Å’Ã˜Â¯Ã›Å’ (Ã˜Â¯Ã˜Â± Ã˜Â´Ã™â€¦Ã˜Â§Ã˜Â±Ã™â€¡ Ã™ÂÃ˜Â§ÃšÂ©Ã˜ÂªÃ™Ë†Ã˜Â±Ã˜Å’ Ã™â€ Ã˜Â§Ã™â€¦ Ã™â€¦Ã˜Â­Ã˜ÂµÃ™Ë†Ã™â€žÃ˜Å’ Ã™â€ Ã˜Â§Ã™â€¦ Ã˜ÂªÃ˜Â£Ã™â€¦Ã›Å’Ã™â€ Ã¢â‚¬Å’ÃšÂ©Ã™â€ Ã™â€ Ã˜Â¯Ã™â€¡)
        /// </summary>
        /// <param name="keyword">Ã˜Â¹Ã˜Â¨Ã˜Â§Ã˜Â±Ã˜Âª Ã˜Â¬Ã˜Â³Ã˜ÂªÃ˜Â¬Ã™Ë†</param>
        /// <param name="topCount">Ã˜Â­Ã˜Â¯Ã˜Â§ÃšÂ©Ã˜Â«Ã˜Â± Ã˜ÂªÃ˜Â¹Ã˜Â¯Ã˜Â§Ã˜Â¯ Ã™â€ Ã˜ÂªÃ›Å’Ã˜Â¬Ã™â€¡ (Ã™Â¾Ã›Å’Ã˜Â´Ã¢â‚¬Å’Ã™ÂÃ˜Â±Ã˜Â¶ Ã›ÂµÃ›Â°)</param>
        /// <returns>Ã™â€žÃ›Å’Ã˜Â³Ã˜ÂªÃ›Å’ Ã˜Â§Ã˜Â² GoodsReceiptDto</returns>
        public async Task<List<GoodsReceiptDto>> SearchReceiptsAsync(string keyword, int topCount = 50)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetTopReceiptsWithDetailsAsync(topCount);

            keyword = keyword.Trim().ToLower();

            var query = _dbSet
                .Include(r => r.Product)
                .Include(r => r.Supplier)
                .Include(r => r.Warehouse)
                .Where(r => // Ã™â€¡Ã™â€¦Ã™â€¡ Ã™Ë†Ã˜Â¶Ã˜Â¹Ã›Å’Ã˜ÂªÃ¢â‚¬Å’Ã™â€¡Ã˜Â§: Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯ Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯Ã˜Â´Ã˜Â¯Ã™â€¡ Ã™â€¡Ã™â€¦ Ã˜Â¨Ã˜Â§Ã›Å’Ã˜Â¯ Ã˜Â¨Ã˜Â§ Ã˜Â´Ã™â€¦Ã˜Â§Ã˜Â±Ã™â€¡ Ã™ÂÃ˜Â§ÃšÂ©Ã˜ÂªÃ™Ë†Ã˜Â± Ã™Â¾Ã›Å’Ã˜Â¯Ã˜Â§ Ã˜Â´Ã™Ë†Ã˜Â¯
                            (r.InvoiceNumber != null && r.InvoiceNumber.ToLower().Contains(keyword)) ||
                            (r.Product != null && r.Product.Name.ToLower().Contains(keyword)) ||
                            (r.Supplier != null && r.Supplier.Name.ToLower().Contains(keyword)))
                .OrderByDescending(r => r.ReceiptDate)
                .Take(topCount)
                .AsNoTracking();
            var receipts = await query.ToListAsync();
            var result = new List<GoodsReceiptDto>();
            foreach (var receipt in receipts)
            {
                // Ã˜Â¯Ã˜Â±Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª Ã™â€ Ã˜Â§Ã™â€¦ ÃšÂ©Ã˜Â§Ã˜Â±Ã˜Â¨Ã˜Â± Ã˜Â¨Ã˜Â§ Ã™â€¦Ã˜Â¯Ã›Å’Ã˜Â±Ã›Å’Ã˜Âª Ã˜Â®Ã˜Â·Ã˜Â§
                string userName = "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ";
                if (!string.IsNullOrEmpty(receipt.UserId))
                {
                    userName = await _usermanagementService.GetUserNameByIdAsync(receipt.UserId);
                    if (string.IsNullOrEmpty(userName))
                        userName = "Ù†Ø§Ù…Ø´Ø®Øµ";
                }
                result.Add(new GoodsReceiptDto
                {
                    ReceiptId = receipt.ReceiptId,
                    ProductName = receipt.Product?.Name ?? "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ",
                    SupplierName = receipt.Supplier?.Name ?? "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ",
                    WarehouseName = receipt.Warehouse?.Name ?? "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ",
                    UserFullName = userName,
                    Quantity = receipt.Quantity,
                    Unit = receipt.Unit,
                    UnitPrice = receipt.UnitPrice,
                    TotalPrice = receipt.Quantity * receipt.UnitPrice,
                    InvoiceNumber = receipt.InvoiceNumber,
                    InvoiceDate = receipt.InvoiceDate,
                    ReceiptDate = receipt.ReceiptDate,
                    StatusText = receipt.Status == 0 ? "Ã˜Â¯Ã˜Â± Ã˜Â§Ã™â€ Ã˜ÂªÃ˜Â¸Ã˜Â§Ã˜Â± Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯" : (receipt.Status == 1 ? "Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â´Ã˜Â¯Ã™â€¡" : "Ã™â€žÃ˜ÂºÃ™Ë† Ã˜Â´Ã˜Â¯Ã™â€¡"),
                    Status = receipt.Status,
                    BatchNumber = receipt.BatchNumber ?? string.Empty,
                    Description = receipt.Description ?? string.Empty
                });
            }
            return result;
        }

        public async Task<List<GoodsReceiptDto>> SearchReceipts(string keyword, int topCount = 50)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return await GetTopReceiptsWithDetailsAsync(topCount);

            keyword = keyword.Trim().ToLower();

            var query = _dbSet
                .Include(r => r.Product)
                .Include(r => r.Supplier)
                .Include(r => r.Warehouse)
                .Where(r => r.Status == 1 && // Ã¢Å“â€¦ Ã™ÂÃ™â€šÃ˜Â· Ã˜Â±Ã˜Â³Ã›Å’Ã˜Â¯ Ã™â€¡Ã˜Â§Ã›Å’ Ã˜ÂªÃ˜Â§Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â´Ã˜Â¯Ã™â€¡
                            (r.InvoiceNumber.ToLower().Contains(keyword) ||
                             (r.Product != null && r.Product.Name.ToLower().Contains(keyword)) ||
                             (r.Supplier != null && r.Supplier.Name.ToLower().Contains(keyword))))
                .OrderByDescending(r => r.ReceiptDate)
                .Take(topCount)
                .AsNoTracking();
            var receipts = await query.ToListAsync();
            var result = new List<GoodsReceiptDto>();
            foreach (var receipt in receipts)
            {
                // Ã˜Â¯Ã˜Â±Ã›Å’Ã˜Â§Ã™ÂÃ˜Âª Ã™â€ Ã˜Â§Ã™â€¦ ÃšÂ©Ã˜Â§Ã˜Â±Ã˜Â¨Ã˜Â± Ã˜Â¨Ã˜Â§ Ã™â€¦Ã˜Â¯Ã›Å’Ã˜Â±Ã›Å’Ã˜Âª Ã˜Â®Ã˜Â·Ã˜Â§
                string userName = "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ";
                if (!string.IsNullOrEmpty(receipt.UserId))
                {
                    userName = await _usermanagementService.GetUserNameByIdAsync(receipt.UserId);
                    if (string.IsNullOrEmpty(userName))
                        userName = "Ù†Ø§Ù…Ø´Ø®Øµ";
                }
                result.Add(new GoodsReceiptDto
                {
                    ReceiptId = receipt.ReceiptId,
                    ProductName = receipt.Product?.Name ?? "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ",
                    SupplierName = receipt.Supplier?.Name ?? "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ",
                    WarehouseName = receipt.Warehouse?.Name ?? "Ã™â€ Ã˜Â§Ã™â€¦Ã˜Â´Ã˜Â®Ã˜Âµ",
                    UserFullName = userName,
                    Quantity = receipt.Quantity,
                    Unit = receipt.Unit,
                    UnitPrice = receipt.UnitPrice,
                    TotalPrice = receipt.Quantity * receipt.UnitPrice,
                    InvoiceNumber = receipt.InvoiceNumber,
                    InvoiceDate = receipt.InvoiceDate,
                    ReceiptDate = receipt.ReceiptDate,
                    StatusText = receipt.Status == 0 ? "Ã˜Â¯Ã˜Â± Ã˜Â§Ã™â€ Ã˜ÂªÃ˜Â¸Ã˜Â§Ã˜Â± Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯" : (receipt.Status == 1 ? "Ã˜ÂªÃ˜Â£Ã›Å’Ã›Å’Ã˜Â¯ Ã˜Â´Ã˜Â¯Ã™â€¡" : "Ã™â€žÃ˜ÂºÃ™Ë† Ã˜Â´Ã˜Â¯Ã™â€¡"),
                    Status = receipt.Status,
                    BatchNumber = receipt.BatchNumber ?? string.Empty,
                    Description = receipt.Description ?? string.Empty
                });
            }
            return result;
        }

    }
}
