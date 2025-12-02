using System.ComponentModel.DataAnnotations;

namespace LanchesMac.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Informe o seu nome")]
        [Display(Name = "Nome")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Informe o seu sobrenome")]
        [Display(Name = "Sobrenome")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Informe a sua senha")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Informe o seu e-mail")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}",
            ErrorMessage = "E-mail em formato inválido")]
        public string Email { get; set; } // se eu deixar esta propriedade, a validação do modelo falha ao tentar
                                          //logar por causa da validação ModelState.IsValid no AccountController
                                          //já que ela verifica se todos os campos da viewmodel foram preenchidos e
                                          //neste caso não estava sendo passado nenhum email, ainda

        [Required(ErrorMessage = "Informe o seu telefone")]
        [Display(Name = "Telefone")]
        [RegularExpression(@"^\s*(\+\d{1,3}\s?)?(|\d{0}|\(?\d{2}\)?[-\.\s]?)(\d{4,5}[-\.\s]?\d{4})\s*$",
            ErrorMessage = "Telefone em formato inválido")]
        public string PhoneNumber { get; set; }
        public string? ImagePath { get; set; }
        public string ReturnUrl { get; set; }
        public string Registro { get; set; }
    }
}
