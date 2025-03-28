using Domain.DTOs;
using Domain.Entities.Models;
using Domain.Interfaces;
using Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class BeneficiarioServices : IBeneficiarioService
    {
        private readonly AppDbContext _context;

        public BeneficiarioServices(AppDbContext context)
        {
            _context = context;
        }
        public async Task<CotacaoAcaoDTO> AlterarAsync(int idCotacao, CotacaoBeneficiario beneficiario, int idParceiro)
        {
            var cotacao = await _context.Cotacoes
                            .FirstOrDefaultAsync(c => c.Id == idCotacao && c.IdParceiro == idParceiro);
            if (cotacao == null)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Cotação não encontrada ou não pertence ao parceiro."
                };
            }
            var tipoParentesco = await _context.TiposParentesco
                .FirstOrDefaultAsync(tp => tp.Id == beneficiario.IdParentesco);
            if (tipoParentesco == null)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Tipo de parentesco inválido."
                };
            }

            var existing = await _context.CotacaoBeneficiarios
                .FirstOrDefaultAsync(b => b.Id == beneficiario.Id && b.IdCotacao == idCotacao);

            if (existing == null)
            {
                beneficiario.IdCotacao = idCotacao;
                _context.CotacaoBeneficiarios.Add(beneficiario);
            }
            else
            {
                existing.Nome = beneficiario.Nome;
                existing.Percentual = beneficiario.Percentual;
                existing.IdParentesco = beneficiario.IdParentesco;
            }

            await _context.SaveChangesAsync();

            return new CotacaoAcaoDTO
            {
                Sucesso = true,
                Mensagem = existing == null ? "Beneficiário incluído com sucesso." : "Beneficiário atualizado com sucesso."
            };
        }

        public async Task<CotacaoBeneficiario> DetalharAsync(int idCotacao, int idBeneficiario, int idParceiro)
        {
            return await _context.CotacaoBeneficiarios
                            .Include(b => b.TipoParentesco)
                            .FirstOrDefaultAsync(b => b.Id == idBeneficiario && b.IdCotacao == idCotacao && b.Cotacao.IdParceiro == idParceiro);
        }

        public async Task<CotacaoAcaoDTO> ExcluirAsync(int idCotacao, int idBeneficiario, int idParceiro)
        {
            var beneficiario = await _context.CotacaoBeneficiarios
                            .FirstOrDefaultAsync(b => b.Id == idBeneficiario && b.IdCotacao == idCotacao && b.Cotacao.IdParceiro == idParceiro);
            if (beneficiario == null)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Beneficiário não encontrado ou não pertence à cotação do parceiro."
                };
            }

            _context.CotacaoBeneficiarios.Remove(beneficiario);
            await _context.SaveChangesAsync();

            return new CotacaoAcaoDTO
            {
                Sucesso = true,
                Mensagem = "Beneficiário excluído com sucesso."
            };
        }

        public async Task<List<CotacaoBeneficiario>> ListarAsync(int idCotacao, int idParceiro)
        {
            return await _context.CotacaoBeneficiarios
                            .Where(b => b.IdCotacao == idCotacao && b.Cotacao.IdParceiro == idParceiro)
                            .Include(b => b.TipoParentesco)
                            .ToListAsync();
        }
    }
}
