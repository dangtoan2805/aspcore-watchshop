using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using aspCore.WatchShop.Entities;
using Microsoft.Extensions.Caching.Memory;
using aspCore.WatchShop.DAOs;
using aspCore.WatchShop.EF;
using System;
using System.Linq;
using aspCore.WatchShop.Helper;
using aspCore.WatchShop.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace aspCore.WatchShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CustomerController : Controller
    {
        private CustomerModel _model = null;
        public CustomerController(watchContext context, IMapper mapper, IMemoryCache cache)
        {
            _model = new CustomerModel(context, mapper, cache);
        }
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Customers()
        {
            return Json(_model.GetCustomers());
        }
    }
}