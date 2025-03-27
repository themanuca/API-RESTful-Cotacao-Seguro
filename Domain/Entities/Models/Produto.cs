using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Models
{
    public class Produto
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Description { get; set; }
        public decimal BaseValue { get; set; }
        public decimal Limit { get; set; }
    }
}