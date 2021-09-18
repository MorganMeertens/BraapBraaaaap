using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MotorbikeSpecs.Model
{
    public class Motorbike
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Make { get; set; } = null!;

        [Required]
        public string Model { get; set; } = null!;

        [Required]
        public string Category { get; set; } = null!;

        [Required]
        public string Year { get; set; } = null!;

        [Required]
        public string ImageURL { get; set; } = null!;

        [Required]
        public string YouTubeReviewLink { get; set; } = null!;

        [Required]
        public string YouTubeThumbnailURL { get; set; } = null!;

        [Required]
        public string EngineType { get; set; } = null!;

        [Required]
        public string Power { get; set; } = null!;

        [Required]
        public string Torque { get; set; } = null!;

        [Required]
        public string Displacement { get; set; } = null!;

        [Required]
        public string Compression { get; set; } = null!;

        [Required]
        public string BoreXStroke { get; set; } = null!;

        [Required]
        public string FuelConsumption { get; set; } = null!;

        [Required]
        public int CompanyId { get; set; }

        public Company Company { get; set; } = null!;

        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
