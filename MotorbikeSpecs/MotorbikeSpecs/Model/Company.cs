using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MotorbikeSpecs.Model
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string WebURL { get; set; }

        public string LogoURL { get; set; }

        public ICollection<Motorbike> Motorbikes { get; set; } = new List<Motorbike>();

    }
}
