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
            var cobertura = await _context.CotacaoCobertura
            .FirstOrDefaultAsync(c => c.IdCobertura == idCobertura && c.IdCotacao == idCotacao);
            if (cobertura == null)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Cobertura não encontrada ou não pertence à cotação do parceiro."
                };
            }

            _context.CotacaoCobertura.Remove(cobertura);

            var cotacao = await _context.Cotacao
                .FirstOrDefaultAsync(c => c.Id == idCotacao);
            var produto = await _context.Produto.FindAsync(cotacao.IdProduto);

            var coberturaLista = await _context.CotacaoCobertura
           .Where(c => c.IdCotacao == idCotacao).ToListAsync();

            cotacao.Premio = CalcularPremio(produto, coberturaLista.Where(c => c.Id != idCobertura).ToList());

            await _context.SaveChangesAsync();

            return new CotacaoAcaoDTO
            {
                Sucesso = true,
                Mensagem = "Cobertura excluída com sucesso."
            };
        }

        public async Task<CotacaoAcaoDTO> IncluirAsync(int idCotacao, CotacaoCobertura cobertura, int idParceiro)
        {
            var cotacao = await _context.Cotacao
                        .FirstOrDefaultAsync(c => c.Id == idCotacao && c.IdParceiro == idParceiro);


            var coberturaLista = await _context.CotacaoCobertura
            .Where(c => c.IdCotacao == idCotacao).ToListAsync();

            if (cotacao == null)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Cotação não encontrada ou não pertence ao parceiro."
                };
            }

            if (coberturaLista.Any(c => c.IdCobertura == cobertura.IdCobertura))
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Não é permitido adicionar coberturas repetidas."
                };
            }

            var coberturaAux = await _context.Cobertura.FindAsync(cobertura.IdCobertura);
            if (coberturaAux == null)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Cobertura não encontrada."
                };
            }

            if (coberturaAux.Type == "Básica" && coberturaLista.Any(c => _context.Cobertura.Any(cob => cob.Id == c.IdCobertura && cob.Type == "Básica")))
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Apenas uma cobertura Básica é permitida."
                };
            }

            var faixaEtariaValida = ValidarFaixaEtaria(cotacao);
            if (!faixaEtariaValida.Sucesso)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = faixaEtariaValida.Mensagem
                };
            }

            var calculoDesconto = CalcularDescontoEAgravo(cobertura, faixaEtariaValida.FaixaIdade);
            if (!faixaEtariaValida.Sucesso)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = faixaEtariaValida.Mensagem
                };
            }

            cobertura.IdCotacao = idCotacao;
            _context.CotacaoCobertura.Add(cobertura);

            var produto = await _context.Produto.FindAsync(cotacao.IdProduto);
            coberturaLista.Add(cobertura);
            cotacao.Premio = CalcularPremio(produto, coberturaLista);

            await _context.SaveChangesAsync();

            return new CotacaoAcaoDTO
            {
                Sucesso = true,
                Mensagem = "Cobertura incluída com sucesso."
            };
        }

        public async Task<List<CotacaoCobertura>> ListarAsync(int idCotacao, int idParceiro)
        {
            return await _context.CotacaoCobertura
                 .Where(c => c.IdCotacao == idCotacao)
                 .ToListAsync();
        }
        private decimal CalcularPremio(Produto produto, List<CotacaoCobertura> coberturas)
        {
            return produto.BaseValue + coberturas.Sum(c => c.ValorTotal);
        }

        private CotacaoFaixaEtariaDTO ValidarFaixaEtaria(Cotacao cotacao)
        {
            var hoje = DateTime.Now;
            var idade = hoje.Year - cotacao.Nascimento.Year;
            if (cotacao.Nascimento.Date > hoje.AddYears(-idade))
            {
                idade--;
            }

            if (idade < 6 || idade > 65)
            {
                return new CotacaoFaixaEtariaDTO
                {
                    Sucesso = false,
                    Mensagem = "A idade do segurado deve estar entre 6 e 65 anos.",
                    Idade = idade,
                    FaixaIdade = null

                };

            }
            char[] eliminarString = ['A', 'a', 'n', 'o', 's'];
            var faixa = _context.FaixaIdade.ToList();

            var faixaIdade = faixa
                .FirstOrDefault(f =>
                    idade >= int.Parse(f.Description.Split(eliminarString)[0]) &&
                    idade <= int.Parse(f.Description.Split(eliminarString)[1]));

            if (faixaIdade == null)
            {

                return new CotacaoFaixaEtariaDTO
                {
                    Sucesso = false,
                    Mensagem = "Faixa etária não encontrada para a idade do segurado.",
                    Idade = idade,
                    FaixaIdade = null

                };
            }
            return new CotacaoFaixaEtariaDTO
            {
                Sucesso = true,
                Mensagem = "",
                Idade = idade,
                FaixaIdade = faixaIdade

            };
        }

        private CotacaoAcaoDTO CalcularDescontoEAgravo(CotacaoCobertura cobertura, FaixaIdade faixaIdade)
        {
            var coberturaAux = _context.Cobertura.FirstOrDefault(cob => cob.Id == cobertura.IdCobertura);
            if (coberturaAux == null)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = $"Cobertura com ID {cobertura.IdCobertura} não encontrada."
                };
            }

            if (coberturaAux.Type == "Básica")
            {
                if (faixaIdade.Desconto > 0)
                {
                    cobertura.ValorDesconto = coberturaAux.Value * (faixaIdade.Desconto / 100);
                    cobertura.ValorAgravo = null;
                    cobertura.ValorTotal = coberturaAux.Value - cobertura.ValorDesconto.Value;
                }
                else if (faixaIdade.Agravo > 0)
                {
                    cobertura.ValorAgravo = coberturaAux.Value * (faixaIdade.Agravo / 100);
                    cobertura.ValorDesconto = null;
                    cobertura.ValorTotal = coberturaAux.Value + cobertura.ValorAgravo.Value;
                }
                else
                {
                    cobertura.ValorDesconto = null;
                    cobertura.ValorAgravo = null;
                    cobertura.ValorTotal = coberturaAux.Value;
                }
            }
            else
            {
                cobertura.ValorDesconto = null;
                cobertura.ValorAgravo = null;
                cobertura.ValorTotal = coberturaAux.Value;
            }

            return new CotacaoAcaoDTO
            {
                Sucesso = true,
            };


        }
    }
}
