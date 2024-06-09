namespace ShoppingCart.Models
{
    public class ProductsCrudViewModel
    {
        public IEnumerable<DbService.Product> Products { get;set; }
        public string Alert { get; set; }
        public string TypeAlert { get; set; }

        public List<KeyValueViewModel> Categories { get; set; }
    }
}
