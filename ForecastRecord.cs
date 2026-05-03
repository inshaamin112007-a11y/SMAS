using System;
using System.ComponentModel.DataAnnotations;

namespace SMAS.API.Models
{
    public class ForecastRecord : Entity
    {
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }

        public DateTime ForecastDate { get; set; }

        public int PredictedDemand { get; set; }

        public double TrendScore { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}