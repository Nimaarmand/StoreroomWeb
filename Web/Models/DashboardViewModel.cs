namespace Web.Models;

public class DashboardViewModel
{
    public int ProductCount { get; set; }
    public int ActiveProductCount { get; set; }
    public int WarehouseCount { get; set; }
    public int TotalStock { get; set; }

    public List<LowStockRow> LowStockProducts { get; set; } = new();
    public List<WarehouseStockRow> WarehouseStocks { get; set; } = new();

    public class LowStockRow
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public int Stock { get; set; }
        public decimal? MinStockLevel { get; set; }
    }

    public class WarehouseStockRow
    {
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public int Number { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }

        /// <summary>درصد پرشدن ظرفیت انبار</summary>
        public int FillPercent => Max > 0 ? Math.Min(100, (int)Math.Round(Number * 100.0 / Max)) : 0;
    }
}
