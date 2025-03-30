using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Models
{
    public class CotacaoCobertura
    {
        [Key]
        public int Id { get; set; }
        public int IdCotacao { get; set; }
        public int IdCobertura { get; set; }
        public decimal? ValorDesconto { get; set; }
        public decimal? ValorAgravo { get; set; }
        public decimal ValorTotal { get; set; }
    }
}