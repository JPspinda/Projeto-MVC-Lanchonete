using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Controllers
{
    public class LancheController : Controller
    {
        private readonly ILanchesRepository _lanchesRepository;

        public LancheController(ILanchesRepository lanchesRepository)
        {
            _lanchesRepository = lanchesRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult List(string categoria) // estou pegando este parâmetro da própria url do site, ou seja, todos os parâmetros que estõs nos métodos
                                                    // das controllers podem ser pegos diretamente da url, neste caso o link fica mais ou menos assim:
                                                    // https://localhost:7004/Lanche/List?categoria=natural, perceba que, após a url padrão, tem um ponto de interrogação
                                                    // este ponto passa um valor ao parâmetro da categoria
        {
            //ViewData["Titulo"] = "Todos os Lanches"; // o ViewData armazena como se fosse um dicionário
            //ViewData["Data"] = DateTime.Now;

            //var lanches = _lanchesRepository.Lanches;
            //var totalLanches = lanches.Count();

            //ViewBag.Total = "Total de lanches: "; // já o ViewBag armazena como se fosse uma classe e suas propriedades
            //ViewBag.TotalLanches = totalLanches;

            //var lancheListViewModel = new LancheListViewModel();
            //lancheListViewModel.Lanches = _lanchesRepository.Lanches;
            //lancheListViewModel.CategoriaAtual = "Categoria Atual";

            IEnumerable<Lanche> lanches;
            string categoriaAtual = string.Empty;

            if (string.IsNullOrEmpty(categoria))
            {
                lanches = _lanchesRepository.Lanches.OrderBy(l => l.lancheId);
                categoriaAtual = "Todos os Lanches";
            }
            else
            {
                //if (string.Equals("Normal", categoria, StringComparison.OrdinalIgnoreCase)) // o Equals compara duas strings, o OrdinalIgnoreCase faz com que a comparação não seja case sensitive
                //{
                //    lanches = _lanchesRepository.Lanches
                //        .Where(l => l.Categoria.categoriaNome.Equals("Normal")) // o Where é usado para fazer filtros em coleções
                //        .OrderBy(l => l.Nome);
                //    categoriaAtual = "Lanches Normais";
                //}
                //else
                //{
                //    lanches = _lanchesRepository.Lanches
                //        .Where(l => l.Categoria.categoriaNome.Equals("Natural"))
                //        .OrderBy(l => l.Nome);
                //    categoriaAtual = "Lanches Naturais";
                //}
                lanches = _lanchesRepository.Lanches
                    .Where(l => l.Categoria.categoriaNome.Equals(categoria, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(l => l.Nome);

                categoriaAtual = categoria;
            }

            var lancheListViewModel = new LancheListViewModel
            {
                Lanches = lanches,
                CategoriaAtual = categoriaAtual
            };

            return View(lancheListViewModel);
        }

        public IActionResult Details(int lancheId)
        {
            var lanche = _lanchesRepository.Lanches
                .FirstOrDefault(l => l.lancheId == lancheId);
            return View(lanche);
        }
    }
}
