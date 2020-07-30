using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using aspCore.WatchShop.Entities;
using aspCore.WatchShop.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using aspCore.WatchShop.EF;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace aspCore.WatchShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {

        private ProductModel _model = null;
        public ProductController(IMemoryCache cache, watchContext context, IMapper mapper)
        {
            _model = new ProductModel(context, mapper, cache);
        }

        // HTML, Categotys. Wires
        public ActionResult Index()
        {
            ViewBag.Category = _model.GetCategories();
            ViewBag.Wires = _model.GetTypeWires();
            return View();
        }

        //******** List Product*******//
        public JsonResult Products()
        {
            return Json(_model.GetAllProduct());
        }

        [Route("Category")]
        public JsonResult ProductByCategory(int indexCate)
        {
            return Json(_model.GetProductByCate(indexCate));
        }

        [Route("Sale")]
        public JsonResult ProductSales()
        {
            // List<ProductVM> result = _model.GetProductsOnSale();
            // return Json(result);
            return null;
        }

        public JsonResult ProductOnSales(int sale)
        {
            // List<ProductVM> result = _model.GetProductsOnSale(-1);
            // return Json(result);
            return null;
        }

        public JsonResult Find(string key)
        {
            return Json(_model.FindWithKey(key));
        }

        //******** Detail *******/
        public ActionResult Detail(int id)
        {
            ViewBag.Category = _model.GetCategories();
            ViewBag.Wires = _model.GetTypeWires();
            ViewBag.Props = new ProductDetail().GetType().GetProperties().Select(o => o.Name).Skip(2).SkipLast(2).ToArray();
            return View("ProductDetail", _model.GetProductDetail(id));
        }

        public ActionResult Create()
        {
            ViewBag.Category = _model.GetCategories();
            ViewBag.Wires = _model.GetTypeWires();
            ViewBag.Props = new ProductDetail().GetType().GetProperties().Select(o => o.Name).ToArray();
            return View("ProductDetail", new ProductDetailVM());
        }

        public NoContentResult UpdateStatus(int productId, int indexStatus)
        {
            _model.UpdateStatusProduct(productId, indexStatus);
            return NoContent();
        }

        public ActionResult Delete(int id)
        {
            _model.DeleteProduct(id);
            return RedirectToAction("Index");
        }

        public ActionResult Submit(ProductDetailVM p, string Files)
        {
            if (p.ID >= 1)
                _model.UpdateProduct(p);
            else
                _model.AddProduct(p);
            return RedirectToAction("Index");
        }

        public string UploadImage(IFormFile img)
        {
            if (img != null)
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\product", img.FileName);
                img.CopyToAsync(new FileStream(path, FileMode.Create));
                return img.FileName;
            }
            return "";
        }

    }
}