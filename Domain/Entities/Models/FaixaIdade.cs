using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Models
{
    public class FaixaIdade
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(20)]
        public string Description { get; set; }
        public decimal Desconto { get; set; }
        public decimal Agravo { get; set; }
    }
}