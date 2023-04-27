namespace mvc_products.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }

        public int Quantity { get; set; } = 0;

        public double UnitPrice { get; set; } = 0;


        public double TotalPrice
        {
            get { return Quantity * UnitPrice; }


        }

    }

}
