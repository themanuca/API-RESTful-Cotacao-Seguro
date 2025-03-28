using Domain.DTOs;
using Domain.Entities.Models;
using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CreativeTestAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class CotacaoController : ControllerBase
    {
        private readonly ICotacaoService _cotacaoService;
        public CotacaoController(ICotacaoService cotacaoService)
        {
            _cotacaoService = cotacaoService;
        }

        [HttpPost]
        public async Task<IActionResult> Incluir([FromBody] Cotacao cotacao)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var idParceiro = (int)HttpContext.Items["IdParceiro"];
            var cotacaoIncluir = await _cotacaoService.IncluirAsync(cotacao, idParceiro);

            if (!cotacaoIncluir.Sucesso)
            {
                return BadRequest(cotacaoIncluir.Mensagem);
            }

            return Ok(new CotacaoIncluirDTO{ Mensagem = "Sucesso", IdCotacao = cotacaoIncluir.IdCotacao, Sucesso=true });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detalhar(int id)
        {
            var idParceiro = (int)HttpContext.Items["IdParceiro"];
            var cotacaoDetalhar = await _cotacaoService.DetalharAsync(id, idParceiro);

            if (!cotacaoDetalhar.Sucesso)
            {
                return NotFound(cotacaoDetalhar.Mensagem);
            }

            return Ok(cotacaoDetalhar.Cotacao);
        }
    }
}
