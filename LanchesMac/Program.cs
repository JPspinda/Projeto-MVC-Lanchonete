using LanchesMac.Areas.Admin.Servicos;
using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories;
using LanchesMac.Repositories.Interfaces;
using LanchesMac.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddPaging(options =>
{
    options.ViewName = "Bootstrap4"; // aqui estou definindo o tema do Bootstrap 4 para a paginação
    options.PageParameterName = "pageindex"; // aqui estou definindo o nome do parâmetro da página na URL
});

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
builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>(); // aqui estou fazendo a injeção de dependência para o serviço de inicialização de usuários e papéis
builder.Services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp)); // aqui estou adicionando o carrinho na sessão do usuário pelo Id, e o AddScoped também serve para que, ao invés de ser o AddTransient, o AddScoped funciona como uma chamada para cada requisição, e não na aplicação em si, no caso quando fizer uma requisição, o Transient lê como sendo igual porqu está na mesma aplicação, agora com o Scoped não, pois ele trata requisições como diferentes para cada sessão
builder.Services.AddScoped<RelatorioVendaService>();
builder.Services.AddScoped<GraficoVendasService>();

// preciso dessa configuração para configurar as sessions
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
    {
        policy.RequireRole("Admin");
    }); // aqui estou criando uma política de autorização chamada "Admin" que exige que o usuário tenha o papel de "Admin"
});

//configurando as sessions na program
builder.Services.AddMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(7); // por exemplo
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<ConfigurationImage>(builder.Configuration.GetSection("ConfigurationPastaImagens")); // aqui estou configurando a seção ConfigurationImage do appsettings.json para ser injetada na aplicação

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
app.UseStaticFiles();

CriarPerfisUsuario(app); // aqui estou chamando o método para criar os papéis e usuários iniciais

app.UseSession();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute( // aqui estou definindo o padrão da rota para filtrar os lanches por categoria ao invés de ficar utilizando o ?categoria=natural na url
    name: "categoriaFiltro",
    pattern: "Lanche/{action}/{categoria?}",
    defaults: new { Controller = "Lanche", action = "List" })
    .WithStaticAssets();

app.MapControllerRoute( // aqui é um padão de rota para as controllers, eles são praticamente parâmetros que são passados pela url para definir qual controller e ação serão chamadas
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

void CriarPerfisUsuario(WebApplication app) // aqui estou chamando o método para criar os papéis e usuários iniciais
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>(); // aqui estou criando um escopo para o serviço
    using (var scope = scopedFactory.CreateScope()) // aqui estou criando o escopo
    {
        var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>(); // aqui estou obtendo o serviço de inicialização de usuários e papéis
        service.SeedRoles(); // aqui estou chamando o método para criar os papéis
        service.SeedUsers(); // aqui estou chamando o método para criar os usuários
    }
}