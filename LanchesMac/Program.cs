using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories;
using LanchesMac.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// fazendo a conexão com o banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Essa configuração é para estabelecer a conexão da aplicação com o banco de dados informado no Json

builder.Services.AddIdentity<IdentityUser, IdentityRole>() // aqui estou configurando o Identity para gerenciar a autenticação e autorização dos usuários
    .AddEntityFrameworkStores<AppDbContext>()// aqui estou dizendo que o Identity vai usar o AppDbContext para armazenar os dados dos usuários
    .AddDefaultTokenProviders();

// há um código que você consegue colocar aqui na Program que permite eu o padrão da senha para que ela não precisa necessariamente ser forte, ou seja, sem números, caracteres especiais, etc, já que esse é um padrão do Identity, aí neste caso, preciso pesquisar na internet qual o código
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;
});

//fazendo a injeção de dependências
builder.Services.AddTransient<ILanchesRepository, LancheRepository>(); // aqui serve para fazer a injeção de dependência automaticamente no projeto, podemos injetar assim as dependências nos controllers
builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddTransient<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp)); // aqui estou adicionando o carrinho na sessão do usuário pelo Id, e o AddScoped também serve para que, ao invés de ser o AddTransient, o AddScoped funciona como uma chamada para cada requisição, e não na aplicação em si, no caso quando fizer uma requisição, o Transient lê como sendo igual porqu está na mesma aplicação, agora com o Scoped não, pois ele trata requisições como diferentes para cada sessão

// preciso dessa configuração para configurar as sessions
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//configurando as sessions na program
builder.Services.AddMemoryCache();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute( // aqui estou definindo o padrão da rota para filtrar os lanches por categoria ao invés de ficar utilizando o ?categoria=natural na url
    name: "categoriaFiltro",
    pattern: "Lanche/{action}/{categoria?}",
    defaults: new { Controller = "Lanche", action = "List" })
    .WithStaticAssets();

app.MapControllerRoute( // aqui é um padão de rota para as controllers, eles são praticamente parâmetros que são passados pela url para definir qual controller e ação serão chamadas
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapAreaControllerRoute(
    name: "areas",
    areaName: "Admin", // Adicione o nome da área aqui, por exemplo "Admin"
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
