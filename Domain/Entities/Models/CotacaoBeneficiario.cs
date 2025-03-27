using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Models
{
    public class CotacaoBeneficiario
    {
        [Key]
        public int Id { get; set; }
        public int IdCotacao { get; set; }
        [Required, MaxLength(100)]
        public string Nome { get; set; }
        public decimal Percentual { get; set; }
        public int IdParentesco { get; set; }

        // Relacionamentos
        public Cotacao Cotacao { get; set; }
        public TipoParentesco TipoParentesco { get; set; }
    }
}