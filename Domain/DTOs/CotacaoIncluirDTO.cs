using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CotacaoIncluirDTO
    {
        public bool Sucesso { get; set; }
        public int IdCotacao { get; set; }
        public string Mensagem { get; set; }
    }
}
