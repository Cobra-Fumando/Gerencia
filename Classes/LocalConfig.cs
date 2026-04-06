using Microsoft.EntityFrameworkCore;
using Serviços.Config;
using Serviços.Connection;
using Serviços.Tabelas;

namespace Serviços.Classes
{
    public class LocalConfig
    {
        private readonly AppDbContext context;
        private readonly ILogger<LocalConfig> logger;
        public LocalConfig(AppDbContext context, ILogger<LocalConfig> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<TabelaProblem<object>> CadastrarLocal(Local local)
        {
            try
            {
                await context.Locais.AddAsync(local);

                return StatusProblem.Ok<object>("Local cadastrado com sucesso");
            }
            catch(Exception ex)
            {
                return StatusProblem.Fail<object>("Erro inesperado");
            }
        }

        public async Task<TabelaProblem<List<Local>>> ListarLocais()
        {
            try
            {
                var locais = await context.Locais.ToListAsync();
                return StatusProblem.Ok<List<Local>>("Locais encontrado ", locais);
            }
            catch(Exception ex)
            {
                return StatusProblem.Fail<List<Local>>("Erro inesperado");
            }
        }

        public async Task<TabelaProblem<object>> AtualizarLocal(LocalAtualizacao local)
        {
            try
            {

                var localBanco = await context.Locais.FirstOrDefaultAsync(x => x.Id == local.Id);

                if (localBanco == null)
                {
                    return StatusProblem.Fail<object>("Local não encontrado");
                }

                if (!string.IsNullOrWhiteSpace(local.Nome)) localBanco.Nome = local.Nome;
                if (!string.IsNullOrWhiteSpace(local.Descricao)) localBanco.Descricao = local.Descricao;
                if (!string.IsNullOrWhiteSpace(local.Endereco)) localBanco.Endereco = local.Endereco;
                if (local.Capacidade.HasValue) localBanco.Capacidade = local.Capacidade.Value;

                context.Locais.Update(localBanco);
                await context.SaveChangesAsync();

                return StatusProblem.Ok<object>("Atualizado com sucesso");
            }
            catch (Exception ex)
            {
                logger.LogError("Erro inesperado");
                return StatusProblem.Fail<object>("Erro inesperado");
            }
        }
    }
}
