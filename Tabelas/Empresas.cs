using System.ComponentModel.DataAnnotations.Schema;

namespace Serviços.Tabelas
{
    [Table("Empresas")]
    public class Empresas
    {
        public int Id { get; set; }
        public string NomeEmpresa { get; set; }
        public string Cnpj { get; set; }
        public string Endereco { get; set; }
        public string Telefone { get; set; }

        public int UsuarioId { get; set; }
        public List<Usuarios> Usuario { get; set; }
    }
}
