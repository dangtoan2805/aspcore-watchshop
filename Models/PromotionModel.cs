using System.Linq;
using System.Collections.Generic;
using System;
using aspCore.WatchShop.DAOs;
using aspCore.WatchShop.EF;
using AutoMapper;
using aspCore.WatchShop.Entities;

namespace aspCore.WatchShop.Models
{
    public class PromotionModel
    {
        private PromotionDao _db = null;
        private IMapper _mapper = null;
        public PromotionModel(watchContext context, IMapper mapper)
        {
            _db = new PromotionDao(context);
            _mapper = mapper;
        }

        // =============== PRIVATE ================
        private void EditPromotion(PromotionVM promotion, int[] products)
        {
            if (promotion == null && products.Length == 0) return;
            _db.AddPromotion(_mapper.Map<Promotion>(promotion), products);
        }

        private void AddPromotion(PromotionVM promotion, int[] products)
        {
            if (promotion == null && products.Length == 0) return;
            _db.AddPromotion(_mapper.Map<Promotion>(promotion), products);
        }

        // =============== Puble ================
        public List<PromotionVM> GetPromotions()
        {
            var rawData = _db.GetPromotions();
            if (rawData.Count == 0) return null;
            List<PromotionVM> promotions = new List<PromotionVM>();
            rawData.ForEach(item => promotions.Add(_mapper.Map<PromotionVM>(item)));
            return promotions;
        }

        public void Submit(PromotionVM promotion, int[] product)
        {
            if (_db.IsExsist(promotion.ID))
                EditPromotion(promotion, product);
            else
                AddPromotion(promotion, product);
        }

        public PromotionVM GetDetail(int promID)
        {
            if (promID == 0) return null;
            var obj = _db.FindPromotionByID(promID);
            if (obj == null) return null;
            return _mapper.Map<PromotionVM>(obj);
        }

        public int[] GetProductPromotion(int promID)
        {
            if (promID == 0) return null;
            return _db.GetPromotionDetail(promID);
        }

        public void DeletePromotion(int promID)
        {
            if (promID == 0) return;
            _db.RemovePromotion(promID);
        }

        public void UpdateStatus(int promID, bool status)
        {
            if (promID == 0) return;
            _db.UpdateStatus(promID, status);
        }
    }
    public class PromotionVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool Status { get; set; }
        public decimal Discount { get; set; }
    }

}