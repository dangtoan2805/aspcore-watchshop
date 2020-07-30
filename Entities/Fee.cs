using System.ComponentModel.DataAnnotations;

namespace aspCore.WatchShop.Entities
{
    public class Fee
    {  
        [Key] 
        public int ID { get; set; }
        public decimal Tax { get; set; }
        public int Transport { get; set; }
    }
}
