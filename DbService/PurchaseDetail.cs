using System.ComponentModel.DataAnnotations;

namespace DbService
{
    public class PurchaseDetail
    {
        public long Id { get; set; }
        public long PurchaseId { get; set; }
        [StringLength(100)]
        public string ProductName { get; set; }
        public  int Quantity { get; set; }
        public decimal Price { get; set; }

        public virtual Purchase Purchase { get; set; }
        public virtual Product Product { get; set; }
    }
}
