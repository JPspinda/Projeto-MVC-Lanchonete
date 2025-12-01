using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;

namespace LanchesMac.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Usuarios> _userManager;
        private readonly SignInManager<Usuarios> _signInManager;
        private string _filePath;
        private readonly AppDbContext _context;

        public AccountController(
            UserManager<Usuarios> userManager, 
            SignInManager<Usuarios> signInManager,
            IWebHostEnvironment _env,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _filePath = _env.WebRootPath;
            _context = context;
        }

        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
           //if (!ModelState.IsValid) //O que isso faz, precisamente:
                                    //•	ModelState é preenchido pelo model binding do ASP.NET e pela validação baseada em atributos de dados(DataAnnotations)
                                    //  do LoginViewModel — por exemplo[Required], [DataType], [RegularExpression] etc.
                                    //•	ModelState.IsValid == false indica que uma ou mais regras de validação falharam(campos faltando, formato inválido, etc).
                                    //•	Ao retornar a View(loginVM) quando inválido você:
                                   //•	evita executar lógica sobre dados inválidos(ex.: _userManager.FindByNameAsync),
                                   //•	impede operações inseguras ou desperdício de recursos,
                                   //•	permite que a view mostre mensagens de erro(via @Html.ValidationSummary() / @Html.ValidationMessageFor()),
                                    //  oferecendo feedback ao usuário.
           // {
           //     return View(loginVM);
           // }

            var user = await _userManager.FindByEmailAsync(loginVM.Email);

            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false); //os parâmetros 
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(loginVM.ReturnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    return Redirect(loginVM.ReturnUrl);
                }
            }

            ModelState.AddModelError("", "Falha ao realizar o login");
            return View(loginVM);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(LoginViewModel registroVM)
        {
            if (ModelState.IsValid)
            {
                var user = new Usuarios { UserName = registroVM.UserName, Email = registroVM.Email };
                var result = await _userManager.CreateAsync(user, registroVM.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("Registro", "Falha ao registrar usuário");
                }
            }

            return View(registroVM);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();// Limpa a sessão atual
            HttpContext.User = null; // Limpa o usuário autenticado no contexto HTTP
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult SetProfileImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SetProfileImage(IFormFile anexo)
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
                return RedirectToAction("Index", "Home");

            if (!ValidaImagem(anexo))
                return View();

            if (!string.IsNullOrEmpty(usuario.ImagePath))
            {
                var filePath = _filePath + "\\fotos\\" + usuario.ImagePath;

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            var nome = SalvarArquivo(anexo);
            usuario.ImagePath = nome;

            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home"); ;
        }

        public IActionResult DeleteProfileImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProfileImageConfirmed()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (string.IsNullOrEmpty(usuario.ImagePath))
            {
                return RedirectToAction("Index", "Home");
            }

            if (usuario == null)
                return RedirectToAction("Index", "Home");

            if (string.IsNullOrEmpty(usuario.ImagePath))
                return RedirectToAction("Index", "Home");

            var filePath = _filePath + "\\fotos\\" + usuario.ImagePath;

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            usuario.ImagePath = null;

            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> AccessDenied()
        {
            return View();
        }

        public bool ValidaImagem(IFormFile anexo)
        {
            switch (anexo.ContentType)
            {
                case "image/jpeg":
                    return true;
                case "image/bmp":
                    return true;
                case "image/png":
                    return true;
                case "image/gif":
                    return true;
                default:
                    return false;
            }
        }

        public string SalvarArquivo(IFormFile anexo)
        {
            var nome = Guid.NewGuid().ToString() + anexo.FileName; //incluiremos no nome original da imagem, um novo “Guid”,
                                                                   //para que os nomes não se repitam na pasta, esta estratégia
                                                                   //visa evitar que haja sobreposição de 2 fotos com o mesmo
                                                                   //nome (obs: há N formas de fazer isso, isto é só um exemplo).

            var filePath = _filePath + "\\fotos"; //iremos concatenar no path uma pasta chamada “fotos”
                                                  //para que as fotos fiquem agrupadas dentro dela.

            if (!Directory.Exists(filePath)) //iremos validar se a pasta existe, caso não, iremos criá-la.
            {
                Directory.CreateDirectory(filePath);
            }

            using (var stream = System.IO.File.Create(filePath + "\\" + nome)) // vamos instanciar um “stream” para que faça o salvamento
                                                                               // da imagem na pasta indicada com o nome que indicamos no passo 1.
            {
                anexo.CopyToAsync(stream); //dentro do comando delimitado pela cláusula “using”,
                                           //utilizaremos o método to tipo de dado “IFormFile”
                                           //que faz a cópia do objeto chamado “CopyToAsync”
                                           //salvando-o no objeto instanciado como “stream”,
                                           //sendo assim a foto foi salva neste passo.
            }
            return nome; // retornamos o nome criado, já que ele foi gerado dentro do método,
                         // para que possa ser utilizado no salvamento do objeto “Pessoa” no banco de dados.
        }
    }
}
