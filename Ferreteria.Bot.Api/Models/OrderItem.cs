namespace Ferreteria.Bot.Api.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderIdRef { get; set; }
        public Order Order { get; set; }
        public int ProductIdRef { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
