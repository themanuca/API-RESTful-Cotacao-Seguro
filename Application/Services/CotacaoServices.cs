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
        private List<CotacaoItemDTO> _listaCotacoDTO = [];
        private readonly IBeneficiarioService _beneficiarioService;
        private readonly ICoberturaService _coberturaService;
        public CotacaoServices(AppDbContext context, IBeneficiarioService beneficiarioService , ICoberturaService coberturaService)
        {
            _context = context;
            _beneficiarioService = beneficiarioService;
            _coberturaService = coberturaService;
        }
        public async Task<CotacaoAcaoDTO> AlterarAsync(int id, CotacaoDetalheDTO cotacao, int idParceiro)
        {
            var existe = await _context.Cotacao
                .FirstOrDefaultAsync(c => c.Id == cotacao.Id && c.IdParceiro == idParceiro);

            var beneficiarioList = await _context.CotacaoBeneficiario.Where(b => b.IdCotacao == cotacao.Id).ToListAsync();
            var cotacaoCobertura = await _context.CotacaoCobertura.Where(b => b.IdCotacao == cotacao.Id).ToListAsync();

            if (existe == null)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Cotação não encontrada ou não pertence ao parceiro."
                };
            }
            var foneValido = ValidarDDDeTelefone(cotacao);
            if (!foneValido.Sucesso)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = foneValido.Mensagem
                };
            }

            var faixaEtariaValido = ValidarFaixaEtaria(cotacao);
            if (!faixaEtariaValido.Sucesso)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = faixaEtariaValido.Mensagem
                };
            }

            if (cotacaoCobertura != null && cotacaoCobertura.Any())
            {
                var coberturaValida = ValidarCoberturas(cotacaoCobertura);
                if (!coberturaValida.Sucesso)
                {
                    return new CotacaoAcaoDTO
                    {
                        Sucesso = false,
                        Mensagem = coberturaValida.Mensagem
                    };
                }

                var calculoDesconto = CalcularDescontoEAgravo(cotacaoCobertura, faixaEtariaValido.FaixaIdade);
                if (!calculoDesconto.Sucesso)
                {
                    return new CotacaoAcaoDTO
                    {
                        Sucesso = false,
                        Mensagem = calculoDesconto.Mensagem
                    };
                }
            }

            if (beneficiarioList != null && beneficiarioList.Any())
            {
                var beneficiarioValido = ValidarBeneficiarios(beneficiarioList);
                if (!beneficiarioValido.Sucesso)
                {
                    return new CotacaoAcaoDTO
                    {
                        Sucesso = false,
                        Mensagem = beneficiarioValido.Mensagem
                    };
                }
            }
            var produto = await _context.Produto.FindAsync(cotacao.IdProduto);
            if (produto == null)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Produto não encontrado."
                };
            }

            var importanciaSegura = ValidarImportanciaSegurada(existe, produto);
            if (!importanciaSegura.Sucesso)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = importanciaSegura.Mensagem
                };
            }

            existe.IdProduto = cotacao.IdProduto;
            existe.NomeSegurado = cotacao.NomeSegurado;
            existe.DDD = cotacao.DDD;
            existe.Telefone = cotacao.Telefone;
            existe.Endereco = cotacao.Endereco;
            existe.CEP = cotacao.CEP;
            existe.Documento = cotacao.Documento;
            existe.Nascimento = cotacao.Nascimento;
            existe.ImportanciaSegurada = cotacao.ImportanciaSegurada;

            if (cotacaoCobertura != null && cotacaoCobertura.Any())
            {
                existe.Premio = CalcularPremio(produto, cotacaoCobertura);
            }

            existe.DataAtualizacao = DateTime.Now;

            await _context.SaveChangesAsync();

            return new CotacaoAcaoDTO
            {
                Sucesso = true,
                Mensagem = "Cotação atualizada com sucesso."
            };
        }

        public async Task<CotacaoDetalharDTO> DetalharAsync(int id, int idParceiro)
        {
            var cotacao = await _context.Cotacao.FirstOrDefaultAsync(c => c.Id == id && c.IdParceiro == idParceiro);


            if (cotacao == null)
            {   
                return (new CotacaoDetalharDTO 
                { 
                    Sucesso=false, 
                    Cotacao=null,
                    Mensagem = "Cotação não encontrada ou não pertence ao parceiro." 
                });
            }

            var beneficiarioList = await _context.CotacaoBeneficiario.Where(b => b.IdCotacao == cotacao.Id).ToListAsync();
            var cotacaoCobertura = await _context.CotacaoCobertura.Where(b => b.IdCotacao == cotacao.Id).ToListAsync();

            var cotacaoDetalhe = new CotacaoDetalharDTO
            {
                Sucesso = true,
                Cotacao = new CotacaoDetalheDTO
                {
                    Id = cotacao.Id,
                    IdProduto = cotacao.IdProduto,
                    DataCriacao = cotacao.DataCriacao,
                    DataAtualizacao = cotacao.DataAtualizacao,
                    NomeSegurado = cotacao.NomeSegurado,
                    DDD = cotacao.DDD,
                    Telefone = cotacao.Telefone,
                    Endereco = cotacao.Endereco,
                    CEP = cotacao.CEP,
                    Documento = cotacao.Documento,
                    Nascimento = cotacao.Nascimento,
                    Premio = cotacao.Premio,
                    ImportanciaSegurada = cotacao.ImportanciaSegurada,
                    Beneficiarios = beneficiarioList,
                    Coberturas = cotacaoCobertura
                },
                Mensagem = "Detalhes da cotação obtidos com sucesso."
            };
            return (cotacaoDetalhe);
        }

        public async Task<CotacaoAcaoDTO> ExcluirAsync(int id, int idParceiro)
        {
            var cotacao = await _context.Cotacao
                    .FirstOrDefaultAsync(c => c.Id == id && c.IdParceiro == idParceiro);

            if (cotacao == null)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Cotação não encontrada ou não pertence ao parceiro."
                };
            }

            _context.Cotacao.Remove(cotacao);
            await _context.SaveChangesAsync();

            return new CotacaoAcaoDTO
            {
                Sucesso = true,
                Mensagem = "Cotação excluída com sucesso."
            };
        }

        public async Task<CotacaoIncluirDTO> IncluirAsync(CotacaoDetalheDTO cotacao, int idParceiro)
        {

            var cotacaoIncluir = new CotacaoIncluirDTO();

            var foneValido = ValidarDDDeTelefone(cotacao);
            if (!foneValido.Sucesso)
            {
                cotacaoIncluir.Sucesso = false;
                cotacaoIncluir.IdCotacao = 0;
                cotacaoIncluir.Mensagem = foneValido.Mensagem;
                return cotacaoIncluir;
            }

            var dadosFaixaEtaria = ValidarFaixaEtaria(cotacao);
            if (!dadosFaixaEtaria.Sucesso)
            {
                cotacaoIncluir.Sucesso = false;
                cotacaoIncluir.IdCotacao = 0;
                cotacaoIncluir.Mensagem = dadosFaixaEtaria.Mensagem;
                return cotacaoIncluir;
            }

            var coberturaValida = ValidarCoberturas(cotacao.Coberturas);
            if (!coberturaValida.Sucesso)
            {
                cotacaoIncluir.Sucesso = false;
                cotacaoIncluir.IdCotacao = 0;
                cotacaoIncluir.Mensagem = coberturaValida.Mensagem;
                return cotacaoIncluir;
            }

            var calculoDensconto = CalcularDescontoEAgravo(cotacao.Coberturas, dadosFaixaEtaria.FaixaIdade);
            if (!calculoDensconto.Sucesso)
            {
                cotacaoIncluir.Sucesso = false;
                cotacaoIncluir.IdCotacao = 0;
                cotacaoIncluir.Mensagem = calculoDensconto.Mensagem;
                return cotacaoIncluir;
            }

            var beneficiarioValido = ValidarBeneficiarios(cotacao.Beneficiarios);
            if (!beneficiarioValido.Sucesso)
            {
                cotacaoIncluir.Sucesso = false;
                cotacaoIncluir.IdCotacao = 0;
                cotacaoIncluir.Mensagem = beneficiarioValido.Mensagem;
                return cotacaoIncluir;
            }


            var parceiro = await _context.Parceiro.FindAsync(idParceiro);
            if (parceiro == null)
            {
                cotacaoIncluir.Sucesso = false;
                cotacaoIncluir.IdCotacao = 0;
                cotacaoIncluir.Mensagem = "Parceiro não encontrado.";

                return (cotacaoIncluir);
            }

            var produto = await _context.Produto.FindAsync(cotacao.IdProduto);
            if (produto == null)
            {
                cotacaoIncluir.Sucesso = false;
                cotacaoIncluir.IdCotacao = 0;
                cotacaoIncluir.Mensagem = "Produto não encontrado.";

                return (cotacaoIncluir);
            }

            //var importanciasegura = ValidarImportanciaSegurada(cotacao, produto);
            //if (!importanciasegura.sucesso)
            //{
            //    cotacaoincluir.sucesso = false;
            //    cotacaoincluir.idcotacao = 0;
            //    cotacaoincluir.mensagem = importanciasegura.mensagem;
            //    return cotacaoincluir;
            //}

            //ordenarbeneficiarios(cotacao);

            cotacao.Premio = CalcularPremio(produto, cotacao.Coberturas);
            cotacao.IdParceiro = idParceiro;
            cotacao.DataCriacao = DateTime.Now;
            cotacao.DataAtualizacao = DateTime.Now;

            var cotacaoSalvar = new Cotacao
            {
                IdProduto = produto.Id,
                DataCriacao = DateTime.Now,
                IdParceiro = idParceiro,
                NomeSegurado = cotacao.NomeSegurado,
                DDD = cotacao.DDD,
                Telefone = cotacao.Telefone,
                Endereco = cotacao.Endereco,
                CEP = cotacao.CEP,
                Documento = cotacao.Documento,
                Nascimento = cotacao.Nascimento,
                Premio = cotacao.Premio,
                ImportanciaSegurada = cotacao.ImportanciaSegurada
            };

            _context.Cotacao.Add(cotacaoSalvar);
            await _context.SaveChangesAsync();
            var idCotacao = cotacaoSalvar.Id;
            if (idCotacao > 0)
            {
                if(cotacao.Beneficiarios.Count > 0)
                {
                    foreach (var beneficio in cotacao.Beneficiarios)
                    {
                        await _beneficiarioService.AlterarAsync(idCotacao, beneficio, idParceiro);
                    }

                } 
                 if(cotacao.Coberturas.Count > 0)
                {
                    foreach(var cobertura in cotacao.Coberturas)
                    {
                       await _coberturaService.IncluirAsync(idCotacao, cobertura, idParceiro);
                    }
                }
            }
            cotacaoIncluir.Sucesso = true;
            cotacaoIncluir.IdCotacao = idCotacao;
            cotacaoIncluir.Mensagem = "Cotação criada com sucesso.";

            return (cotacaoIncluir);
        }

        public async Task<CotacaoListarDTO> ListarAsync(int idParceiro)
        {

            var cotacoes = await _context.Cotacao
                 .Where(c => c.IdParceiro == idParceiro).ToListAsync();

            foreach(var cotacao in cotacoes)
            {
                if(cotacao.IdProduto > 0)
                {
                    var produto = await _context.Produto.FirstOrDefaultAsync(c => c.Id == cotacao.IdProduto);
                    if (produto?.Description.Length > 0)
                    {
                        _listaCotacoDTO.Add(new CotacaoItemDTO
                        {
                            Id = cotacao.Id,
                            NomeSegurado = cotacao.NomeSegurado,
                            Documento = cotacao.Documento,
                            NomeProduto = produto.Description
                        });
                    };
                }
            };

            if (_listaCotacoDTO.Count <= 0)
            {
                return new CotacaoListarDTO
                {
                    Sucesso = false,
                    Cotacoes = _listaCotacoDTO,
                    Mensagem = "Sem cotações para esse parceiro."
                };

            }

            return new CotacaoListarDTO
            {
                Sucesso = true,
                Cotacoes = _listaCotacoDTO,
                Mensagem = "Cotações listadas com sucesso."
            };
        }

        private CotacaoAcaoDTO ValidarDDDeTelefone(CotacaoDetalheDTO cotacao)
        {
            if (cotacao.DDD.HasValue && !cotacao.Telefone.HasValue)
            {
                return new CotacaoAcaoDTO {
                    Sucesso = false,
                    Mensagem = "Telefone é obrigatório quando DDD é informado."
                };
            }
            if (!cotacao.DDD.HasValue && cotacao.Telefone.HasValue)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "DDD é obrigatório quando Telefone é informado."
                };
            }
            return new CotacaoAcaoDTO
            {
                Sucesso = true,
                Mensagem = ""
            };
        }

        private CotacaoFaixaEtariaDTO ValidarFaixaEtaria(CotacaoDetalheDTO cotacao)
        {
            var hoje = DateTime.Now;
            var idade = hoje.Year - cotacao.Nascimento.Year;
            if (cotacao.Nascimento.Date > hoje.AddYears(-idade)) idade--;

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
            char[] eliminarString = ['A','a','n','o','s'];
            var faixa = _context.FaixaIdade.ToList();

            var faixaIdade =faixa
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

        private CotacaoAcaoDTO ValidarCoberturas(List<CotacaoCobertura> coberturas)
        {
            if (coberturas == null || !coberturas.Any())
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Pelo menos uma cobertura Básica e uma Adicional são obrigatórias."
                };
            }

            var coberturasBasicas = coberturas
                .Where(c => _context.Cobertura.Any(cob => cob.Id == c.IdCobertura && cob.Type == "Básica"))
                .ToList();
            var coberturasAdicionais = coberturas
                .Where(c => _context.Cobertura.Any(cob => cob.Id == c.IdCobertura && cob.Type == "Adicional"))
                .ToList();

            if (coberturasBasicas.Count == 0 || coberturasAdicionais.Count == 0)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Pelo menos uma cobertura Básica e uma Adicional são obrigatórias."
                };
            }

            if (coberturasBasicas.Count > 1)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Apenas uma cobertura Básica é permitida."
                };
            }

            if (coberturas.GroupBy(c => c.IdCobertura).Any(g => g.Count() > 1))
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = "Não é permitido adicionar coberturas repetidas."
                };
            }

            return new CotacaoAcaoDTO
            {
                Sucesso = true,
                Mensagem = ""
            };
        }

        private CotacaoAcaoDTO CalcularDescontoEAgravo(List<CotacaoCobertura> coberturas, FaixaIdade faixaIdade)
        {
            foreach (var cobertura in coberturas)
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
            }
            return new CotacaoAcaoDTO
            {
                Sucesso = true,
            };
        }

        private CotacaoAcaoDTO ValidarBeneficiarios(List<CotacaoBeneficiario> beneficiarios)
        {
            if (beneficiarios != null && beneficiarios.Any())
            {
                var somaPercentual = beneficiarios.Sum(b => b.Percentual);
                if (somaPercentual != 100)
                {
                   return new CotacaoAcaoDTO
                    {
                        Sucesso = false,
                        Mensagem = "A soma dos percentuais dos beneficiários deve ser 100."
                    };
                }
            }
            return new CotacaoAcaoDTO
            {
                Sucesso = true
            };

        }

        private void OrdenarBeneficiarios(List<CotacaoBeneficiario> beneficiario)
        {
            if (beneficiario != null && beneficiario.Any())
            {
                beneficiario = beneficiario.OrderBy(b => b.IdParentesco).ToList();
            }
        }

        private CotacaoAcaoDTO ValidarImportanciaSegurada(Cotacao cotacao, Produto produto)
        {
            if (cotacao.ImportanciaSegurada > produto.Limit)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = $"A Importância Segurada excede o limite do produto ({produto.Limit})."
                };
            }

            var produtos = _context.Produto.OrderBy(p => p.Limit).ToList();
            var produtoCorreto = produtos.FirstOrDefault(p => cotacao.ImportanciaSegurada <= p.Limit);
            if (produtoCorreto != null && produtoCorreto.Id != cotacao.IdProduto)
            {
                return new CotacaoAcaoDTO
                {
                    Sucesso = false,
                    Mensagem = $"A Importância Segurada de {cotacao.ImportanciaSegurada} deve ser usada com o produto {produtoCorreto.Description}."
                };
            }

            return new CotacaoAcaoDTO 
            { 
                Sucesso = true,
            };
        }

        private decimal CalcularPremio(Produto produto, List<CotacaoCobertura> coberturas)
        {
            return produto.BaseValue + coberturas.Sum(c => c.ValorTotal);
        }

    }
}
