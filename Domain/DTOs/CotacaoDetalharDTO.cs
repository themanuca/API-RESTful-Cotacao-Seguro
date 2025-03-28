using Domain.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CotacaoDetalharDTO
    {
        public bool Sucesso { get; set; }
        public Cotacao Cotacao { get; set; }
        public string Mensagem { get; set; }
    }
}
