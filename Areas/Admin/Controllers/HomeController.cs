using System;
using System.Linq;
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
    public class HomeController : Controller
    {
        private OrderModel _model;

        public HomeController(watchContext context, IMapper mapper)
        {
            _model = new OrderModel(context, mapper);
        }
        public ActionResult Index()
        {
            var data = _model.GetCountStatus();
            ViewBag.Dones = data.Item1;
            ViewBag.Deliverys = data.Item2;
            ViewBag.Received = data.Item3;
            ViewBag.TotalRevenue = data.Item4;
            return View();
        }

        public JsonResult OrderReport(string start, string end)
        {
            return Json(_model.GetReport(start, end));
        }

    }
}