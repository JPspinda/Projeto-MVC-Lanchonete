using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories;
using LanchesMac.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// fazendo a conexão com o banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Essa configuração é para estabelecer a conexão da aplicação com o banco de dados informado no Json

//fazendo a injeção de dependências
builder.Services.AddTransient<ILanchesRepository, LancheRepository>(); // aqui serve para fazer a injeção de dependência automaticamente no projeto, podemos injetar assim as dependências nos controllers
builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
