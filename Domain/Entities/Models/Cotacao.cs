using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Models
{
    public class Cotacao
    {
        
        [Key]
        public int Id { get; set; }
        public int IdProduto { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public int IdParceiro { get; set; }
        [Required, MaxLength(100)]
        public string NomeSegurado { get; set; }
        public int? DDD { get; set; }
        public int? Telefone { get; set; }
        [Required, MaxLength(255)]
        public string Endereco { get; set; }
        [Required, MaxLength(8)]
        public string CEP { get; set; }
        [Required, MaxLength(20)]
        public string Documento { get; set; }
        public DateTime Nascimento { get; set; }
        public decimal Premio { get; set; }
        public decimal ImportanciaSegurada { get; set; }

        // Relacionamentos
        public Produto Produto { get; set; }
        public Parceiro Parceiro { get; set; }
        public List<CotacaoBeneficiario> Beneficiarios { get; set; }
        public List<CotacaoCobertura> Coberturas { get; set; }
    
    }
}