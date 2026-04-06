using System.ComponentModel.DataAnnotations;

namespace Serviços.Tabelas
{
    public class UsuarioLogin
    {
        [Required(ErrorMessage = "Email não pode estar vazio")]
        [EmailAddress(ErrorMessage = "Email invalido")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Somente emails @gmail.com são permitidos")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Senha não pode estar vazio")]
        [MinLength(6, ErrorMessage = "A senha tem que ter pelo menos 6 caracter")]
        public required string Senha { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        public required string Nome { get; set; }
    }
}
