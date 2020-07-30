using System.Collections.Generic;
using System.Linq;
using aspCore.WatchShop.EF;
using aspCore.WatchShop.Entities;

namespace aspCore.WatchShop.DAOs
{
    public class CustomerDao
    {
        private watchContext _db = null;
        public CustomerDao(watchContext context)
        {
            _db = context;
        }

        public List<Customer> GetCusomters()
        {
            return _db.Customers.ToList();
        }
    }
}