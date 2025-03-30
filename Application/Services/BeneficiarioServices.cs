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
            var cotacao = await _context.Cotacao
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

            var existing = await _context.CotacaoBeneficiario
                .FirstOrDefaultAsync(b => b.Id == beneficiario.Id && b.CotacaoId == idCotacao);

            if (existing == null)
            {
                beneficiario.CotacaoId = idCotacao;
                _context.CotacaoBeneficiario.Add(beneficiario);
            }
            else
            {
                existing.Nome = beneficiario.Nome;
                existing.Percentual = beneficiario.Percentual;
                existing.IdParentesco = beneficiario.IdParentesco;
            }

            if (cotacao.Beneficiario != null && cotacao.Beneficiario.Any())
            {
                var somaPercentual = cotacao.Beneficiario.Sum(b => b.Percentual);
                if (somaPercentual != 100)
                {
                  return  new CotacaoAcaoDTO
                    {
                        Sucesso = false,
                        Mensagem = "A soma dos porcentuais dos beneficiários deve ser 100."
                    };
                }

                cotacao.Beneficiario = cotacao.Beneficiario.OrderBy(b => b.IdParentesco).ToList();
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
            if(idBeneficiario <= 0 || idCotacao <= 0)
            {
                throw new Exception("Dados ausentes.");
            }
            return await _context.CotacaoBeneficiario
                            .FirstOrDefaultAsync(b => b.Id == idBeneficiario && b.CotacaoId == idCotacao);
        }

        public async Task<CotacaoAcaoDTO> ExcluirAsync(int idCotacao, int idBeneficiario, int idParceiro)
        {
            var beneficiario = await _context.CotacaoBeneficiario
                            .FirstOrDefaultAsync(b => b.Id == idBeneficiario && b.CotacaoId == idCotacao);
            if (beneficiario == null)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Beneficiário não encontrado ou não pertence à cotação do parceiro."
                };
            }

            _context.CotacaoBeneficiario.Remove(beneficiario);
            await _context.SaveChangesAsync();

            return new CotacaoAcaoDTO
            {
                Sucesso = true,
                Mensagem = "Beneficiário excluído com sucesso."
            };
        }

        public async Task<List<CotacaoBeneficiario>> ListarAsync(int idCotacao, int idParceiro)
        {
            return await _context.CotacaoBeneficiario
                            .Where(b => b.CotacaoId == idCotacao)
                            .ToListAsync();
        }
    }
}
