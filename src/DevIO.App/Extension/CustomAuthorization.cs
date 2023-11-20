using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DevIO.App.Extension
{
    public class CustomAuthorization
    {
        #region ValidarClaimsUsuario
        /*
         Valida se o usuário está autenticado(logado) e se além de logado, ele possui as claims necessárias para realizar o acesso.

         "Valida as Claims do usuário com base se o usuário possui autenticação e se ele possui as Claims com o valor que nós queremos."
        */
        #endregion
        public static bool ValidarClaimsUsuario(HttpContext context, string claimName, string claimValue)
        {
            return context.User.Identity.IsAuthenticated &&
                   context.User.Claims.Any(c => c.Type == claimName && c.Value.Contains(claimValue));
        }

    }

    /* Aqui é a configuração do Atributo que nós iremos utilizar na configuração/decoração do HTML que irá utilizar um filtro(RequisitoClaimFilter) 
     que iremos configurar.

    OBS: Lembrando que na convensão do Identity o "Attribute" do nome  ClaimsAuthorizeAttribute não será utilizado na decoração nos html, vamos
    utilizar penas o ClaimsAuthorize
    */
    public class ClaimsAuthorizeAttribute : TypeFilterAttribute
    {
        public ClaimsAuthorizeAttribute(string claimName, string claimValue) : base(typeof(RequisitoClaimFilter))
        {
            Arguments = new object[] { new Claim(claimName, claimValue) };
        }
    }


    // Configuração do filtro
    public class RequisitoClaimFilter : IAuthorizationFilter
    {
        private readonly Claim _claim;

        public RequisitoClaimFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Se não tiver autenticado redirecionaremos para a pagina responsavél por fazer a autenticação
            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area = "Identity", page = "/Account/Login", ReturnUrl = context.HttpContext.Request.Path.ToString() }));
                return;
            }
            
            // Caso já esteja autenticado, aqui verificaremos que ele possui a claim que esperamos, caso não enviaremos o CodeStatus 403(acesso negado)
            if (!CustomAuthorization.ValidarClaimsUsuario(context.HttpContext, _claim.Type, _claim.Value))
            {
                context.Result = new StatusCodeResult(403);
            }
        }
    }
}
