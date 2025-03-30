using Domain.DTOs;
using Domain.Entities.Models;
using Domain.Interfaces;
using Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreativeTestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CotacaoController : ControllerBase
    {
        private readonly ICotacaoService _cotacaoService;
        private readonly AppDbContext _context;

        public CotacaoController(ICotacaoService cotacaoService, AppDbContext context)
        {
            _cotacaoService = cotacaoService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Incluir([FromBody] CotacaoDetalheDTO cotacao)
        {
            var cotacaoIncluir = await _cotacaoService.IncluirAsync(cotacao, 1);

            if (!cotacaoIncluir.Sucesso)
            {
                return BadRequest(cotacaoIncluir.Mensagem);
            }

            return Ok(new CotacaoIncluirDTO{ Mensagem = "Sucesso", IdCotacao = cotacaoIncluir.IdCotacao, Sucesso=true });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detalhar(int id)
        {
            //var parceiro = await _context.Parceiro
            //                .FirstOrDefaultAsync(p => p.Id == id);
            //if (parceiro == null)
            //    return Unauthorized("Parceiro inválido");
            var cotacaoDetalhar = await _cotacaoService.DetalharAsync(id, 1);

            if (!cotacaoDetalhar.Sucesso)
            {
                return NotFound(cotacaoDetalhar.Mensagem);
            }

            return Ok(cotacaoDetalhar.Cotacao);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Alterar(int id, [FromBody] CotacaoDetalheDTO cotacao)
        {
            // Obter o IdParceiro do HttpContext (injetado pelo middleware)
            if (!HttpContext.Items.TryGetValue("IdParceiro", out var idParceiroObj) || idParceiroObj == null)
            {
                return Unauthorized("Parceiro não autenticado.");
            }

            int idParceiro = (int)idParceiroObj;

            var cotacaoAcao = await _cotacaoService.AlterarAsync(id, cotacao, idParceiro);

            if (!cotacaoAcao.Sucesso)
            {
                return BadRequest(cotacaoAcao.Mensagem);
            }

            return Ok(new CotacaoAcaoDTO{ Sucesso= true, Mensagem = cotacaoAcao.Mensagem });
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            // Obter o IdParceiro do HttpContext (injetado pelo middleware)
            if (!HttpContext.Items.TryGetValue("IdParceiro", out var idParceiroObj) || idParceiroObj == null)
            {
                return Unauthorized("Parceiro não autenticado.");
            }

            int idParceiro = (int)idParceiroObj;

            var cotacaoListar = await _cotacaoService.ListarAsync(idParceiro);

            if (!cotacaoListar.Sucesso)
            {
                return NotFound(cotacaoListar.Mensagem);
            }

            return Ok(cotacaoListar.Cotacoes);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            // Obter o IdParceiro do HttpContext (injetado pelo middleware)
            if (!HttpContext.Items.TryGetValue("IdParceiro", out var idParceiroObj) || idParceiroObj == null)
            {
                return Unauthorized("Parceiro não autenticado.");
            }

            int idParceiro = (int)idParceiroObj;

            var cotacaoAcao = await _cotacaoService.ExcluirAsync(id, idParceiro);

            if (!cotacaoAcao.Sucesso)
            {
                return NotFound(cotacaoAcao.Mensagem);
            }

            return Ok();
        }
    }
}
