using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")] // aqui estou definindo que apenas usuários com o role de Admin podem acessar esse controller
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
