using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Serviços.Interfaces;
using Serviços.Tabelas;

namespace Serviços.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        IClass_Users class_Users;
        public UsuarioController(IClass_Users class_Users)
        {
            this.class_Users = class_Users;
        }
        [HttpPost("Cadastrar")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Cadastrar([FromBody] Usuarios usuarios)
        {
            TabelaProblem<UsuarioDto> user = await class_Users.AdicionarUser(usuarios);
            if (!user.Sucesso) return BadRequest(new { Mensagem = user.Mensagem });
            return Ok(new { Mensagem = "Usuario cadastrado com sucesso", Dados = user.Dados });
        }

        [HttpGet("Confirm")]
        [Authorize(AuthenticationSchemes = "Confirm")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Confirm([FromBody] UsuarioConfirmacao confirmacao)
        {
            TabelaProblem<object> result = await class_Users.Confirmar(confirmacao);
            if(!result.Sucesso) return BadRequest(new { Mensagem = result.Mensagem });
            return Ok(new { Mensagem = result.Mensagem });
        }

        [HttpPost("Logar")]
        [EnableRateLimiting("fixed")]
        public async Task<IActionResult> Logar([FromBody] UsuarioLogin usuario)
        {
            TabelaProblem<UsuarioToken> user = await class_Users.Logar(usuario);
            if (!user.Sucesso) return BadRequest(new { Mensagem = user.Mensagem });
            return Ok(new { Mensagem = user.Mensagem, Dados = user.Dados?.Token });
        }
    }
}
