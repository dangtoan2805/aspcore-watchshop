using System.Linq;
using System;
using System.Diagnostics;
using aspCore.WatchShop.EF;
using aspCore.WatchShop.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace aspCore.WatchShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PromotionController : Controller
    {
        private PromotionModel _model = null;
        private ProductModel _producModel = null;
        public PromotionController(watchContext context, IMapper mapper, IMemoryCache cache)
        {
            _model = new PromotionModel(context, mapper);
            _producModel = new ProductModel(context, mapper, cache);
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(int promID)
        {
            var data = _model.GetDetail(promID);
            ViewBag.Category = _producModel.GetCategories();

            return View(data);
        }

        public IActionResult Delete(int promID)
        {
            _model.DeletePromotion(promID);
            return View("Index");
        }

        public IActionResult Submit(PromotionVM promotion, int[] products)
        {
            if (products[0] == -1)
                products = _producModel.GetAllProduct().Select(item => item.ID).ToArray();
            _model.Submit(promotion, products);

            return View("Index");
        }

        #region AJAX        
        public JsonResult GetPromotion()
        {
            return Json(_model.GetPromotions());
        }

        public NoContentResult UpdateStatus(int promID, bool status)
        {
            _model.UpdateStatus(promID, status);
            return NoContent();
        }

        public JsonResult GetProductPromotion(int promID)
        {
            var arProductID = _model.GetProductPromotion(promID);
            return Json(_producModel.GetProducts().Where(item => arProductID.Contains(item.ID)));
        }

        public JsonResult GetProductIDByCate(int cateID)
        {
            return Json(_producModel.GetProductByCate(cateID).Select(item => item.ID).ToArray());
        }
        #endregion
    }
}