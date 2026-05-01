using Microsoft.EntityFrameworkCore;
using Serviços.Connection;
using Serviços.Config;

namespace Serviços.Baclground
{
    public class CheckAgenda : BackgroundService
    {
        private readonly ILogger<CheckAgenda> logger;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public CheckAgenda(ILogger<CheckAgenda> logger, IServiceScopeFactory serviceScopeFactory)
        {
            this.logger = logger;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = serviceScopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var enviarMensagem = scope.ServiceProvider.GetRequiredService<EnviarMensagem>();
                    var inicio = DateTime.UtcNow.Date;
                    var fim = inicio.AddDays(1);
                    int take = 100;


                    var lista = await context.Agendamento
                                    .Where(a =>
                                        a.DataAgendamento >= inicio &&
                                        a.DataAgendamento < fim &&
                                        !a.MensagemEnviada)
                                    .OrderBy(a => a.Id)
                                    .Take(100)
                                    .ToListAsync(stoppingToken);


                    foreach (var a in lista)
                    {
                        a.MensagemEnviada = true;

                        await enviarMensagem.EnviarEmail(
                            "EmailEmpresa",
                            "Lembrete de Agendamento",
                            a.Email,
                            a.NomeCompleto,
                            $"Olá! Este é um lembrete de que você tem um agendamento marcado para hoje às {a.DataAgendamento:HH:mm}. Por favor, esteja preparado para o seu compromisso.",
                            "Senha");

                    }

                    await context.SaveChangesAsync(stoppingToken);

                    int deletados = await context.Agendamento.Where(a => a.DataAgendamento < DateTime.UtcNow).ExecuteDeleteAsync(stoppingToken);

                    if (deletados > 0)
                    {
                        logger.LogInformation($"Deletados {deletados} agendamento(s) expirados");
                    }

                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro ao verificar agendamentos");
                }
            }
        }

    }
}
