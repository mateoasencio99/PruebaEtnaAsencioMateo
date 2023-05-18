using WebApplication1.Models;

namespace ShoppingCart.Models
{
    public class FinalCartViewModel
    {
        public IEnumerable<FinalCartGridViewModel> Products { get; set; }
        public string Alert { get; set; }
        public string TypeAlert { get; set; }
    }
}
