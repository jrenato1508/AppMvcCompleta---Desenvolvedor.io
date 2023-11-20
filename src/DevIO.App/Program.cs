using DevIO.App.Configurations;
using DevIO.App.Data;
using DevIO.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

#region  CONFIG AMBIENTE
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();
#endregion


#region  CONFIG IDENTITY
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                                            options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();
#endregion

builder.Services.AddDbContext<MeuDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

#region  Config Notificação
/*
Removeemos daqui e adicionamos na class AddMvcConfiguration dentro da pasta Configuration

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 3; // Cofiguração padrão do tempo exebição
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopCenter; // Cofig. Da localização onde será ebixido a notificação na tela.
});

*/
#endregion
#region Config MSG Global
/*
    Mensagens de validações dos tipos de valores que serão inseridos nos inputs,datetime e etc... 


    Removeemos daqui e adicionamos na class AddMvcConfiguration dentro da pasta Configuration


    builder.Services.AddControllersWithViews(o =>
    {
        o.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => "O valor preenchido é inválido para este campo.");
        o.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(x => "Este campo precisa ser preenchido.");
        o.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => "Este campo precisa ser preenchido.");
        o.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => "É necessário que o body na requisição não esteja vazio.");
        o.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(x => "O valor preenchido é inválido para este campo.");
        o.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => "O valor preenchido é inválido para este campo.");
        o.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => "O campo deve ser numérico");
        o.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(x => "O valor preenchido é inválido para este campo.");
        o.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => "O valor preenchido é inválido para este campo.");
        o.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x => "O campo deve ser numérico.");
        o.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => "Este campo precisa ser preenchido.");
    });
*/
#endregion
#region  AddRazorRuntimeCompilation
/*  Configuração do RazorRuntimeCompilation.Para não ter necessidade de parar a aplicação para alterar uma pagina html
    É necessário instalar o pacote Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation na mesma versão .Net usada no projeto

Removeemos daqui e adicionamos na class AddMvcConfiguration dentro da pasta Configuration

builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();

*/
#endregion
builder.Services.AddMvcConfiguration();


#region CONFIG INJEÇÃO DEPENDENCIA
/*
 OBS:(pensamento meu) Acho que o projeto DevIO.App consegue enxergar as Interfaces IFornecedorRepository,
     IProdutosRepository e IEnderecoRepository que estão no projeto(ClassLibery) DevIO.Bussines através
     da referencia de projeto DevIO.Data. Se formos nas referências do projeto DevIO.App, iremos encontrar
     o projeto DevIO.Data que foi ferecenciado por nós, ao abrir o dropDown da referencia do DevIo.Data
     iremos enxergar o projeto DevIo.Bussines
 

Criamos uma class denteo de Configurations com o nome DependencyInjectionConfig para declarar as nossas injeções de dependendia e deixar a
class Program mais Organizada. Feito isso basta chamar a classe de configuração aqui
    ex: builder.Services.ResolveDependencies();

**builder.Services.AddScoped<MeuDbContext>();
**builder.Services.AddScoped<IFornecedorRepository, FornecedorRepository>();
**builder.Services.AddScoped<IProdutoRepository, ProdutosRepository>();
**builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>();
  // DI para fazer funcionar o nossa validação da moeda
**builder.Services.AddSingleton<IValidationAttributeAdapterProvider, MoedaValidationAttributeAdapterProvider>();


*/
#endregion
builder.Services.ResolveDependencies();


#region CONFIG AUTOMAPPER
/*
  Declaração do Automapper, responsavel por mapear as ViewModels da camada de apresentação(DevIo.App) com a
  Model da camada de negócios(DevIo.Bussines)
 */
#endregion
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

#region CONFIGURAÇÃO TRATAMENTO DE ERRO
#region OBS
/*
 Adicionamos essa configuração de erro para o ambiente de Desenvolvimento e para os ambientes diferentes de desenvolvimento. Para completar a
 a configuração, adicionamos algum metodos na HomeController para quando receber um numero de erro passarmos determinada mensagem. Esse erro só
 Será exibido quando o ambiente for diferente de desenvolvimento.
 */
#endregion
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/erro/500");
    app.UseStatusCodePagesWithRedirects("/erro/{0}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    #region OBS UseHsts
    /*
     O UseHsts é um Middleware que ele adiciona o street transport security no header da requisição. Então quando nós pergarmos o nosso request
     e examinar através do inpect(F12), conseguimos ver que lá no header ele irá passar esse street transport security.

     O que ele está dizendo com isso?
     O Hsts ele é uma implementação de segurança que os browser mordernos suportão  onde se você tentar uma conexão não segura(http) ele vai te
     forçar a seguir por uma conexão segura. Se você não tiver uma conexão segura, você vai receber um erro. Essa que é a ídeia do HSTS
     */
    #endregion
    #region OBS02
    /*
     Se quisermos configurar o HSTS e não temos confições de subir um novo código na aplicação ou se quisermos fazer de outra maneira mesmo é
     só criar um arquivo web.config é adicionar 
     */
    #endregion
    app.UseHsts(); 
}
#endregion

#region OBS UseHttpsRedirection
/*
  O UseHttpsRedirection é um Middleware, ele faz a mesma coisa que o Hsts. A função dele aqui é redirecionar de HTTP para HTTPS. Assim como o 
  HSTS só que tem um detalhe. Para o HSTS funcionar, o primeio acesso precisa ser em HTTPS(aí é que entra o  UseHttpsRedirection). Dessa forma 
  o Broser vai entender e guardar a informação que a aplicação só vai conversar via HTTPS. Essa informação fica guardada no cooking e pode ser
  perdida  quando limparem o cache do navegador ou então até expirar(30 dias). é Por isso que geralmente essas duas configurações andam juntas.
  Se o primeioro acesso não for via HTTPS o HSTS não irá funcionar e o broserw não vai entender que a aplicação só conversa via https.
 */
#endregion

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

#region Config Globalização
/*Criamos e passamos a configuração para dentro da class GlobalizationConfig dentro pasta Configuration
 * 
 * 
 * 

var defaultCulture = new CultureInfo("pt-BR");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new List<CultureInfo> { defaultCulture },
    SupportedUICultures = new List<CultureInfo> { defaultCulture }
};
app.UseRequestLocalization(localizationOptions);

 */
#endregion
app.UseGlobalizationConfig();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
