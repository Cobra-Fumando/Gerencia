using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serviços.Tabelas
{
    [Table("Tb_Users")]
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(cpf), IsUnique = true)]
    public class Usuarios
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Nome é obrigatório")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email invalido")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$",ErrorMessage = "Somente emails @gmail.com são permitidos")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatório")]
        [MinLength(6, ErrorMessage = "A senha deve ter no minimo 6 carecter")]
        public required string Senha { get; set; }
        public string? Telefone { get; set; }
        public string? cpf { get; set; }
        public string? cep { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }

    public class UsuarioToken
    {
        public Usuarios Usuario { get; set; }
        public string Token { get; set; }
    }
}
