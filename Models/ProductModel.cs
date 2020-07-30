using System.Linq;
using System;
using System.Collections.Generic;
using aspCore.WatchShop.DAOs;
using aspCore.WatchShop.EF;
using aspCore.WatchShop.Entities;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace aspCore.WatchShop.Models
{
    public class ProductModel
    {
        const string productKey = "products";
        const string cateKey = "cates";
        const string wireKey = "wires";
        private bool _isModified = true;
        private IMemoryCache _cache = null;
        private ProductDao _db = null;
        private IMapper _mapper = null;

        public ProductModel(watchContext context, IMapper mapper, IMemoryCache cache)
        {
            _db = new ProductDao(context);
            _mapper = mapper;
            _cache = cache;
        }

        #region Category, Wires
        // Get List Wire in Db
        public List<TypeWireVM> GetTypeWires()
        {
            List<TypeWireVM> wires;
            if (!_cache.TryGetValue(cateKey, out wires))
            {
                var rawData = _db.GetTypeWires();
                if (rawData.Count <= 0) return new List<TypeWireVM>();
                wires = new List<TypeWireVM>();
                rawData.ForEach(item => wires.Add(_mapper.Map<TypeWireVM>(item)));
                _cache.Set(cateKey, wires, DateTime.MaxValue);
            }
            return wires;
        }
        // Get List Cateogry in Db
        public List<CategoryVM> GetCategories()
        {
            List<CategoryVM> cates;
            if (!_cache.TryGetValue(cateKey, out cates))
            {
                var rawData = _db.GetCategories();
                if (rawData.Count <= 0) return new List<CategoryVM>();
                cates = new List<CategoryVM>();
                rawData.ForEach(item => cates.Add(_mapper.Map<CategoryVM>(item)));
                _cache.Set(cateKey, cates, DateTime.MaxValue);
            }
            return cates;
        }

        #endregion

        #region  Products
        // =============== PRIVATE ================
        //Conver  product domain to productVM with determind promotion product's
        private List<ProductVM> ConvertListProduct(List<Product> rawData)
        {
            if (rawData.Count <= 0) return null;
            List<ProductVM> products = new List<ProductVM>();
            List<ProductPromotion> proms = ListProductsPromotion();
            rawData.ForEach(item =>
            {
                ProductVM obj = _mapper.Map<ProductVM>(item);
                int index = proms.FindIndex(prom => prom.ProductID == obj.ID);
                if (index != -1) obj.Promotion = proms[index].Discount;
                products.Add(obj);
            });
            return products;
        }

        //If the product belongs to more than 2 promotions, the promotion applies later.
        private List<ProductPromotion> ListProductsPromotion()
        {
            List<Promotion> rawData = _db.GetPromotionsAppling(DateTime.Now);
            List<ProductPromotion> products = new List<ProductPromotion>();
            if (rawData.Count <= 0) return products;
            foreach (var item in rawData)
            {
                item.PromotionDetail.ForEach(pDetail =>
                {
                    if (_db.isShow(pDetail.ProductID))
                    {
                        int index = products.FindIndex(p => p.ProductID == pDetail.ProductID);
                        if (index == -1) products.Add(new ProductPromotion() { ProductID = pDetail.ProductID, Discount = item.Discount });
                        else products[index].Discount = item.Discount;
                    }
                });
            }
            return products;
        }

        //Get All product
        public List<ProductVM> GetAllProduct()
        {
            List<ProductVM> products;
            if (_isModified == true)
            {
                products = ConvertListProduct(_db.GetAllProduct());
                _isModified = false;
            }
            else
            {
                if (!_cache.TryGetValue(productKey, out products))
                    products = ConvertListProduct(_db.GetAllProduct());
            }
            if (products == null) return new List<ProductVM>();
            _cache.Set(productKey, products, DateTime.MaxValue);
            return products;
        }

        // =============== Public ================
        public List<ProductVM> GetProducts()
        {
            var products = _db.GetAllProduct();
            if (products.Count == 0) return new List<ProductVM>();
            List<ProductVM> productVMs = new List<ProductVM>();
            products.ForEach(item => productVMs.Add(_mapper.Map<ProductVM>(item)));
            return productVMs;
        }

        public List<ProductVM> GetProductPromotion()
        {
            List<ProductPromotion> proms = ListProductsPromotion();
            if (proms.Count <= 0)
                return GetAllProduct()
                        .OrderBy(item => item.SaleCount)
                        .ToList();
            List<ProductVM> products;
            if (!_cache.TryGetValue(productKey, out products))
            {
                products = new List<ProductVM>();
                proms.ForEach(item =>
                {
                    var data = _db.FindProductByID(item.ProductID);
                    ProductVM obj = _mapper.Map<ProductVM>(data);
                    obj.Promotion = item.Discount;
                    products.Add(obj);
                });
            }
            else
                products = products.Where(p => proms.Exists(prom => prom.ProductID == p.ID)).ToList();
            return products;
        }

        public List<ProductVM> GetProductByCate(int cateID)
        {
            List<ProductVM> products = null;
            if (!_cache.TryGetValue(productKey, out products))
            {
                products = ConvertListProduct(_db.GetProductsByCate(cateID));
                if (products == null) return new List<ProductVM>();
                _cache.Set(cateKey, products, DateTime.MaxValue);
            }
            return products
                .Where(item => item.CategoryID == cateID)
                .ToList();
        }

        public List<ProductVM> GetProductRelation(string key)
        {
            return GetAllProduct().Where(item => item.Name.Contains(key)).Take(4).ToList();
        }

        public List<ProductVM> TakeProductPromotion(int items)
        {
            List<ProductPromotion> proms = ListProductsPromotion();
            if (proms.Count <= 0)
                return GetAllProduct()
                        .OrderBy(item => item.SaleCount)
                        .Take(items)
                        .ToList();
            proms = proms.OrderBy(item => item.Discount).Take(items).ToList();
            List<ProductVM> products;
            if (!_cache.TryGetValue(productKey, out products))
            {
                products = new List<ProductVM>();
                proms.ForEach(item =>
                {
                    var data = _db.FindProductByID(item.ProductID);
                    ProductVM obj = _mapper.Map<ProductVM>(data);
                    obj.Promotion = item.Discount;
                    products.Add(obj);
                });
            }
            else
                products = products.Where(p => proms.Exists(prom => prom.ProductID == p.ID)).ToList();
            return products;
        }

        public List<ProductVM> TakeProducByCate(int items, int cateID)
        {
            var data = GetProductByCate(cateID);
            if (data == null) return CreateEmptyProduct(items);
            return data.Take(items).ToList();
        }

        public List<ProductVM> FindWithKey(string key)
        {
            List<ProductVM> products = new List<ProductVM>();
            if (key != null)
                products = GetAllProduct().Where(item => item.Name.Contains(key)).ToList();
            return products;
        }

        public ProductVM FindByID(int id)
        {
            return GetAllProduct().Where(item => item.ID == id).FirstOrDefault();
        }

        // =============== CURD PRODUCT ================
        public ProductDetailVM GetProductDetail(int id)
        {
            var obj = _mapper.Map<ProductDetailVM>(_db.GetProductDetail(id));
            if (obj == null) return new ProductDetailVM();
            var disount = _db.GetProductDiscount(id);
            obj.Promotion = disount;
            return obj;
        }

        public void AddProduct(ProductDetailVM p)
        {
            _isModified = true;
            _db.AddProduct(_mapper.Map<Product>(p), _mapper.Map<ProductDetail>(p));
        }

        public void UpdateProduct(ProductDetailVM p)
        {
            _isModified = true;
            _db.UpdateProduct(_mapper.Map<Product>(p), _mapper.Map<ProductDetail>(p));
        }

        public void UpdateStatusProduct(int id, int indexStatsu)
        {
            _isModified = true;
            _db.UpdateStatusProduct(id, indexStatsu == 0 ? false : true);
        }

        public void DeleteProduct(int id)
        {
            _isModified = true;
            _db.DeleteProduct(id);
        }

        public List<ProductVM> CreateEmptyProduct(int number)
        {
            List<ProductVM> ls = new List<ProductVM>();
            for (int i = 0; i < number; i++)
            {
                ls.Add(new ProductVM() { Image = "no-img.jpg", Price = 0 });
            }
            return ls;
        }

        #endregion
    }

    public class ProductPromotion
    {
        public int ProductID { get; set; }
        public decimal Discount { get; set; }
    }

    public class TypeWireVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class CategoryVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class ProductVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int CategoryID { get; set; }
        public int TypeWireID { get; set; }
        public string Image { get; set; }
        public int SaleCount { get; set; }
        public decimal Promotion { get; set; }
        public bool Show { get; set; }
    }

    public class ProductDetailVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int CategoryID { get; set; }
        public int TypeWireID { get; set; }
        public string Image { get; set; }
        public int SaleCount { get; set; }
        public decimal Promotion { get; set; }
        public bool isShow { get; set; }

        //Detail
        public string Images { get; set; }
        public string TypeGlass { get; set; }
        public string TypeBorder { get; set; }
        public string TypeMachine { get; set; }
        public string Parameter { get; set; }
        public string ResistWater { get; set; }
        public string Warranty { get; set; }
        public string Brand { get; set; }
        public string Origin { get; set; }
        public string Color { get; set; }
        public string Func { get; set; }
        public string DescriptionProduct { get; set; }

        public ProductDetailVM()
        {
            this.Image = "no-img.jpg";
            this.isShow = true;
        }
    }
}