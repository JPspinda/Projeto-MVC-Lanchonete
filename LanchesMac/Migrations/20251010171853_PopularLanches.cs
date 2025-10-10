using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LanchesMac.Migrations
{
    /// <inheritdoc />
    public partial class PopularLanches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Lanches(CategoriaId, DescricaoCurta, DescricaoDetalhada, EmEstoque, ImagemThumbbailUrl, ImagemUrl, IsLanchePreferido, Nome, Preco)" +
                "VALUES(1, 'Pão, hamburger, ovo, presunto, queijo e batata palha', 'Delicioso pão de hamburger com ovo frito; presunt e queijo de primeira qualidade acompanhada com batata palha.', 1, 'http://www.macoratti.net/Imagens/lanches/cheesesalada1.jpg', 'http://www.macoratti.net/Imagens/lanches/cheesesalada1.jpg', 0, 'Cheese Salada', 12.50)");
            migrationBuilder.Sql("INSERT INTO Lanches(CategoriaId, DescricaoCurta, DescricaoDetalhada, EmEstoque, ImagemThumbbailUrl, ImagemUrl, IsLanchePreferido, Nome, Preco)" +
                "VALUES(1, 'Pão, presunto, mussarela e tomate', 'Delicioso pão francês quentinho com presunto e mussarela bem servidos com tomate preparado com carinho.', 1, 'http://www.macoratti.net/Imagens/lanches/mistoquente4.jpg', 'http://www.macoratti.net/Imagens/lanches/mistoquente4.jpg', 0, 'Misto Quente', 8.00)");
            migrationBuilder.Sql("INSERT INTO Lanches(CategoriaId, DescricaoCurta, DescricaoDetalhada, EmEstoque, ImagemThumbbailUrl, ImagemUrl, IsLanchePreferido, Nome, Preco)" +
                "VALUES(1, 'Pão, hamburger, presunto, mussarela e batata palha', 'Delicioso pão de hamburger de nossa preparação e presunto e mussarela; acompanha batata palha.', 1, 'http://www.macoratti.net/Imagens/lanches/cheeseburger1.jpg', 'http://www.macoratti.net/Imagens/lanches/cheeseburger1.jpg', 0, 'Cheese Burger', 11.00)");
            migrationBuilder.Sql("INSERT INTO Lanches(CategoriaId, DescricaoCurta, DescricaoDetalhada, EmEstoque, ImagemThumbbailUrl, ImagemUrl, IsLanchePreferido, Nome, Preco)" +
                "VALUES(2, 'Pão integral, queijo branco, peito de peru, cenoura, alface, iogurte', 'Delicioso pão natural com queijo branco, peito de peru e cenoura ralada com alface picado e iogurte natural.', 1, 'http://www.macoratti.net/Imagens/lanches/lanchenatural.jpg', 'http://www.macoratti.net/Imagens/lanches/lanchenatural.jpg', 1, 'Lanche Salada Peito de Peru', 15.00)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Lanches");
        }
    }
}
