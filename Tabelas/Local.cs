using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Serviços.Tabelas
{
    [Table("Local")]
    public class Local
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "O Nome do lugar é obrigatório")]
        public required string Nome { get; set; }

        [Required(ErrorMessage = "A descrição do lugar é obrigatória")]
        public required string Descricao { get; set; }

        [Required(ErrorMessage = "A capacidade não pode ser vazia")]
        [MinLength(1, ErrorMessage = "A capacidade deve ser maior que 0")]
        public int Capacidade { get; set; }

        [Required(ErrorMessage = "O endereço do lugar é obrigatório")]
        public required string Endereco { get; set; }
        public bool ativo { get; set; } = true;
    }

    public class LocalAtualizacao
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Descricao { get; set; }
        public int? Capacidade { get; set; }
        public string? Endereco { get; set; }
        public bool ativo { get; set; }

    }
}
