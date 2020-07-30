using Microsoft.AspNetCore.Mvc;
using aspCore.WatchShop.EF;
using aspCore.WatchShop.Models;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;

namespace aspCore.WatchShop.Controllers
{
    public class ProductController : Controller
    {
        private ProductModel _productModel = null;
        public ProductController(IMemoryCache cache, watchContext context, IMapper mapper)
        {
            _productModel = new ProductModel(context, mapper, cache);
        }

        public IActionResult ProductDetail(int id)
        {
            return View(_productModel.GetProductDetail(id));
        }

        public IActionResult Promotions()
        {
            ViewBag.Title = "Khuyến mãi";
            ViewBag.RequestName = "promotions";
            return View("Products");
        }

        public IActionResult Men()
        {
            ViewBag.Title = "Đồng hồ nam"; ;
            ViewBag.RequestName = "men";
            return View("Products");
        }

        public IActionResult Women()
        {
            ViewBag.Title = "Đồng hồ nữ"; ;
            ViewBag.RequestName = "women";
            return View("Products");
        }

        public IActionResult Accessories()
        {
            ViewBag.Title = "Phụ kiện";
            ViewBag.RequestName = "accessories";
            return View("Products");
        }

        #region GET DATA WITH AJAX
        public JsonResult Search(string key)
        {
            return Json(_productModel.FindWithKey(key));
        }

        public JsonResult GetPromotions()
        {
            return Json(_productModel.GetProductPromotion());
        }

        public JsonResult GetCategory(int cateID)
        {
            return Json(_productModel.GetProductByCate(cateID));
        }

        public JsonResult GetRelations(string key)
        {
            if (key == null) return Json(null);
            return Json(_productModel.GetProductRelation(key));
        }
        #endregion




    }
}