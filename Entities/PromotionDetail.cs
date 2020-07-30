namespace aspCore.WatchShop.Entities
{
    public class PromotionDetail
    {
        public int PromotionID { get; set; }
        public int ProductID { get; set; }
        //Nav property
        public Product Product { get; set; }
        public Promotion Promotion { get; set; }
    }
}