using Domain.DTOs;
using Domain.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IBeneficiarioService
    {
        Task<CotacaoAcaoDTO> AlterarAsync(int idCotacao, CotacaoBeneficiario beneficiarios, int idParceiro);
        Task<List<CotacaoBeneficiario>> ListarAsync(int idCotacao, int idParceiro);
        Task<CotacaoBeneficiario> DetalharAsync(int idCotacao, int idBeneficiario, int idParceiro);
        Task<CotacaoAcaoDTO> ExcluirAsync(int idCotacao, int idBeneficiario, int idParceiro);

    }
}
