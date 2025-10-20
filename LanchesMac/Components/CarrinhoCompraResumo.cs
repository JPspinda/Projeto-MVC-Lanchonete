using LanchesMac.Models;
using LanchesMac.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Components
{
    public class CarrinhoCompraResumo : ViewComponent
    {
        private readonly CarrinhoCompra _carrinhoCompra;

        public CarrinhoCompraResumo(CarrinhoCompra carrinhoCompra)
        {
            _carrinhoCompra = carrinhoCompra;
        }

        public IViewComponentResult Invoke()
        {
            //var itens = _carrinhoCompra.GetCarrinhoCompraItens(); //Aqui estou pegando os itens do carrinho de compras

            var itens = new List<CarrinhoCompraItem>()
            {
                new CarrinhoCompraItem(),
                new CarrinhoCompraItem()
            };

            _carrinhoCompra.CarrinhoCompraItens = itens; //Aqui estou atribuindo os itens que peguei do carrinho de compras para a propriedade CarrinhoCompraItens da própria classe CarrinhoCompra

            var carrinhoCompraVM = new CarrinhoCompraViewModel //Aqui estou criando o ViewModel do carrinho de compras
            {
                CarrinhoCompra = _carrinhoCompra,
                CarrinhoCompraTotal = _carrinhoCompra.GetCompraCarrinhoTotal()
            };

            return View(carrinhoCompraVM); //Aqui estou retornando a View do componente com o carrinho de compras
        }
    }
}
