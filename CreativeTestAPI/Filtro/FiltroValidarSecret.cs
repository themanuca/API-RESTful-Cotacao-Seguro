using Infra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CreativeTestAPI.Filtro
{
    public class FiltroValidarSecret : IActionFilter
    {
        private readonly AppDbContext _context;

        public FiltroValidarSecret(AppDbContext context)
        {
            _context = context;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            throw new NotImplementedException();
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("X-Parceiro-Secret", out var secretValue))
            {
                context.Result = new BadRequestObjectResult("O header 'X-Parceiro-Secret' é obrigatório.");
                return;
            }

            var parceiro = _context.Parceiro
                .FirstOrDefault(p => p.Secret == secretValue.ToString());

            if (parceiro == null)
            {
                context.Result = new UnauthorizedObjectResult("Secret do parceiro inválido.");
                return;
            }

            context.HttpContext.Items["IdParceiro"] = parceiro.Id;

        }
    }
}
