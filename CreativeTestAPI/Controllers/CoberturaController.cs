using Domain.Entities.Models;
using Domain.Interfaces;
using Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreativeTestAPI.Controllers
{
    [ApiController]
    [Route("api/cotacao/{idCotacao}/cobertura")]
    public class CoberturaController : ControllerBase
    {
        private readonly ICoberturaService _coberturaService;
        private readonly AppDbContext _context;

        public CoberturaController(ICoberturaService coberturaService, AppDbContext context)
        {
            _coberturaService = coberturaService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> IncluirCoberturaPorIdCotacao(int idCotacao, [FromBody] CotacaoCobertura cobertura)
        {
            var parceiro = await _context.Parceiro
                .FirstOrDefaultAsync(p => p.Secret == Request.Headers["Secret"]);
            if (parceiro == null)
                return Unauthorized("Parceiro inválido");

            var result = await _coberturaService.IncluirAsync(idCotacao, cobertura, parceiro.Id);
            if (!result.Sucesso)
                return BadRequest(result.Mensagem);

            return Ok(new { Status = "Sucesso" });
        }

        [HttpGet]
        public async Task<IActionResult> RecupearCoberturasPorIdCotacao(int idCotacao)
        {
            var parceiro = await _context.Parceiro
                .FirstOrDefaultAsync(p => p.Secret == Request.Headers["Secret"]);
            if (parceiro == null)
                return Unauthorized("Parceiro inválido");

            var coberturas = await _coberturaService.ListarAsync(idCotacao, parceiro.Id);
            return Ok(coberturas);
        }

        [HttpDelete("{idCobertura}")]
        public async Task<IActionResult> ExcluirCobertura(int idCotacao, int idCobertura)
        {
            var parceiro = await _context.Parceiro
                .FirstOrDefaultAsync(p => p.Secret == Request.Headers["Secret"]);
            if (parceiro == null)
                return Unauthorized("Parceiro inválido");

            var result = await _coberturaService.ExcluirAsync(idCotacao, idCobertura, parceiro.Id);
            if (!result.Sucesso)
                return NotFound(result.Mensagem);

            return Ok(new { Status = "Sucesso" });
        }
    }
}
