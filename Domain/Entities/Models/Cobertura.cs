using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Models
{ 
    public class Cobertura
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Description { get; set; }
        [Required, MaxLength(20)]
        public string Type { get; set; }
        public decimal Value { get; set; }
    }
}