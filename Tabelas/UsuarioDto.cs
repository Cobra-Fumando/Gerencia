namespace Serviços.Tabelas
{
    public class UsuarioDto
    {
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public string? Cep { get; set; }
        public string? Telefone { get; set; }
        public string? Cpf { get; set; }
    }
}
