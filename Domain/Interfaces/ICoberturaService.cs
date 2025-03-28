using Domain.DTOs;
using Domain.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICoberturaService
    {
        Task<CotacaoAcaoDTO> IncluirAsync(int idCotacao, CotacaoCobertura cobertura, int idParceiro);
        Task<List<CotacaoCobertura>> ListarAsync(int idCotacao, int idParceiro);
        Task<CotacaoAcaoDTO> ExcluirAsync(int idCotacao, int idCobertura, int idParceiro);
    }
}
