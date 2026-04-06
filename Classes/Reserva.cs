using Serviços.Config;
using Serviços.Tabelas;
using System.Security.Claims;

namespace Serviços.Classes
{
    public class Reserva
    {
        private readonly IHttpContextAccessor httpContext;
        public Reserva(IHttpContextAccessor httpContext) 
        {
        
            this.httpContext = httpContext;
        }

        public async Task<TabelaProblem<object>> AgendarHorario(Agendamento agendamento)
        {
            var contextA = httpContext.HttpContext;
            var Nome = contextA.User.FindFirst(ClaimTypes.Name)?.Value;
            var Email = contextA.User.FindFirst(ClaimTypes.Email)?.Value;

            //agendar reserva usando os dados do agendamento e do usuário autenticado
            //empresas

            return StatusProblem.Ok<object>("Continuar");
        }
    }
}
