using Microsoft.AspNetCore.Localization;
using System.Globalization;

public static class GlobalizationConfig
{
    public static IApplicationBuilder UseGlobalizationConfig(this IApplicationBuilder app)
    {
        #region  OBS Globalização
        /*Configuração Necessário para a projeto ser pt-br. Quando o projeto não tem uma cultura definida ele pega a cultura da
         maquina/Sevidor. Nesse caso, por mais que o ambiente de desenvolvimento esteja com a cultura BR, não sabemos se o servidor
         em quem o projeto estará alocado está em Pt-Br então por via das duvidas sempre bom deixar configurado

        Feito isso, precisaremos configurar o arquivo que faz uso do javascript para fazer as validações e aceitar essa configuração
         */
        #endregion
        var defaultCulture = new CultureInfo("pt-BR");
        var localizationOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(defaultCulture),
            SupportedCultures = new List<CultureInfo> { defaultCulture },
            SupportedUICultures = new List<CultureInfo> { defaultCulture }
        };

        app.UseRequestLocalization(localizationOptions);

        return app;
    }
}
