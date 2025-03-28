using Domain.DTOs;
using Domain.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICotacaoService
    {
        Task<CotacaoIncluirDTO> IncluirAsync(Cotacao cotacao, int idParceiro);
        Task<CotacaoAcaoDTO> AlterarAsync(int id, Cotacao cotacao, int idParceiro);
        Task<CotacaoListarDTO> ListarAsync(int idParceiro);
        Task<CotacaoDetalharDTO> DetalharAsync(int id, int idParceiro);
        Task<CotacaoAcaoDTO> ExcluirAsync(int id, int idParceiro);
    }
}
