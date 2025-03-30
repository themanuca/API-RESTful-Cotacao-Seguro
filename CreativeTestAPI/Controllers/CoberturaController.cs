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
        private readonly IHttpContextAccessor _httpContextAcessor;

        public CoberturaController(ICoberturaService coberturaService, AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _coberturaService = coberturaService;
            _context = context;
            _httpContextAcessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> IncluirCoberturaPorIdCotacao(int idCotacao, [FromBody] CotacaoCobertura cobertura)
        {
            var idParceiro = (int)_httpContextAcessor.HttpContext.Items["IdParceiro"];
            

            var result = await _coberturaService.IncluirAsync(idCotacao, cobertura, idParceiro);
            if (!result.Sucesso)
                return BadRequest(result.Mensagem);

            return Ok(new { Status = "Sucesso" });
        }

        [HttpGet]
        public async Task<IActionResult> RecupearCoberturasPorIdCotacao(int idCotacao)
        {
            var idParceiro = (int)_httpContextAcessor.HttpContext.Items["IdParceiro"];

            var coberturas = await _coberturaService.ListarAsync(idCotacao, idParceiro);
            return Ok(coberturas);
        }

        [HttpDelete("{idCobertura}")]
        public async Task<IActionResult> ExcluirCobertura(int idCotacao, int idCobertura)
        {
            var idParceiro = (int)_httpContextAcessor.HttpContext.Items["IdParceiro"];

            var result = await _coberturaService.ExcluirAsync(idCotacao, idCobertura, idParceiro);
            if (!result.Sucesso)
                return NotFound(result.Mensagem);

            return Ok(new { Status = "Sucesso" });
        }
    }
}
