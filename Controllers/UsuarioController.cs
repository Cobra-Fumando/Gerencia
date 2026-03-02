using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serviços.Classes;
using Serviços.Tabelas;

namespace Serviços.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        Class_Users class_Users;
        public UsuarioController(Class_Users class_Users)
        {
            this.class_Users = class_Users;
        }
        [HttpPost("Cadastrar")]
        public async Task<IActionResult> Cadastrar(Usuarios usuarios)
        {
            TabelaProblem<UsuarioDto> user = await class_Users.AdicionarUser(usuarios);
            if (!user.Sucesso) return BadRequest(new { Mensagem = user.Mensagem });
            return Ok(new { Mensagem = "Usuario cadastrado com sucesso", Dados = user.Dados });
        }

        [HttpPost("Logar")]
        public async Task<IActionResult> Logar(UsuarioLogin usuario)
        {
            TabelaProblem<UsuarioToken> user = await class_Users.Logar(usuario);
            if (!user.Sucesso) return BadRequest(new { Mensagem = user.Mensagem });
            return Ok(new { Mensagem = user.Mensagem, Dados = user.Dados });
        }
    }
}
