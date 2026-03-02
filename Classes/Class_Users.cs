using Microsoft.EntityFrameworkCore;
using Serviços.Config;
using Serviços.Connection;
using Serviços.Tabelas;

namespace Serviços.Classes
{
    public class Class_Users
    {
        public AppDbContext context;
        public GenerateToken generateToken;
        public Hasher hasher;
        public Class_Users(AppDbContext context, GenerateToken generateToken, Hasher hasher)
        {
            this.context = context;
            this.generateToken = generateToken;
            this.hasher = hasher;
        }

        public async Task<TabelaProblem<UsuarioDto>> AdicionarUser(Usuarios usuarios)
        {
            try
            {
                Usuarios UsuarioCriado = new Usuarios
                {
                    Nome = usuarios.Nome,
                    Email = usuarios.Email,
                    Senha = hasher.HashPassword(usuarios.Senha),
                    Telefone = usuarios.Telefone,
                    cpf = usuarios.cpf,
                    cep = usuarios.cep
                };

                var usuarioExistente = await context.Usuarios.AsNoTracking()
                                            .Where(u => u.Email == UsuarioCriado.Email || u.cpf == UsuarioCriado.cpf)
                                            .Select(u => new { u.Email, u.cpf })
                                            .FirstOrDefaultAsync();

                if (usuarioExistente is not null)
                {
                    if (usuarioExistente.Email == UsuarioCriado.Email)
                    {
                        return StatusProblem.Fail<UsuarioDto>("Já existe um usuário cadastrado com esse email", null);
                    }
                    else if (usuarioExistente.cpf == UsuarioCriado.cpf)
                    {
                        return StatusProblem.Fail<UsuarioDto>("Já existe um usuário cadastrado com esse cpf", null);
                    }
                }

                await context.Usuarios.AddAsync(UsuarioCriado);
                await context.SaveChangesAsync();

                UsuarioDto Dto = new UsuarioDto
                {
                    Nome = UsuarioCriado.Nome,
                    Email = UsuarioCriado.Email,

                };

                return StatusProblem.Ok<UsuarioDto>("Usuário criado com sucesso", Dto);
            }catch(Exception ex)
            {
                return StatusProblem.Fail<UsuarioDto>("Ocorreu um erro ao tentar criar o usuário", null);
            }
        }

        public async Task<TabelaProblem<UsuarioToken>> Logar(UsuarioLogin usuario)
        {
            try
            {
                string Email = usuario.Email;
                string Senha = usuario.Senha;

                Usuarios? UsuarioEncontrado = await context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Email == Email);

                if (UsuarioEncontrado is null) return StatusProblem.Fail<UsuarioToken>("Email ou senha incorretos", null);

                bool senhaValida = hasher.verificar(UsuarioEncontrado.Senha, Senha);
                if(!senhaValida) return StatusProblem.Fail<UsuarioToken>("Email ou senha incorretos", null);

                string token = generateToken.GerarToken(UsuarioEncontrado.Nome);
                UsuarioToken usuarioToken = new UsuarioToken
                {
                    Usuario = UsuarioEncontrado,
                    Token = token
                };

                return StatusProblem.Ok<UsuarioToken>("Login realizado com sucesso", usuarioToken);
            }catch(Exception ex)
            {
                return StatusProblem.Fail<UsuarioToken>("Ocorreu um erro ao tentar logar", null);
            }
        }
    }
}
