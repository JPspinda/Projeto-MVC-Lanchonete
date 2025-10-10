using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanchesMac.Models
{
    [Table("Categorias")]
    public class Categoria
    {
        [Key]
        public int CategoriaId { get; set; }

        [StringLength(100, ErrorMessage = "O tamanho máximo é de 100 caracteres")]
        [Required(ErrorMessage = "Informe o nome da caregoria")]
        [Display(Name = "Nome")]
        public string categoriaNome { get; set; }

        [StringLength(200, ErrorMessage = "O tamanho máximo é de 200 caracteres")]
        [Required(ErrorMessage = "Informe a descrição da caregoria")]
        [Display(Name = "Descricao")]
        public string Descricao { get; set; }
        public List<Lanche> Lanches { get; set; }
    }
}
