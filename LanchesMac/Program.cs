using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddPaging(options =>
{
    options.ViewName = "Bootstrap4"; // aqui estou definindo o tema do Bootstrap 4 para a pagina��o
    options.PageParameterName = "pageindex"; // aqui estou definindo o nome do par�metro da p�gina na URL
});

// fazendo a conex�o com o banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Essa configura��o � para estabelecer a conex�o da aplica��o com o banco de dados informado no Json

builder.Services.AddIdentity<IdentityUser, IdentityRole>() // aqui estou configurando o Identity para gerenciar a autentica��o e autoriza��o dos usu�rios
    .AddEntityFrameworkStores<AppDbContext>()// aqui estou dizendo que o Identity vai usar o AppDbContext para armazenar os dados dos usu�rios
    .AddDefaultTokenProviders();

// h� um c�digo que voc� consegue colocar aqui na Program que permite eu o padr�o da senha para que ela n�o precisa necessariamente ser forte, ou seja, sem n�meros, caracteres especiais, etc, j� que esse � um padr�o do Identity, a� neste caso, preciso pesquisar na internet qual o c�digo
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 3;
    options.Password.RequiredUniqueChars = 1;
});

//fazendo a inje��o de depend�ncias
builder.Services.AddTransient<ILanchesRepository, LancheRepository>(); // aqui serve para fazer a inje��o de depend�ncia automaticamente no projeto, podemos injetar assim as depend�ncias nos controllers
builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddTransient<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>(); // aqui estou fazendo a inje��o de depend�ncia para o servi�o de inicializa��o de usu�rios e pap�is
builder.Services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp)); // aqui estou adicionando o carrinho na sess�o do usu�rio pelo Id, e o AddScoped tamb�m serve para que, ao inv�s de ser o AddTransient, o AddScoped funciona como uma chamada para cada requisi��o, e n�o na aplica��o em si, no caso quando fizer uma requisi��o, o Transient l� como sendo igual porqu est� na mesma aplica��o, agora com o Scoped n�o, pois ele trata requisi��es como diferentes para cada sess�o

// preciso dessa configura��o para configurar as sessions
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireRole("Admin");
    }); // aqui estou criando uma pol�tica de autoriza��o chamada "Admin" que exige que o usu�rio tenha o papel de "Admin"
});

//configurando as sessions na program
builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7); // por exemplo
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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

CriarPerfisUsuario(app); // aqui estou chamando o m�todo para criar os pap�is e usu�rios iniciais

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute( // aqui estou definindo o padr�o da rota para filtrar os lanches por categoria ao inv�s de ficar utilizando o ?categoria=natural na url
    name: "categoriaFiltro",
    pattern: "Lanche/{action}/{categoria?}",
    defaults: new { Controller = "Lanche", action = "List" })
    .WithStaticAssets();

app.MapControllerRoute( // aqui � um pad�o de rota para as controllers, eles s�o praticamente par�metros que s�o passados pela url para definir qual controller e a��o ser�o chamadas
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

void CriarPerfisUsuario(WebApplication app) // aqui estou chamando o m�todo para criar os pap�is e usu�rios iniciais
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>(); // aqui estou criando um escopo para o servi�o
    using (var scope = scopedFactory.CreateScope()) // aqui estou criando o escopo
    {
        var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>(); // aqui estou obtendo o servi�o de inicializa��o de usu�rios e pap�is
        service.SeedRoles(); // aqui estou chamando o m�todo para criar os pap�is
        service.SeedUsers(); // aqui estou chamando o m�todo para criar os usu�rios
    }
}