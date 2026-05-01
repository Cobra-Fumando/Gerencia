using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serviços.Tabelas
{
    [Table("Agendamento")]
    public class Agendamento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]
        public required string NomeCompleto { get; set; }

        [Required(ErrorMessage = "Email não pode estar vazio")]
        [EmailAddress(ErrorMessage = "Email invalido")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Somente emails @gmail.com são permitidos")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Cpf não pode estar vazio")]
        public required string Cpf { get; set; }

        [Required(ErrorMessage = "Telefone não pode estar vazio")]
        public string Telefone { get; set; }
        public DateTime DataAgendamento { get; set; }

        public bool MensagemEnviada { get; set; } = false;

        [ForeignKey("Local")]
        public int LocalId { get; set; }
        public Local Local { get; set; }
    }
}
