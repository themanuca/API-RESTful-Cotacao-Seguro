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
            var parceiro = await _context.Parceiros
                            .FirstOrDefaultAsync(p => p.Secret == Request.Headers["Secret"]);
            if (parceiro == null)
                return Unauthorized("Parceiro inválido"); var cotacaoDetalhar = await _cotacaoService.DetalharAsync(id, parceiro.Id);

            if (!cotacaoDetalhar.Sucesso)
            {
                return NotFound(cotacaoDetalhar.Mensagem);
            }

            return Ok(cotacaoDetalhar.Cotacao);
        }
    }
}
