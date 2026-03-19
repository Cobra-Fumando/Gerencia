using Serviços.Tabelas;

namespace Serviços.Interfaces
{
    public interface IClass_Users
    {
        public Task<TabelaProblem<UsuarioDto>> AdicionarUser(Usuarios usuarios);
        public Task<TabelaProblem<object>> Confirmar(UsuarioConfirmacao confirmacao);
        public Task<TabelaProblem<UsuarioToken>> Logar(UsuarioLogin usuario);
    }
}
