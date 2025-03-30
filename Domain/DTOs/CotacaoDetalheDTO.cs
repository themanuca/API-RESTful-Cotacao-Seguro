using Domain.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CotacaoDetalheDTO
    {
        public int Id { get; set; }
        public int IdProduto { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public string NomeSegurado { get; set; }
        public int? DDD { get; set; }
        public int? Telefone { get; set; }
        public string Endereco { get; set; }
        public string CEP { get; set; }
        public string Documento { get; set; }
        public DateTime Nascimento { get; set; }
        public decimal Premio { get; set; }
        public decimal ImportanciaSegurada { get; set; }
        public Produto Produto { get; set; }
        public List<CotacaoBeneficiario> Beneficiarios { get; set; }
        public List<CotacaoCobertura> Coberturas { get; set; }
    }
}
