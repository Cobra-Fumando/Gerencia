using Serviços.Config;
using Serviços.Tabelas;
using System.Security.Claims;
using Serviços.Connection;
using Microsoft.EntityFrameworkCore;

namespace Serviços.Classes
{
    public class Reserva
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly ILogger<Reserva> logger;
        private readonly AppDbContext context;
        public Reserva(IHttpContextAccessor httpContext, ILogger<Reserva> logger, AppDbContext context)
        {

            this.httpContext = httpContext;
            this.logger = logger;
            this.context = context;
        }

        public async Task<TabelaProblem<object>> AgendarHorario(Agendamento agendamento)
        {
            try
            {

                await context.Agendamento.AddAsync(agendamento);

                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao agendar horário para o usuário {Email}", agendamento.Email);
                return StatusProblem.Fail<object>("Não foi possível agendar o horário. Tente novamente mais tarde.");
            }

            return StatusProblem.Ok<object>("Agendado com sucesso");
        }

        public async Task<TabelaProblem<List<Agendamento>>> ListarAgendamentos()
        {
            try
            {
                var agendamentos = await context.Agendamento.ToListAsync();
                return StatusProblem.Ok<List<Agendamento>>("Agendamentos encontrados", agendamentos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao listar agendamentos");
                return StatusProblem.Fail<List<Agendamento>>("Não foi possível listar os agendamentos. Tente novamente mais tarde.");
            }
        }

        public async Task<TabelaProblem<object>> CancelarAgendamento(int id)
        {
            try
            {
                var agendamento = await context.Agendamento.FirstOrDefaultAsync(x => x.Id == id);

                if (agendamento is null)
                {
                    return StatusProblem.Fail<object>("Agendamento não encontrado");
                }

                TimeSpan DataConsulta = agendamento.DataAgendamento - DateTime.UtcNow;

                if (DataConsulta.TotalDays <= 2)
                {
                    return StatusProblem.Fail<object>("Não é possível cancelar um agendamento quando falta 2 dias para consulta");
                }
        
                context.Agendamento.Remove(agendamento);
                await context.SaveChangesAsync();
                return StatusProblem.Ok<object>("Agendamento cancelado com sucesso");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao cancelar agendamento com id {Id}", id);
                return StatusProblem.Fail<object>("Não foi possível cancelar o agendamento. Tente novamente mais tarde.");
            }
        }
    }
}
