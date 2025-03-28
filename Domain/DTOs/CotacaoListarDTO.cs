using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CotacaoListarDTO
    {
        public bool Sucesso { get; set; }
        public List<CotacaoItemDTO> Cotacoes { get; set; }
        public string Mensagem { get; set; }
    }
}
