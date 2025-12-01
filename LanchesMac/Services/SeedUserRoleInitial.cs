using LanchesMac.Models;
using Microsoft.AspNetCore.Identity;

namespace LanchesMac.Services
{
    public class SeedUserRoleInitial : ISeedUserRoleInitial
    {
        private readonly UserManager<Usuarios> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedUserRoleInitial(UserManager<Usuarios> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void SeedRoles()
        {
            if (!_roleManager.RoleExistsAsync("Member").Result) // aqui estou vendo se o papel de "Member" já existe
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Member";
                role.NormalizedName = "MEMBER";
                IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
            }

            if (!_roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new IdentityRole();
                role.Name = "Admin";
                role.NormalizedName = "ADMIN";
                IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
            }
        }

        public void SeedUsers()
        {
            if (_userManager.FindByEmailAsync("usuario@localhost").Result == null)
            {
                Usuarios user = new Usuarios(); // aqui estou criando um novo usuário caso ele não exista
                user.UserName = "usuario@localhost";
                user.Email = "usuario@localhost";
                user.NormalizedUserName = "USUARIO@LOCALHOST";
                user.NormalizedEmail = "USUARIO@LOCALHOST";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult result = _userManager.CreateAsync(user, "Numsey#2022").Result; // aqui estou criando o usuário com uma senha padrão

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "Member").Wait(); // aqui estou adicionando o usuário ao papel de "Member"
                }
            }

            if (_userManager.FindByEmailAsync("admin@localhost").Result == null)
            {
                Usuarios user = new Usuarios(); // aqui estou criando um novo usuário caso ele não exista
                user.UserName = "admin@localhost";
                user.Email = "admin@localhost";
                user.NormalizedUserName = "ADMIN@LOCALHOST";
                user.NormalizedEmail = "ADMIN@LOCALHOST";
                user.EmailConfirmed = true;
                user.LockoutEnabled = false;
                user.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult result = _userManager.CreateAsync(user, "Numsey#2022").Result; // aqui estou criando o usuário com uma senha padrão

                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "Admin").Wait(); // aqui estou adicionando o usuário ao papel de "Member"
                }
            }
        }
    }
}
