using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Models
{
    public class TipoParentesco
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(20)]
        public string Description { get; set; }
    }
}
