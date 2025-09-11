namespace Ferreteria.Bot.Api.Models
{
    public class Order
    {
        public int Id { get; set; }              // PK
        public string OrderId { get; set; }      // e.g. "O-0001"
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public string Currency { get; set; } = "UYU";
        public string Status { get; set; } = "nuevo";
        public string DeliveryAddress { get; set; }
        public string Notes { get; set; }
    }
}
