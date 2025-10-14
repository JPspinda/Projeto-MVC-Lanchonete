using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories;
using LanchesMac.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// fazendo a conex�o com o banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); // Essa configura��o � para estabelecer a conex�o da aplica��o com o banco de dados informado no Json

//fazendo a inje��o de depend�ncias
builder.Services.AddTransient<ILanchesRepository, LancheRepository>(); // aqui serve para fazer a inje��o de depend�ncia automaticamente no projeto, podemos injetar assim as depend�ncias nos controllers
builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp)); // aqui estou adicionando o carrinho na sess�o do usu�rio pelo Id, e o AddScoped tamb�m serve para que, ao inv�s de ser o AddTransient, o AddScoped funciona como uma chamada para cada requisi��o, e n�o na aplica��o em si, no caso quando fizer uma requisi��o, o Transient l� como sendo igual porqu est� na mesma aplica��o, agora com o Scoped n�o, pois ele trata requisi��es como diferentes para cada sess�o

// preciso dessa configura��o para configurar as sessions
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
