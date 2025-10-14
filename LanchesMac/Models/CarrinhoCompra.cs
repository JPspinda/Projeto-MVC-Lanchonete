using LanchesMac.Context;
using Microsoft.EntityFrameworkCore;

namespace LanchesMac.Models
{
    public class CarrinhoCompra
    {
        private readonly AppDbContext _context;

        public CarrinhoCompra(AppDbContext context)
        {
            _context = context;
        }

        public string CarrinhoCompraId { get; set; }
        public List<CarrinhoCompraItem> CarrinhoCompraItens { get; set; }

        public static CarrinhoCompra GetCarrinho(IServiceProvider services)
        {
            //define um sessão
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;

            //obtem um serviço do tipo do nosso contexto
            var context = services.GetService<AppDbContext>();

            //obtem ou gera o id do carrinho
            string carrinhoId = session.GetString("CarrinhoId") ?? Guid.NewGuid().ToString();

            //atribui o id do carrinho na sessão
            session.SetString("CarrinhoId", carrinhoId);

            //retorna o carrinho com o contexto e o id atribuído ou obtido
            return new CarrinhoCompra(context)
            {
                CarrinhoCompraId = carrinhoId,
            };
        }

        public void AdicionarAoCarrinho(Lanche lanche)
        {
            var carrinhoCompraItem = _context.CarrinhoCompraItens
                .SingleOrDefault(s => s.Lanche.lancheId == lanche.lancheId &&
                s.CarrinhoCompraId == CarrinhoCompraId); // aqui irei verificar se o item que está querendo adicionar já existe ou não no carrinho por meio da tabela de carrinhoCompraItem
        
            if(carrinhoCompraItem == null)
            {
                carrinhoCompraItem = new CarrinhoCompraItem
                {
                    CarrinhoCompraId = CarrinhoCompraId,
                    Lanche = lanche,
                    Quantidade = 1
                };
                _context.CarrinhoCompraItens.Add(carrinhoCompraItem);
            }
            else
            {
                carrinhoCompraItem.Quantidade++;
            }

            _context.SaveChanges();
        }

        public void RemoverDoCarrinho(Lanche lanche) 
        {
            var carrinhoCompraItem = _context.CarrinhoCompraItens
                .SingleOrDefault(s => s.Lanche.lancheId == lanche.lancheId &&
                s.CarrinhoCompraId == CarrinhoCompraId); // verifico novamente se o lanche e o carrinho existe

            if(carrinhoCompraItem != null) // se tiver algo no carrinho entra neste if
            {
                if(carrinhoCompraItem.Quantidade > 1) // aqui, caso tenha mais de um item no carrinho eu só vou decrementar a quantidade e não excluir totalemente o CarrinhCompraItem
                {
                    carrinhoCompraItem .Quantidade--;
                }
                else // agora aqui sim
                {
                    _context.CarrinhoCompraItens.Remove(carrinhoCompraItem);
                }
            }

            _context.SaveChanges();
        }

        public List<CarrinhoCompraItem> GetCarrinhoCompraItens()
        {
            // aqui estou fazendo o seguinte, na primeira palavra, antes dos ?? ferá com que seja retornado a lista da própria classe, se ela não for nula, esses dois ?? significa
            // que, caso uma variável não for nula, retorne ela, caso ela for nula, retorne o que está a direita dos ??, que no caso é criando uma própria lista de carrinhoCompraItem
            // ou seja, está incluindo na variável CarrinhoCompraItens da própria classe uma lista de itens geradas automaticamente a partir da tabela do banco de dados
            return CarrinhoCompraItens ?? (CarrinhoCompraItens = _context.CarrinhoCompraItens.Where(c => c.CarrinhoCompraId == CarrinhoCompraId).Include(s => s.Lanche).ToList());
        }

        public void LimparCarrinho()
        {
            // nesta variável é retornado uma lista com todos os itens que fazem parte deste carrinho que está comparando
            var carrinhoItens = _context.CarrinhoCompraItens.Where(carrinho => carrinho.CarrinhoCompraId == CarrinhoCompraId);

            _context.CarrinhoCompraItens.RemoveRange(carrinhoItens);
        }

        public decimal GetCompraCarrinhoTotal()
        {
            //aqui ele está pesquisando na lista de carrinhoCompraItens da classe, primeiro, quais itens faz parte deste carrinho e depois já seleciona os preços vezes as quantidades de cada carrinhoItem
            var total = _context.CarrinhoCompraItens.Where(c => c.CarrinhoCompraId == CarrinhoCompraId).Select(c => c.Lanche.Preco * c.Quantidade).Sum();

            return total;
        }
    }
}
