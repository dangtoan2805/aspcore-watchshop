using Microsoft.AspNetCore.Mvc;
using aspCore.WatchShop.EF;
using aspCore.WatchShop.Models;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace aspCore.WatchShop.Controllers
{
    public class CartController : Controller
    {
        private ProductModel _productModel = null;
        private CartModel _cartModel = null;
        private OrderModel _orderModel = null;

        public CartController(IMemoryCache cache, watchContext context, IMapper mapper)
        {
            _cartModel = new CartModel(context, mapper);
            _orderModel = new OrderModel(context, mapper);
            _productModel = new ProductModel(context, mapper, cache);
        }
        public IActionResult Index()
        {
            ViewBag.Cart = _cartModel.GetCart(HttpContext.Session);
            ViewBag.Tax = _orderModel.GetTax();
            ViewBag.Tranpost = _orderModel.GetTranpost();
            return View();
        }

        [HttpPost]
        public IActionResult Submit(OrderDetailVM order)
        {
            _orderModel.AddNewOrder(order, _cartModel.GetCart(HttpContext.Session));
            return RedirectToAction("Index", "Home");
        }

        public IActionResult BuyNow(int productID)
        {
            AddItem(productID);
            return RedirectToAction("Index");
        }

        //AJAX
        public EmptyResult AddItem(int productID)
        {
            var product = _productModel.FindByID(productID);
            if (product != null) _cartModel.AddItem(HttpContext.Session, product);
            return new EmptyResult();
        }
        public NoContentResult RemoveItem(int productID)
        {
            _cartModel.RemoveItem(HttpContext.Session, productID);
            return NoContent();
        }
    }
}