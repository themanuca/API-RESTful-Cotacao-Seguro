using Domain.Entities.Models;
using Domain.Interfaces;
using Infra.Data;
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

        public BeneficiarioController(IBeneficiarioService beneficiarioService, AppDbContext context)
        {
            _beneficiarioService = beneficiarioService;
            _context = context;
        }

        [HttpPut]
        public async Task<IActionResult> AlterarListaBeneficiario(int idCotacao, [FromBody] CotacaoBeneficiario beneficiario)
        {
            var parceiro = await _context.Parceiro
                .FirstOrDefaultAsync(p => p.Secret == Request.Headers["Secret"]);
            if (parceiro == null)
                return Unauthorized("Parceiro inválido");

            var result = await _beneficiarioService.AlterarAsync(idCotacao, beneficiario, parceiro.Id);
            if (!result.Sucesso)
                return BadRequest(result.Mensagem);

            return Ok(new { Status = "Sucesso", IdBeneficiario = beneficiario.Id });
        }

        [HttpGet]
        public async Task<IActionResult> ReuperarListaBeneficiariPorIdCotacao(int idCotacao)
        {
            var parceiro = await _context.Parceiro
                .FirstOrDefaultAsync(p => p.Secret == Request.Headers["Secret"]);
            if (parceiro == null)
                return Unauthorized("Parceiro inválido");

            var beneficiarios = await _beneficiarioService.ListarAsync(idCotacao, parceiro.Id);
            return Ok(beneficiarios);
        }

        [HttpGet("{idBeneficiario}")]
        public async Task<IActionResult> RecuperarBenefificiarioCompleto(int idCotacao, int idBeneficiario)
        {
            //var parceiro = await _context.Parceiros
            //    .FirstOrDefaultAsync(p => p.Secret == Request.Headers["Secret"]);
            //if (parceiro == null)
            //    return Unauthorized("Parceiro inválido");

            var beneficiario = await _beneficiarioService.DetalharAsync(idCotacao, idBeneficiario, 1);
            if (beneficiario == null)
                return NotFound("Beneficiário não encontrado ou não pertence à cotação do parceiro.");

            return Ok(beneficiario);
        }

        [HttpDelete("{idBeneficiario}")]
        public async Task<IActionResult> ExcluirBeneficiarioPorIdCotacao(int idCotacao, int idBeneficiario)
        {
            var parceiro = await _context.Parceiro
                .FirstOrDefaultAsync(p => p.Secret == Request.Headers["Secret"]);
            if (parceiro == null)
                return Unauthorized("Parceiro inválido");

            var result = await _beneficiarioService.ExcluirAsync(idCotacao, idBeneficiario, parceiro.Id);
            if (!result.Sucesso)
                return NotFound(result.Mensagem);

            return Ok(new { Status = "Sucesso" });
        }
    }
}
