namespace ShoppingCart.Models
{
    public class PurcharseViewModel
    {
        public IEnumerable<DbService.Product> Products { get; set; }
        public string Alert { get; set; }
        public string TypeAlert { get; set; }
    }
}
