using LanchesMac.Context;
using LanchesMac.Models;
using Microsoft.EntityFrameworkCore;

namespace LanchesMac.Areas.Admin.Servicos
{
    public class RelatorioVendaService
    {
        private readonly AppDbContext _context;

        public RelatorioVendaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Pedido>> FindByDateAsync(DateTime? minDate, DateTime? maxDate)
        {
            var query = from obj in _context.Pedidos select obj;
            if (minDate.HasValue)
            {
                query = query.Where(x => x.PedidoEnviado >= minDate.Value);
            }
            if (maxDate.HasValue)
            {
                query = query.Where(x => x.PedidoEnviado <= maxDate.Value);
            }
            return await query
                        .Include(p => p.PedidoItens)
                        .ThenInclude(pd => pd.Lanche)
                        .OrderByDescending(p => p.PedidoEnviado)
                        .ToListAsync();
        }
    }
}
