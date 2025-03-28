using Domain.DTOs;
using Domain.Entities.Models;
using Domain.Interfaces;
using Infra.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CoberturaServices : ICoberturaService
    {
        private readonly AppDbContext _context;

        public CoberturaServices(AppDbContext context)
        {
            _context = context;
        }
        public async Task<CotacaoAcaoDTO> ExcluirAsync(int idCotacao, int idCobertura, int idParceiro)
        {
            var cobertura = await _context.CotacaoCoberturas
                 .FirstOrDefaultAsync(c => c.Id == idCobertura && c.IdCotacao == idCotacao && c.Cotacao.IdParceiro == idParceiro);
            if (cobertura == null)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Cobertura não encontrada ou não pertence à cotação do parceiro."
                };
            }

            _context.CotacaoCoberturas.Remove(cobertura);
            await _context.SaveChangesAsync();

            return new CotacaoAcaoDTO
            {
                Sucesso = true,
                Mensagem = "Cobertura excluída com sucesso."
            };
        }

        public async Task<CotacaoAcaoDTO> IncluirAsync(int idCotacao, CotacaoCobertura cobertura, int idParceiro)
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

            cobertura.IdCotacao = idCotacao;
            _context.CotacaoCoberturas.Add(cobertura);
            await _context.SaveChangesAsync();

            return new CotacaoAcaoDTO
            {
                Sucesso = true,
                Mensagem = "Cobertura incluída com sucesso."
            };
        }

        public async Task<List<CotacaoCobertura>> ListarAsync(int idCotacao, int idParceiro)
        {
            return await _context.CotacaoCoberturas
                 .Where(c => c.IdCotacao == idCotacao && c.Cotacao.IdParceiro == idParceiro)
                 .Include(c => c.Cobertura)
                 .ToListAsync();
        }
    }
}
