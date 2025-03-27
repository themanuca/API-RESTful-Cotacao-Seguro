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
    public class CotacaoServices : ICotacaoService
    {
        private readonly AppDbContext _context;

        public CotacaoServices(AppDbContext context)
        {
            _context = context;
        }
        public Task<CotacaoAcaoDTO> AlterarAsync(int id, Cotacao cotacao, int idParceiro)
        {
            throw new NotImplementedException();
        }

        public async Task<CotacaoDetalharDTO> DetalharAsync(int id, int idParceiro)
        {
            var cotacao = await _context.Cotacoes
                .Include(c => c.Produto)
                .Include(c => c.Beneficiarios)
                .ThenInclude(b => b.TipoParentesco)
                .Include(c => c.Coberturas)
                .ThenInclude(cob => cob.Cobertura)
                .FirstOrDefaultAsync(c => c.Id == id && c.IdParceiro == idParceiro);

            if (cotacao == null)
            {   
                return (new CotacaoDetalharDTO 
                { 
                    Sucesso=false, 
                    Cotacao=null,
                    Mensagem = "Cotação não encontrada ou não pertence ao parceiro." 
                });
            }
            var cotacaoDetalhe = new CotacaoDetalharDTO
            {
                Sucesso = true,
                Cotacao = cotacao,
                Mensagem = "Detalhes da cotação obtidos com sucesso."
            };
            return (cotacaoDetalhe);
        }

        public Task<CotacaoAcaoDTO> ExcluirAsync(int id, int idParceiro)
        {
            throw new NotImplementedException();
        }

        public async Task<CotacaoIncluirDTO> IncluirAsync(Cotacao cotacao, int idParceiro)
        {
            var cotacaoIncluir = new CotacaoIncluirDTO();

            var parceiro = await _context.Parceiros.FindAsync(idParceiro);
            if (parceiro == null)
            {
                cotacaoIncluir.Sucesso = false;
                cotacaoIncluir.IdCotacao = 0;
                cotacaoIncluir.Mensagem = "Parceiro não encontrado.";

                return (cotacaoIncluir);
            }

            var produto = await _context.Produtos.FindAsync(cotacao.IdProduto);
            if (produto == null)
            {
                cotacaoIncluir.Sucesso = false;
                cotacaoIncluir.IdCotacao = 0;
                cotacaoIncluir.Mensagem = "Produto não encontrado.";

                return (cotacaoIncluir);
            }

            cotacao.IdParceiro = idParceiro;
            cotacao.DataCriacao = DateTime.Now;
            cotacao.DataAtualizacao = DateTime.Now;

            _context.Cotacoes.Add(cotacao);
            await _context.SaveChangesAsync();

            cotacaoIncluir.Sucesso = true;
            cotacaoIncluir.IdCotacao = cotacao.Id;
            cotacaoIncluir.Mensagem = "Cotação criada com sucesso.";

            return (cotacaoIncluir);
        }

        public Task<CotacaoListarDTO> ListarAsync(int idParceiro)
        {
            throw new NotImplementedException();
        }
    }
}
