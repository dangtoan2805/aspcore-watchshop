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
    public class OrderModel
    {
        private OrderDao _db = null;
        private IMapper _mapper = null;

        public OrderModel(watchContext context, IMapper mapper)
        {
            _db = new OrderDao(context);
            _mapper = mapper;
        }
        // =============== PRIVATE ================
        private OrderVM ConvertOrderVM(Order order)
        {
            if (order == null) return null;
            int orderID = order.ID;
            int sum = (int)order.OrderDetails.Sum(item => item.Quantity * (item.Price - item.Promotion));
            var obj = _mapper.Map<OrderVM>(order);
            obj.Total = (int)(sum * (1 + order.Tax));
            return obj;
        }

        // =============== PUBLIC ================
        public decimal GetTax() { return _db.GetTax(); }

        public int GetTranpost() { return _db.GetTranport(); }

        public List<OrderVM> GetOrders(string start, string end)
        {
            var data = _db.GetOrders(DateTime.Parse(start), DateTime.Parse(end).AddDays(1));
            if (data == null) return null;
            List<OrderVM> orders = new List<OrderVM>();
            data.ForEach(item => orders.Add(ConvertOrderVM(item)));
            return orders.OrderByDescending(item => item.DateCreated).ToList();
        }

        public OrderDetailVM GetDetail(int id)
        {
            Order order = _db.GetOrderByID(id);
            if (order == null) return null;
            OrderDetailVM detail = _mapper.Map<OrderDetailVM>(order);
            //Get Product in Order
            List<OrderDetail> ls = _db.GetOrderDetailsWithProduct(id);
            if (ls == null) return detail;
            List<ProductOrder> products = new List<ProductOrder>();
            ls.ForEach(item => products.Add(_mapper.Map<ProductOrder>(item)));
            detail.AddProductOrder(products);
            //Return
            return detail;
        }

        public Object FindOrder(string key)
        {
            int orderID;
            if (Int32.TryParse(key, out orderID))
            {
                var obj = _db.GetOrderByID(orderID);
                if (obj == null) return null;
                return new List<OrderVM>() { ConvertOrderVM(obj) };
            }
            List<OrderVM> orders = new List<OrderVM>();
            var data = _db.FindByCustomer(key);
            if (data == null) return null;
            data.ForEach(item => orders.Add(ConvertOrderVM(item)));
            return orders;
        }

        public void UpdateStatus(int orderID, int index)
        {
            _db.UpdateOrderStatus(orderID, index);
        }

        public void AddNewOrder(OrderDetailVM orderVM, List<ProductOrder> cart)
        {
            if (cart == null) return;
            orderVM.DateCreated = DateTime.Now;
            var cus = _mapper.Map<Customer>(orderVM);
            var order = _mapper.Map<Order>(orderVM);
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            cart.ForEach(item =>
            {
                var obj = _mapper.Map<OrderDetail>(item);
                orderDetails.Add(obj);
            });
            _db.AddOrder(cus, order, orderDetails);
        }

        #region Report
        public Tuple<int, int, int, int> GetCountStatus()
        {
            int dones = 0;
            int deliverys = 0;
            int recived = 0;
            int total = 0;
            var data = _db.GetAllOrders();
            if (data == null) return null;
            data.ForEach(item =>
            {
                switch (item.Status)
                {
                    case 2:
                        recived++;
                        break;
                    case 3:
                        deliverys++;
                        break;
                    case 4:
                        dones++;
                        break;
                }
                int sum = (int)item.OrderDetails.Sum(item => item.Quantity * (item.Price - item.Promotion));
                total = total + (int)(sum * (1 + item.Tax));
            });
            return new Tuple<int, int, int, int>(dones, deliverys, recived, total);
        }

        public Object GetReport(string start, string end)
        {
            var data = GetOrders(start, end);
            List<OrderReport> dates = new List<OrderReport>();
            DateTime dStart = DateTime.Parse(start);
            DateTime dEnd = DateTime.Parse(end);
            do
            {
                dates.Add(new OrderReport() { DateCreated = dStart });
                dStart = dStart.AddDays(1);
            } while (dStart.CompareTo(dEnd) <= 0);
            if (data == null) return dates;
            data.GroupBy(p => p.DateCreated)
                .Select(item => new
                {
                    DateCreated = item.Key,
                    CountOrder = item.Count(),
                    Total = item.Sum(p => p.Total)
                })
                .ToList()
                .ForEach(item =>
                {
                    int index = dates.FindIndex(date => date.DateCreated.CompareTo(item.DateCreated) == 0);
                    if (index != -1)
                    {
                        dates[index].CountOrder = item.CountOrder;
                        dates[index].Total = item.Total;
                    }
                });
            return dates;
        }

        #endregion
    }

    public class OrderReport
    {
        public DateTime DateCreated { get; set; }
        public int Total { get; set; }
        public int CountOrder { get; set; }
    }

    public class OrderVM
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }
        public string Phone { get; set; }
        public string Customer { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public int Total { get; set; }
    }

    public class OrderDetailVM
    {
        public int ID { get; set; }
        public DateTime DateCreated { get; set; }
        public string Note { get; set; }
        public int TransportFee { get; set; }
        public decimal Tax { get; set; }
        public string Phone { get; set; }
        public string Customer { get; set; }
        public string Address { get; set; }
        public int Status { get; set; }
        public List<ProductOrder> Products { get; set; }
        public int Total { get; set; }

        public OrderDetailVM() { }

        public void AddProductOrder(List<ProductOrder> orders)
        {
            this.Products = orders;
            int sum = (int)orders.Sum(item => item.Quantity * (item.Price - item.Promotion));
            this.Total = (int)(sum * (1 + this.Tax));
        }


    }

    public class ProductOrder
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public decimal Promotion { get; set; }
        public string Image { get; set; }
    }
}