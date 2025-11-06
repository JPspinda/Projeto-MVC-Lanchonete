using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly CarrinhoCompra _carrinhoCompra;

        public PedidoController(IPedidoRepository pedidoRepository, CarrinhoCompra carrinhoCompra)
        {
            _pedidoRepository = pedidoRepository;
            _carrinhoCompra = carrinhoCompra;
        }

        [Authorize]
        [HttpGet] // se eu nao colocar esse atributo, o padrão já é o Get
        public IActionResult Checkout()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(Pedido pedido)
        {
            int totalItensPedido = 0;
            decimal precoTotalPedido = 0.0m;

            //Oter os itens de compra do carrinho
            List<CarrinhoCompraItem> itens = _carrinhoCompra.GetCarrinhoCompraItens();
            _carrinhoCompra.CarrinhoCompraItens = itens;

            //verifica se o carrinho está vazio
            if(_carrinhoCompra.CarrinhoCompraItens.Count == 0)
            {
                ModelState.AddModelError("", "Seu carrinho está vazio, que tal incluir um lanche?"); //O ModelState.AddModelError adiciona um erro de validação ao modelo, o primeiro parâmetro é o nome do campo que gerou o erro, se for uma validação geral do modelo, deixa vazio, e o segundo parâmetro é a mensagem de erro
            }

            //Calcular o total de itens e o total do pedido
            foreach(var item in itens)
            {
                totalItensPedido += item.Quantidade;
                precoTotalPedido += (item.Lanche.Preco * item.Quantidade);
            }

            //atribuir os valores obtidos ao pedido 
            pedido.TotalItensPedido = totalItensPedido;
            pedido.PedidoTotal = precoTotalPedido;

            //valida os dados
            if(ModelState.IsValid)
            {
                //cria o pedido e os detalhes
                _pedidoRepository.CriarPedido(pedido);
                //define mensagem de sucesso e limpa o carrinho
                ViewBag.CheckoutCompletoMensagem = "Obrigado pelo seu pedido :)"; // a viewbag é um objeto dinâmico que permite passar dados do controller para a view de forma simples e flexível
                ViewBag.TotalPedido = _carrinhoCompra.GetCompraCarrinhoTotal();
                _carrinhoCompra.LimparCarrinho();
                return View("~/Views/Pedido/CheckoutCompleto.cshtml", pedido);
            }

            return View(pedido);
        }
    }
}
