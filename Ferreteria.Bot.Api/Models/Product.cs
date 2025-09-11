namespace Ferreteria.Bot.Api.Models
{
    public class Product
    {
        public int Id { get; set; }                // PK
        public string ProductId { get; set; }      // product_id (ej: "P-001")
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Variant { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public int LeadTimeDays { get; set; }
        public string ImageUrl { get; set; }
    }
}
