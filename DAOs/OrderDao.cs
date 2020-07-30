using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;
using aspCore.WatchShop.EF;
using aspCore.WatchShop.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace aspCore.WatchShop.DAOs
{
    public class OrderDao
    {
        private watchContext _db = null;

        public OrderDao(watchContext context)
        {
            _db = context;
        }

        #region GET
        public List<Order> GetOrders(DateTime start, DateTime end)
        {
            var data = _db.Orders
                .Where(o => o.DateCreated >= start && o.DateCreated <= end)
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ToList();
            return data.Count <= 0 ? null : data;
        }

        public List<Order> GetAllOrders()
        {
            return _db.Orders
                .Include(p => p.OrderDetails)
                .ToList();
        }

        public List<OrderDetail> GetOrderDetailsWithProduct(int orderID)
        {
            // Get OrderDetail Inluce ID,Name, Img of Product
            return (List<OrderDetail>)_db.OrderDetails
                .Where(od => od.OrderID == orderID)
                .Include(od => od.Product)
                .ToList();
        }

        public Order GetOrderByID(int id)
        {
            // Get List Order Include Customer, OD
            return _db.Orders
                .Where(o => o.ID == id)
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .FirstOrDefault();
        }

        public List<Order> FindByCustomer(string name)
        {
            return _db.Orders
                .Where(o => o.Customer.Name.Contains(name))
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                .ToList();
        }

        public decimal GetTax() { return _db.Fees.FirstOrDefault().Tax; }
        public int GetTranport() { return _db.Fees.FirstOrDefault().Transport; }

        #endregion

        public void AddOrder(Customer cus, Order order, List<OrderDetail> cart)
        {
            //Get Frogien key
            int cusLastID = 1;
            int orderLastID = 1;
            if (_db.Customers.Count() > 0)
                cusLastID = _db.Customers.Max(item => item.ID) + 1;
            if (_db.Orders.Count() > 0)
                orderLastID = _db.Orders.Max(item => item.ID) + 1;
            //Hanler data
            order.CustomerID = cusLastID;
            order.OrderDetails = cart;
            cart.ForEach(item =>
            {
                item.OrderID = orderLastID;
                _db.Products.Where(p => p.ID == item.ProductID).FirstOrDefault().SaleCount++;
            });
            //Add data
            _db.Customers.Add(cus);
            _db.Orders.Add(order);
            _db.OrderDetails.AddRange(cart);
            _db.SaveChanges();
        }

        public void AddOrderDeital(List<OrderDetail> cart)
        {
            _db.OrderDetails.AddRange(cart);
        }

        public void UpdateOrderStatus(int orderID, int index)
        {
            Order order = _db.Orders.Where(o => o.ID == orderID).FirstOrDefault();
            if (order == null) return;
            order.Status = index;
            _db.SaveChanges();
        }


    }
}