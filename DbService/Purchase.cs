﻿namespace DbService
{
    public class Purchase
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string Observation { get; set; }


        public virtual List<PurchaseDetail> Details { get; set; }

        public decimal Total => Details.Sum(d => d.Price * d.Quantity);
    }
}
