﻿using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models.DTO
{
    public class PromotionDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
