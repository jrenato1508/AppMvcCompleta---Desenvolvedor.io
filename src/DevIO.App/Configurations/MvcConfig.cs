using AspNetCoreHero.ToastNotification;
using Microsoft.AspNetCore.Mvc;

public static class MvcConfig
{
    public static IServiceCollection AddMvcConfiguration(this IServiceCollection services)
    {
        #region AddRazorRuntimeCompilation
        services.AddRazorPages()
                .AddRazorRuntimeCompilation();
        #endregion

        #region Config MSG Global
        services.AddMvc(o =>
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

            #region  OBS AutoValidateAntiforgeryTokenAttribute => ValidateAntiForgeryToken [Controllers]
            /*
             O AutoValidateAntiforgeryTokenAttribute valida automaticamente o token gerado nas view da aplicação diretamente nas controllers.
             Dessa forma, nas Action que causam alguma alteração de objeto no banco não precisaremos adicionar em cima da Action o o atributo
             ValidateAntiForgeryToken. Nesse nossa aplicação eu não removi o ValidateAntiForgeryToken das Action, mas decidir add essa informação
             aqui pra ficarmos ciente que essa configuração pode ser feita.
             */
            #endregion
            o.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
        });
        #endregion

        #region Config Notificação
        services.AddNotyf(config =>
        {
            config.DurationInSeconds = 3; // Cofiguração padrão do tempo exebição
            config.IsDismissable = true;
            config.Position = NotyfPosition.TopCenter; // Cofig. Da localização onde será ebixido a notificação na tela.
        });
        #endregion


        return services;
    }
}
