using Microsoft.AspNetCore.Identity;

namespace LanchesMac.Models
{
    public class Usuarios : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ImagePath { get; set; }
    }
}
