using Microsoft.AspNetCore.Identity;

namespace LanchesMac.Models
{
    public class Usuarios : IdentityUser
    {
        public string Endereco { get; set; }
    }
}
