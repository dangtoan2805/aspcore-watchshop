using System.Linq;
using System;
using System.Collections.Generic;
using aspCore.WatchShop.DAOs;
using aspCore.WatchShop.EF;
using aspCore.WatchShop.Entities;
using Microsoft.Extensions.Caching.Memory;
using AutoMapper;

namespace aspCore.WatchShop.Models
{
    public class CustomerModel
    {
        const string customerKey = "customers";
        private CustomerDao _db = null;
        private IMemoryCache _cache = null;
        private IMapper _mapper = null;

        public CustomerModel(watchContext context, IMapper mapper, IMemoryCache cache)
        {
            _db = new CustomerDao(context);
            _mapper = mapper;
            _cache = cache;
        }

        public List<CustomerVM> GetCustomers()
        {
            List<CustomerVM> customers = null;
            if (!_cache.TryGetValue(customerKey, out customers))
            {
                var rawData = _db.GetCusomters();
                if (rawData == null) return null;
                customers = new List<CustomerVM>();
                rawData.ForEach(item => customers.Add(_mapper.Map<CustomerVM>(item)));
                _cache.Set(customerKey, customers, DateTime.MaxValue);
            }
            return customers;
        }
    }

    public class CustomerVM
    {
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}