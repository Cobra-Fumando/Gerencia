using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serviços.Tabelas
{
    [Table("RefreshToken")]
    [Index(nameof(Email), IsUnique = true)]
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email invalido")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Somente emails @gmail.com são permitidos")]
        public string Email { get; set; }

        public string Token { get; set; }

        public DateTime Expiration { get; set; }

    }
}
