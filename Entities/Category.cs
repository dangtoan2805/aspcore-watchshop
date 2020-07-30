using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace aspCore.WatchShop.Entities
{
    public class Category
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(30)]
        public string Name { get; set; }
        //Nav property
        public List<Product> Products { get; set; }
    }
}
