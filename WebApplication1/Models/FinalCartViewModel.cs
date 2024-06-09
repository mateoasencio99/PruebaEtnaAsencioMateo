using WebApplication1.Models;

namespace ShoppingCart.Models
{
    public class FinalCartViewModel
    {
        public List<FinalCartGridViewModel> Products { get; set; }
        public string Alert { get; set; }
        public string TypeAlert { get; set; }
        public long Id { get; set; }
        public string Date { get; set; }
        public string Observations { get; set; }
        public bool IsView {  get; set; }
    }
}
