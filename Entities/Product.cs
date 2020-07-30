using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace aspCore.WatchShop.Entities
{
    public class Product
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public bool? isShow { get; set; }
        public bool? isDel { get; set; }
        public int Price { get; set; }
        public string Image { get; set; }
        public int SaleCount { get; set; }
        [ForeignKey("Category")]
        public int CategoryID { get; set; }
        [ForeignKey("TypeWire")]
        public int TypeWireID { get; set; }
        //Nav property
        public Category Category { get; set; }
        public TypeWire TypeWire { get; set; }
        public ProductDetail ProductDetail { get; set; }

    }
}
