using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace aspCore.WatchShop.Entities
{
    public class Promotion
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? Status { get; set; }
        public decimal Discount { get; set; }
        //Nav property
        public List<PromotionDetail> PromotionDetail { get; set; }
    }
}
