using Domain.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CotacaoFaixaEtariaDTO
    {
       public bool Sucesso { get; set; } 
       public string Mensagem { get; set; }
       public int Idade { get; set; }
       public FaixaIdade FaixaIdade { get; set; }
    }
}
