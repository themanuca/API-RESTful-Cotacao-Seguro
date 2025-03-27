using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Models
{
    public class Parceiro
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Description { get; set; }
        [Required, MaxLength(10)]
        public string Secret { get; set; }
    }
}