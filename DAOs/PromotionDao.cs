using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using aspCore.WatchShop.EF;
using aspCore.WatchShop.Entities;
using Microsoft.EntityFrameworkCore;

namespace aspCore.WatchShop.DAOs
{
    public class PromotionDao
    {
        private watchContext _db = null;

        public PromotionDao(watchContext context)
        {
            _db = context;
        }

        public List<Promotion> GetPromotions()
        {
            return _db.Promotions.ToList();
        }

        public bool IsExsist(int promID)
        {
            var index = _db.Promotions.Where(item => item.ID == promID).FirstOrDefault();
            return index == null ? false : true;
        }

        public void AddPromotion(Promotion promotion, int[] product)
        {
            int promID = _db.Promotions.Max(item => item.ID) + 1;
            _db.Promotions.Add(promotion);
            foreach (var item in product)
            {
                _db.PromotionDetails.Add(new PromotionDetail()
                {
                    PromotionID = promID,
                    ProductID = item
                });
            }
            _db.SaveChanges();
        }

        public void RemovePromotion(int promID)
        {
            var prom = _db.Promotions.Where(item => item.ID == promID).FirstOrDefault();
            List<PromotionDetail> promDetail = _db.PromotionDetails.Where(item => item.PromotionID == promID).ToList();
            if (promDetail.Count == 0) return;
            _db.PromotionDetails.RemoveRange(promDetail);
            _db.SaveChanges();
        }

        public void EditPromotion(Promotion promotion, int[] product)
        {
            int promID = promotion.ID;
            var obj = _db.Promotions.Where(item => item.ID == promID).FirstOrDefault();
            obj.Name = promotion.Name;
            obj.Status = promotion.Status;
            obj.ToDate = promotion.ToDate;
            obj.FromDate = promotion.FromDate;
            obj.Discount = promotion.Discount;
            List<PromotionDetail> promDetail = _db.PromotionDetails.Where(item => item.PromotionID == promID).ToList();
            if (promDetail.Count == 0) return;
            _db.PromotionDetails.RemoveRange(promDetail);
            foreach (var item in product)
            {
                _db.PromotionDetails.Add(new PromotionDetail()
                {
                    PromotionID = promID,
                    ProductID = item
                });
            }
            _db.SaveChanges();
        }

        public void UpdateStatus(int promID, bool status)
        {
            var obj = _db.Promotions.Where(item => item.ID == promID).FirstOrDefault();
            obj.Status = status;
            _db.SaveChanges();
        }

        public Promotion FindPromotionByID(int promID)
        {
            return _db.Promotions
                .Where(item => item.ID == promID)
                .FirstOrDefault();
        }

        public int[] GetPromotionDetail(int promID)
        {
            return _db.PromotionDetails
                .Where(item => item.PromotionID == promID)
                .Select(item => item.ProductID)
                .ToArray();
        }
    }

}