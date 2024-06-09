namespace ShoppingCart.Models
{
    public class CategoryCrudViewModel
    {
        public List<DbService.Category> Categories { get; set; }
        public string Alert { get; set; }
        public string TypeAlert { get; set; }
    }
}
