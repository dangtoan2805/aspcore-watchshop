using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using aspCore.WatchShop.DAOs;
using aspCore.WatchShop.Entities;
using aspCore.WatchShop.Helper;
using aspCore.WatchShop.EF;
using aspCore.WatchShop.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace aspCore.WatchShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private OrderModel _model = null;

        public OrderController(watchContext context, IMapper mapper)
        {
            _model = new OrderModel(context, mapper);
        }

        public ActionResult Index() => View();

        public JsonResult Find(string key)
        {
            return Json(_model.FindOrder(key));
        }

        public JsonResult Orders(string start, string end)
        {
            return Json(_model.GetOrders(start, end));
        }

        public NoContentResult UpdateStatus(int orderID, int index)
        {
            _model.UpdateStatus(orderID, index);
            return NoContent();
        }

        public JsonResult Detail(int id)
        {
            return Json(_model.GetDetail(id));
        }
    }
}