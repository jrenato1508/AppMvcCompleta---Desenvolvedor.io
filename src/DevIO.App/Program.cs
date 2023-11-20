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

#region  Config Notifica��o
/*
Removeemos daqui e adicionamos na class AddMvcConfiguration dentro da pasta Configuration

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 3; // Cofigura��o padr�o do tempo exebi��o
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopCenter; // Cofig. Da localiza��o onde ser� ebixido a notifica��o na tela.
});

*/
#endregion
#region Config MSG Global
/*
    Mensagens de valida��es dos tipos de valores que ser�o inseridos nos inputs,datetime e etc... 


    Removeemos daqui e adicionamos na class AddMvcConfiguration dentro da pasta Configuration


    builder.Services.AddControllersWithViews(o =>
    {
        o.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor((x, y) => "O valor preenchido � inv�lido para este campo.");
        o.ModelBindingMessageProvider.SetMissingBindRequiredValueAccessor(x => "Este campo precisa ser preenchido.");
        o.ModelBindingMessageProvider.SetMissingKeyOrValueAccessor(() => "Este campo precisa ser preenchido.");
        o.ModelBindingMessageProvider.SetMissingRequestBodyRequiredValueAccessor(() => "� necess�rio que o body na requisi��o n�o esteja vazio.");
        o.ModelBindingMessageProvider.SetNonPropertyAttemptedValueIsInvalidAccessor(x => "O valor preenchido � inv�lido para este campo.");
        o.ModelBindingMessageProvider.SetNonPropertyUnknownValueIsInvalidAccessor(() => "O valor preenchido � inv�lido para este campo.");
        o.ModelBindingMessageProvider.SetNonPropertyValueMustBeANumberAccessor(() => "O campo deve ser num�rico");
        o.ModelBindingMessageProvider.SetUnknownValueIsInvalidAccessor(x => "O valor preenchido � inv�lido para este campo.");
        o.ModelBindingMessageProvider.SetValueIsInvalidAccessor(x => "O valor preenchido � inv�lido para este campo.");
        o.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(x => "O campo deve ser num�rico.");
        o.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(x => "Este campo precisa ser preenchido.");
    });
*/
#endregion
#region  AddRazorRuntimeCompilation
/*  Configura��o do RazorRuntimeCompilation.Para n�o ter necessidade de parar a aplica��o para alterar uma pagina html
    � necess�rio instalar o pacote Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation na mesma vers�o .Net usada no projeto

Removeemos daqui e adicionamos na class AddMvcConfiguration dentro da pasta Configuration

builder.Services.AddRazorPages()
    .AddRazorRuntimeCompilation();

*/
#endregion
builder.Services.AddMvcConfiguration();


#region CONFIG INJE��O DEPENDENCIA
/*
 OBS:(pensamento meu) Acho que o projeto DevIO.App consegue enxergar as Interfaces IFornecedorRepository,
     IProdutosRepository e IEnderecoRepository que est�o no projeto(ClassLibery) DevIO.Bussines atrav�s
     da referencia de projeto DevIO.Data. Se formos nas refer�ncias do projeto DevIO.App, iremos encontrar
     o projeto DevIO.Data que foi ferecenciado por n�s, ao abrir o dropDown da referencia do DevIo.Data
     iremos enxergar o projeto DevIo.Bussines
 

Criamos uma class denteo de Configurations com o nome DependencyInjectionConfig para declarar as nossas inje��es de dependendia e deixar a
class Program mais Organizada. Feito isso basta chamar a classe de configura��o aqui
    ex: builder.Services.ResolveDependencies();

**builder.Services.AddScoped<MeuDbContext>();
**builder.Services.AddScoped<IFornecedorRepository, FornecedorRepository>();
**builder.Services.AddScoped<IProdutoRepository, ProdutosRepository>();
**builder.Services.AddScoped<IEnderecoRepository, EnderecoRepository>();
  // DI para fazer funcionar o nossa valida��o da moeda
**builder.Services.AddSingleton<IValidationAttributeAdapterProvider, MoedaValidationAttributeAdapterProvider>();


*/
#endregion
builder.Services.ResolveDependencies();


#region CONFIG AUTOMAPPER
/*
  Declara��o do Automapper, responsavel por mapear as ViewModels da camada de apresenta��o(DevIo.App) com a
  Model da camada de neg�cios(DevIo.Bussines)
 */
#endregion
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

#region CONFIGURA��O TRATAMENTO DE ERRO
#region OBS
/*
 Adicionamos essa configura��o de erro para o ambiente de Desenvolvimento e para os ambientes diferentes de desenvolvimento. Para completar a
 a configura��o, adicionamos algum metodos na HomeController para quando receber um numero de erro passarmos determinada mensagem. Esse erro s�
 Ser� exibido quando o ambiente for diferente de desenvolvimento.
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
     O UseHsts � um Middleware que ele adiciona o street transport security no header da requisi��o. Ent�o quando n�s pergarmos o nosso request
     e examinar atrav�s do inpect(F12), conseguimos ver que l� no header ele ir� passar esse street transport security.

     O que ele est� dizendo com isso?
     O Hsts ele � uma implementa��o de seguran�a que os browser mordernos suport�o  onde se voc� tentar uma conex�o n�o segura(http) ele vai te
     for�ar a seguir por uma conex�o segura. Se voc� n�o tiver uma conex�o segura, voc� vai receber um erro. Essa que � a �deia do HSTS
     */
    #endregion
    #region OBS02
    /*
     Se quisermos configurar o HSTS e n�o temos confi��es de subir um novo c�digo na aplica��o ou se quisermos fazer de outra maneira mesmo �
     s� criar um arquivo web.config � adicionar 
     */
    #endregion
    app.UseHsts(); 
}
#endregion

#region OBS UseHttpsRedirection
/*
  O UseHttpsRedirection � um Middleware, ele faz a mesma coisa que o Hsts. A fun��o dele aqui � redirecionar de HTTP para HTTPS. Assim como o 
  HSTS s� que tem um detalhe. Para o HSTS funcionar, o primeio acesso precisa ser em HTTPS(a� � que entra o  UseHttpsRedirection). Dessa forma 
  o Broser vai entender e guardar a informa��o que a aplica��o s� vai conversar via HTTPS. Essa informa��o fica guardada no cooking e pode ser
  perdida  quando limparem o cache do navegador ou ent�o at� expirar(30 dias). � Por isso que geralmente essas duas configura��es andam juntas.
  Se o primeioro acesso n�o for via HTTPS o HSTS n�o ir� funcionar e o broserw n�o vai entender que a aplica��o s� conversa via https.
 */
#endregion

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

#region Config Globaliza��o
/*Criamos e passamos a configura��o para dentro da class GlobalizationConfig dentro pasta Configuration
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
