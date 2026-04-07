namespace SV22T1020439.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; } = "";
        public string Unit { get; set; } = "";
        public decimal Price { get; set; }
        public string Photo { get; set; } = "";
        public string ProductDescription { get; set; } = "";
        public int CategoryID { get; set; }
        public int SupplierID { get; set; }
        public bool IsSelling { get; set; }
        public string CategoryName { get; set; } = "";
        public string SupplierName { get; set; } = "";
    }
}
