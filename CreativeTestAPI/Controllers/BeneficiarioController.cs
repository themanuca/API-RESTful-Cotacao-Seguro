using Domain.Entities.Models;
using Domain.Interfaces;
using Infra.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CreativeTestAPI.Controllers
{
    [ApiController]
    [Route("api/cotacao/{idCotacao}/beneficiario")]
    public class BeneficiarioController : ControllerBase
    {
        private readonly IBeneficiarioService _beneficiarioService;
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAcessor;

        public BeneficiarioController(IBeneficiarioService beneficiarioService, AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _beneficiarioService = beneficiarioService;
            _context = context;
            _httpContextAcessor = httpContextAccessor;

        }

        [HttpPut]
        public async Task<IActionResult> AlterarListaBeneficiario(int idCotacao, [FromBody] CotacaoBeneficiario beneficiario)
        {
            var idParceiro = (int)_httpContextAcessor.HttpContext.Items["IdParceiro"];

            var result = await _beneficiarioService.AlterarAsync(idCotacao, beneficiario, idParceiro);
            if (!result.Sucesso)
                return BadRequest(result.Mensagem);

            return Ok(new { Status = "Sucesso", IdBeneficiario = beneficiario.Id });
        }

        [HttpGet]
        public async Task<IActionResult> ReuperarListaBeneficiariPorIdCotacao(int idCotacao)
        {
            var idParceiro = (int)_httpContextAcessor.HttpContext.Items["IdParceiro"];

            var beneficiarios = await _beneficiarioService.ListarAsync(idCotacao, idParceiro);
            return Ok(beneficiarios);
        }

        [HttpGet("{idBeneficiario}")]
        public async Task<IActionResult> RecuperarBenefificiarioCompleto(int idCotacao, int idBeneficiario)
        {
            var idParceiro = (int)_httpContextAcessor.HttpContext.Items["IdParceiro"];

            var beneficiario = await _beneficiarioService.DetalharAsync(idCotacao, idBeneficiario, idParceiro);
            if (beneficiario == null)
                return NotFound("Beneficiário não encontrado ou não pertence à cotação do parceiro.");

            return Ok(beneficiario);
        }

        [HttpDelete("{idBeneficiario}")]
        public async Task<IActionResult> ExcluirBeneficiarioPorIdCotacao(int idCotacao, int idBeneficiario)
        {
            var idParceiro = (int)_httpContextAcessor.HttpContext.Items["IdParceiro"];

            var result = await _beneficiarioService.ExcluirAsync(idCotacao, idBeneficiario, idParceiro);
            if (!result.Sucesso)
                return NotFound(result.Mensagem);

            return Ok(new { Status = "Sucesso" });
        }
    }
}
