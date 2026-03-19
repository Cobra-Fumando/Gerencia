using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serviços.Config;
using Serviços.Connection;
using Serviços.Interfaces;
using Serviços.Tabelas;
using System.Security.Claims;

namespace Serviços.Classes
{
    public class Class_Users : IClass_Users
    {
        private readonly AppDbContext context;
        private readonly GenerateToken generateToken;
        private readonly Hasher hasher;
        private readonly ILogger<Class_Users> logger;
        private readonly IMemoryCache cache;
        private readonly EnviarMensagem enviarMensagem;
        private readonly IHttpContextAccessor HttpContext;
        public Class_Users(AppDbContext context, GenerateToken generateToken, Hasher hasher, ILogger<Class_Users> logger, IMemoryCache cache, EnviarMensagem enviarMensagem, IHttpContextAccessor httpContext)
        {
            this.context = context;
            this.generateToken = generateToken;
            this.hasher = hasher;
            this.logger = logger;
            this.cache = cache;
            this.enviarMensagem = enviarMensagem;
            this.HttpContext = httpContext;
        }

        public async Task<TabelaProblem<UsuarioDto>> AdicionarUser(Usuarios usuarios)
        {
            string getToken = string.Empty;
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

                if (!cache.TryGetValue($"Usuario_{UsuarioCriado.Email}", out Usuarios? user))
                {
                    user = UsuarioCriado;
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                                                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                                                .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                    var number = new GerarNumero();
                    var codigo = string.Join("", number.numero);

                    UsuarioConfirmacao confirmacao = new UsuarioConfirmacao
                    {
                        Usuario = user,
                        Codigo = codigo,
                        Tentativas = 0
                    };

                    cache.Set($"Usuario_{user.Email}", confirmacao, cacheEntryOptions);

                    getToken = generateToken.GerarTokenConfirmacao(user.Nome, user.Email);

                    await enviarMensagem.Enviar("Emaiempresa", "Verificar Email", user.Email, $"Seu codigo é: {codigo}", "Senha aplicativo", getToken);

                    logger.LogInformation("Usuário adicionado ao cache: {Email}", user.Email);
                }
                else
                {
                    logger.LogInformation("Já existe um usúrio em cache, tente novamente mais tarde");
                    return StatusProblem.Fail<UsuarioDto>("Já existe um usuário em cache, tente novamente mais tarde", null);
                }


                return StatusProblem.Ok<UsuarioDto>($"Requisição enviada com sucesso: {getToken}", null);
            }
            catch (Exception ex)
            {
                return StatusProblem.Fail<UsuarioDto>("Ocorreu um erro ao tentar criar o usuário", null);
            }
        }
        public async Task<TabelaProblem<object>> Confirmar(UsuarioConfirmacao confirmacao)
        {

            var Context = HttpContext.HttpContext;

            if (Context?.User?.Identity?.IsAuthenticated != true) return StatusProblem.Fail<object>("Usuário não autenticado", null);

            if (string.IsNullOrWhiteSpace(confirmacao.Codigo) || confirmacao.Codigo.Length != 4 || !confirmacao.Codigo.All(char.IsDigit))
            {
                return StatusProblem.Fail<object>("Código de confirmação inválido", null);
            }

            var Email = Context?.User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrWhiteSpace(Email)) return StatusProblem.Fail<object>("Não foi possível obter o email do usuário", null);

            if (!cache.TryGetValue($"Usuario_{Email}", out UsuarioConfirmacao? confirmacaoCache))
            {
                return StatusProblem.Fail<object>("Código de confirmação expirado ou inválido", null);
            }

            if (confirmacaoCache?.Tentativas >= 5)
            {
                cache.Remove($"Usuario_{Email}");
                return StatusProblem.Fail<object>("Número máximo de tentativas atingido, por favor solicite um novo código de confirmação", null);
            }

            if (confirmacaoCache.UltimaTentativa.AddSeconds(10) > DateTime.UtcNow) return StatusProblem.Fail<object>("Aguarde 10 segundos antes de tentar novamente", null);

            if (confirmacao.Codigo != confirmacaoCache?.Codigo)
            {
                confirmacaoCache.Tentativas++;
                confirmacaoCache.UltimaTentativa = DateTime.UtcNow;
                return StatusProblem.Fail<object>("Código de confirmação inválido", null);
            }

            try
            {

                await context.Usuarios.AddAsync(confirmacaoCache.Usuario);
                await context.SaveChangesAsync();

                cache.Remove($"Usuario_{Email}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao confirmar usúario");
                return StatusProblem.Fail<object>("Ocorreu um erro ao tentar confirmar o usuário", null);
            }

            return StatusProblem.Ok<object>("Confirmação realizada com sucesso", null);
        }

        public async Task<TabelaProblem<UsuarioToken>> Logar(UsuarioLogin usuario)
        {
            try
            {
                string Email = usuario.Email;
                string Senha = usuario.Senha;

                var Context = HttpContext.HttpContext;
                string? refreshToken = Context?.Request.Cookies["refreshToken"];

                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    var user = await context.Usuarios.AsNoTracking()
                                    .FirstOrDefaultAsync(u => u.Email == usuario.Email);

                    if (user == null) return StatusProblem.Fail<UsuarioToken>("Usuário ou senha inválidos", null);
                    if (!hasher.verificar(Senha, user.Senha)) return StatusProblem.Fail<UsuarioToken>("Usúario ou senha inválidos", null);

                    string Token = generateToken.GerarToken(user.Nome);
                    refreshToken = Guid.NewGuid().ToString();
                    string? refreshTokenCryp = HashToken.GerarHashToken(refreshToken);

                    Context?.Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(7)
                    });

                    context.refreshTokens.Add(new RefreshToken
                    {
                        Nome = user.Nome,
                        Token = refreshTokenCryp,
                        Email = user.Email,
                        Expiration = DateTime.UtcNow.AddDays(7)
                    });

                    await context.SaveChangesAsync();

                    return StatusProblem.Ok<UsuarioToken>("Login realizado com sucesso", new UsuarioToken
                    {
                        Usuario = new UsuarioLogin
                        {
                            Email = user.Email,
                            Nome = user.Nome,
                            Senha = string.Empty
                        },
                        Token = Token
                    });
                }

                var TokenCryp = HashToken.GerarHashToken(refreshToken);
                var TokenInformation = await context.refreshTokens
                                             .FirstOrDefaultAsync(u => u.Token == TokenCryp);

                if (TokenInformation == null)
                {
                    return StatusProblem.Fail<UsuarioToken>("Token de atualização inválido", null);
                }
                else if (TokenInformation.Expiration < DateTime.UtcNow)
                {
                    Context?.Response.Cookies.Delete("refreshToken");
                    context.refreshTokens.Remove(TokenInformation);
                    await context.SaveChangesAsync();
                    return StatusProblem.Fail<UsuarioToken>("Token de atualização inválido", null);
                }

                var TokenNovo = generateToken.GerarToken(TokenInformation.Nome);
                var refreshTokenNovo = Guid.NewGuid().ToString();
                var refreshTokenNovoCryp = HashToken.GerarHashToken(refreshTokenNovo);

                Context?.Response.Cookies.Append("refreshToken", refreshTokenNovo, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                context.refreshTokens.Remove(TokenInformation);
                await context.refreshTokens.AddAsync(new RefreshToken
                {
                    Nome = TokenInformation.Nome,
                    Token = refreshTokenNovoCryp,
                    Email = TokenInformation.Email,
                    Expiration = DateTime.UtcNow.AddDays(7)
                });

                await context.SaveChangesAsync();

                return StatusProblem.Ok<UsuarioToken>("Login realizado com sucesso", new UsuarioToken
                {
                    Usuario = new UsuarioLogin
                    {
                        Email = TokenInformation.Email,
                        Nome = TokenInformation.Nome,
                        Senha = string.Empty
                    },
                    Token = TokenNovo
                });

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao tentar logar");
                return StatusProblem.Fail<UsuarioToken>("Ocorreu um erro ao tentar logar", null);
            }
        }
    }
}
