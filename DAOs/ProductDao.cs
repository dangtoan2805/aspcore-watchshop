using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using aspCore.WatchShop.EF;
using aspCore.WatchShop.Entities;

namespace aspCore.WatchShop.DAOs
{
    public class ProductDao
    {
        private watchContext _db = null;

        public ProductDao(watchContext context)
        {
            _db = context;
        }

        #region REFERENCE ENTITY
        private IQueryable<Product> RefProductsForAdmin()
        {
            return _db.Products.Where(item => item.isDel == false);
        }

        private IQueryable<Product> RefProductsForView()
        {

            return _db.Products.Where(item => item.isDel == false && item.isShow == true);
        }

        #endregion

        #region CURD PRODUCT

        public bool isShow(int id)
        {
            return (bool)RefProductsForAdmin()
                .Where(item => item.ID == id)
                .Select(item => item.isShow)
                .FirstOrDefault();
        }

        public void AddProduct(Product product, ProductDetail detail)
        {
            Product obj = _db.Products.Where(p => p.ID == product.ID).FirstOrDefault();
            if (obj != null) return;
            _db.Products.Add(product);
            _db.SaveChanges();
            detail.ProductID = _db.Products.Select(p => p.ID).ToList().Max();
            _db.ProductDetails.Add(detail);
            _db.SaveChanges();
        }

        public void UpdateProduct(Product product, ProductDetail detail)
        {
            var obj = _db.Products.Where(p => p.ID == product.ID).FirstOrDefault();
            if (obj == null) return;
            ChangeProductData(obj, product);
            ProductDetail productDetail = _db.ProductDetails.Where(p => p.ProductID == obj.ID).FirstOrDefault();
            ChangeDetailData(productDetail, detail);
            _db.SaveChanges();
        }

        public void UpdateStatusProduct(int id, bool isShow)
        {
            Product p = _db.Products.Find(id);
            if (p != null)
                p.isShow = isShow;
            _db.SaveChanges();
        }

        public void DeleteProduct(int id)
        {
            Product p = _db.Products.Find(id);
            if (p != null)
            {
                p.isDel = true;
                var prod = _db.OrderDetails.Where(item => item.ProductID == p.ID).ToList();
                _db.OrderDetails.RemoveRange(prod);
            }
            _db.SaveChanges();
        }

        //***Helper Mapper Object to Database
        public void ChangeProductData(Product A, Product B)
        {
            A.Name = B.Name;
            A.isShow = B.isShow;
            A.Price = B.Price;
            A.Image = B.Image;
            A.CategoryID = B.CategoryID;
            A.TypeWireID = B.TypeWireID;
        }

        public void ChangeDetailData(ProductDetail A, ProductDetail B)
        {
            A.Images = B.Images;

            A.TypeGlass = B.TypeGlass;

            A.TypeBorder = B.TypeBorder;

            A.TypeMachine = B.TypeMachine;

            A.Parameter = B.Parameter;

            A.ResistWater = B.ResistWater;

            A.Warranty = B.Warranty;

            A.Brand = B.Brand;

            A.Origin = B.Origin;

            A.Color = B.Color;

            A.Func = B.Func;

            A.DescriptionProduct = B.DescriptionProduct;
        }
        //***
        #endregion

        #region GET
        public decimal GetProductDiscount(int productID)
        {
            return _db.PromotionDetails
                .Where(item => item.ProductID == productID)
                .Include(item => item.Promotion)
                .Select(item => item.Promotion.Discount)
                .FirstOrDefault();
        }

        public List<Category> GetCategories()
        {
            return _db.Categories.ToList();
        }

        public List<TypeWire> GetTypeWires()
        {
            return _db.TypeWires.ToList();
        }

        //Include PROMOTION DETAIL
        public List<Promotion> GetPromotionsAppling(DateTime time)
        {
            return _db.Promotions
                .Where(item => item.Status == true
                        && item.ToDate >= time && item.FromDate <= time)
                .Include(item => item.PromotionDetail)
                .ToList();
        }

        public List<Promotion> GetPromotions()
        {
            return _db.Promotions
                .Include(item => item.PromotionDetail)
                .ToList();
        }

        // =============== PRODUCT VIEW ================
        public List<Product> GetProductsByCate(int cateID)
        {
            return RefProductsForView()
                .Where(item => item.CategoryID == cateID)
                .Include(item => item.ProductDetail)
                .ToList();
        }

        // =============== PRODUCT ADMIN ================
        public List<Product> GetAllProduct()
        {
            return RefProductsForAdmin()
                .Include(item => item.ProductDetail)
                .ToList();
        }

        public Product FindProductByID(int id)
        {
            return RefProductsForView()
                .Where(item => item.ID == id)
                .Include(item => item.ProductDetail)
                .FirstOrDefault();
        }
        //Include PRODUCT DETAIL
        public Product GetProductDetail(int id)
        {
            return RefProductsForAdmin()
                .Where(item => item.ID == id)
                .Include(p => p.ProductDetail)
                .FirstOrDefault();
        }


        #endregion

    }
}
