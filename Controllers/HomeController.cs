using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using aspCore.WatchShop.Models;
using aspCore.WatchShop.EF;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;

namespace aspCore.WatchShop.Controllers
{
    public class HomeController : Controller
    {
        private ProductModel _productModel = null;
        public HomeController(IMemoryCache cache, watchContext context, IMapper mapper)
        {
            _productModel = new ProductModel(context, mapper, cache);
        }

        public IActionResult Index()
        {
            ViewBag.PromotionWatch = _productModel.TakeProductPromotion(4);
            ViewBag.MenWatch = _productModel.TakeProducByCate(4, 1);
            ViewBag.WomenWatch = _productModel.TakeProducByCate(4, 2);
            return View();
        }

        public IActionResult Result()
        {
            return View();
        }
    }
}
