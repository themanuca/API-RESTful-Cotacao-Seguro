using Infra.Data;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CreativeTestAPI.Controllers
{
    public class TesteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TesteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetProdutos()
        {
            var produtos = _context.Produto.ToList();
            return Ok("ATE AQUI !!!");
        }
    }
}
