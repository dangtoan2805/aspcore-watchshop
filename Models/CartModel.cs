using System;
using System.Collections.Generic;
using aspCore.WatchShop.DAOs;
using aspCore.WatchShop.EF;
using aspCore.WatchShop.Helper;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace aspCore.WatchShop.Models
{
    public class CartModel
    {
        const string cartKey = "cart";
        private PromotionDao _db = null;
        private IMapper _mapper = null;
        public CartModel(watchContext context, IMapper mapper)
        {
            _db = new PromotionDao(context);
            _mapper = mapper;
        }

        public int AddItem(ISession session, ProductVM product)
        {
            if (product == null) return 0;
            var cart = SessionHelper.GetData<List<ProductOrder>>(session, cartKey);
            if (cart != null)
            {
                int index = cart.FindIndex(item => item.ProductID == product.ID);
                if (index == -1) cart.Add(new ProductOrder()
                {
                    ProductID = product.ID,
                    ProductName = product.Name,
                    Quantity = 1,
                    Price = product.Price,
                    Promotion = product.Promotion,
                    Image = product.Image
                });
                else cart[index].Quantity++;

            }
            else
            {
                ProductOrder obj = _mapper.Map<ProductOrder>(product);
                obj.Quantity = 1;
                cart = new List<ProductOrder>() { obj };
            }
            SessionHelper.SetData(session, cartKey, cart);
            return 0;
        }

        public bool RemoveItem(ISession session, int productID)
        {
            var cart = SessionHelper.GetData<List<ProductOrder>>(session, cartKey);
            if (cart != null)
            {
                int index = cart.FindIndex(item => item.ProductID == productID);
                if (index == -1) return false;
                cart.RemoveAt(index);
                SessionHelper.SetData(session, cartKey, cart);
                return true;

            }
            return false;
        }

        public List<ProductOrder> GetCart(ISession session)
        {
            return SessionHelper.GetData<List<ProductOrder>>(session, cartKey);
        }

    }

}