using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs
{
    public class CotacaoItemDTO
    {
        public int Id { get; set; }
        public string NomeSegurado { get; set; }
        public string Documento { get; set; }
        public string NomeProduto { get; set; }
    }
}
