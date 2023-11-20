using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DevIO.App.Extension
{
    #region HtmlTargetElement
    /*
     Podemos Definir aqui para que tipo esse TagHelper está atendendo:
     Ex:
     Se for para um link adicionaremos o "a" => [HtmlTargetElement("a")]
     Se for para uma Div adicionaremos a "Div" => [HtmlTargetElement("Div")]
     Se for para todo mundo adicionaremos o "*" => [HtmlTargetElement("*")]

     Também iremos definir o(s) Atributo(s) que iremos usar para configurar/decorar no html
     Ex:
      Attributes ="Attributes = "supress-by-claim-name"
      Attributes ="Attributes = "supress-by-claim-value"
     */
    #endregion
    #region  Regra de Exebição com base Nome e valor da Claim
    #region IDEIA DA CONFIGURAÇÃO ApagaElementoTagHelper
    /*
     A ideia aqui é esconder(não mostrar) o elemento(Div, Button, etc..) que estiver com essa configuração supress-by-claim-name e
     supress-by-claim-value no HTML para aqueles usuários que não tiverem logados, e mesmo logados precisam ter a Claim e valor da Claim
     necessária para ter o acesso. Nós atribuimos esse exemplo na Index de fornecedor, para a tag "a" que contem o link de exclusão
     ex:    
    <a
     class="btn btn-danger" supress-by-claim-name="Fornecedor" supress-by-claim-value="Excluir"
     asp-action="Delete" asp-route-id="@item.Id"><span class=" fa fa-trash"></span>
    </a>

    Na consiguração a cima estamos dizendo que para acessar esta funcionalidade o usuário precisar estar logado e e possuir a claim fornecedor
    com o valor Excluir.
          
    OBS: Não esquecer que a Claim é definida na tabela do AspNetUserClaims bem como os seu valores. para esse exemplo talvés precisaremos adicionar
         ou remover o valor da claim para vermos essa funcionalidade funcionar com o usuário logado.

         Podemos ver essa funcionalidade em ação quando acessarmos o projeto deslogado.      
     */
    #endregion

    [HtmlTargetElement("*", Attributes = "supress-by-claim-name")]
    [HtmlTargetElement("*", Attributes = "supress-by-claim-value")]
    public class ApagaElementoTagHelper : TagHelper
    {
        #region IHttpContextAccessor
        /*
            Injetamos o IHttpContextAccessor para que podemos pegar o usuário e realizarmos a validação no "temAcesso" para chamar o metodo
            que realiza a validação para saber se o usuário além de logado, tem a claims necessária.

            IHttpContextAccessor é o meio de acessar o contexto via http. Em qualquer lugar da nossa aplicação que quisermos acessar o Http para
            pegarmos o usuário logado podemos injetar esse IHttpContextAccessor
         */
        #endregion
        private readonly IHttpContextAccessor _contextAccessor;

        public ApagaElementoTagHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        #region HtmlAttributeName
        /* 
           Criamos dois tipode de propriedades(supress-by-claim-name, supress-by-claim-value, porque elas precisarão ser analisadas e são elas
           que usaremos para configurar/decorar o nosso html)
        */
        #endregion
        [HtmlAttributeName("supress-by-claim-name")]
        public string IdentityClaimName { get; set; }

        [HtmlAttributeName("supress-by-claim-value")]
        public string IdentityClaimValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            // O Context é aquilo que estamos recebendo na TagHelper
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            //output é a saída, é o que essa tagHelper vai produzir de conteudo
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            // Consultamos se o usuár
            var temAcesso = CustomAuthorization.ValidarClaimsUsuario(_contextAccessor.HttpContext, IdentityClaimName, IdentityClaimValue);

            //Caso tenha acesso não acontecerá nada
            if (temAcesso) return;

            // Caso não tenha acesso as tags não será geradas
            output.SuppressOutput();
        }
    }
    #endregion


    #region Regra de Exibição com base o Nome da Action
    #region IDEIA DA CONFIGURAÇÃO ApagaElementoByActionTagHelper
    /*
     A ideia aqui é com base no nome da Action iremos exibir ou não alguma coisa ou alguma tag na view. Por exemplo, nesse projeto deixamos um 
     botão de editar dentro da PartialView _DetalhesEndereco(propositalmente para mostrar essa funcionalidade) que por sua vez é exibida dentro
     da view de Detalhes(Action Details) de fornecedor. Queremos então não mostrar esse botão de editar endereço dentro da view responsável por
     visualizar os detalhe de Fornecedor.

     ex: _DetalhesEndereco, tg a responsavél por exibir o botão de editar.
    <a supress-by-action="Edit" asp-action="AtualizarEndereco"  asp-route-id="@Model.Id" class="btn btn-warning"
       data-modal=""> <span title="Editar" class="fa fa-pencil-alt"></span>
    </a>

     Se na view, a tag que possuir a configuração supress-by-action="Edit"  não estiver dentro de uma action cujo o nome é edit a tag não será
     exibida. Nesse caso a cima o botão de editar não deverá ser exibido porque a PartialView _DetalhesEndereco é exibida dentro da
     view Details de Fornecedor, cujo o nome da Action é Details
     
     */
    #endregion

    [HtmlTargetElement("*", Attributes = "supress-by-action")]
    public class ApagaElementoByActionTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public ApagaElementoByActionTagHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        [HtmlAttributeName("supress-by-action")]
        public string ActionName { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            var action = _contextAccessor.HttpContext.GetRouteData().Values["action"].ToString();

            if (ActionName.Contains(action)) return;

            output.SuppressOutput();
        }
    }
    #endregion


    #region Regra para desabilitar Link(Href)
    #region IDEIA DA CONGIFURAÇÂO  DesabilitaLinkByClaimTagHelper
    /*
     A ideia aqui é desabilitar todo link(href) que estiver dentro de uma tag "a" quando o usuário não tiver as permissões necessárias para
     acessar tal funcionalidade. Nós iremos adicionar a decoração/configuração no htlm utilizando a "disable-by-claim-name" e
     "disable-by-claim-value" dentro da tag "a", para esse exemplo utilizamos a decoração na view Index de fornecedor dentro da botão editar
      ex:
      <a class="btn btn-warning" disable-by-claim-name="Fornecedor" disable-by-claim-value="Editar"
         asp-action="Edit" asp-route-id="@item.Id"><span class=" fa fa-pencil-alt"></span>
      </a>

      Na consiguração a cima estamos dizendo que para acessar esta funcionalidade o usuário precisar estar logado e e possuir a claim fornecedor
      com o valor Editar.

      OBS: Não esquecer que a Claim é definida na tabela do AspNetUserClaims bem como os seu valores. para esse exmplo talvés precisaremos adicionar
      ou remover o valor da claim para vermos essa funcionalidade funcionar com o usuário logado

      Podemos ver essa funcionalidade em ação quando acessarmos o projeto deslogado.

     */
    #endregion

    [HtmlTargetElement("a", Attributes = "disable-by-claim-name")]
    [HtmlTargetElement("a", Attributes = "disable-by-claim-value")]
    public class DesabilitaLinkByClaimTagHelper : TagHelper
    {
        

        private readonly IHttpContextAccessor _contextAccessor;

        public DesabilitaLinkByClaimTagHelper(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        [HtmlAttributeName("disable-by-claim-name")]
        public string IdentityClaimName { get; set; }

        [HtmlAttributeName("disable-by-claim-value")]
        public string IdentityClaimValue { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            var temAcesso = CustomAuthorization.ValidarClaimsUsuario(_contextAccessor.HttpContext, IdentityClaimName, IdentityClaimValue);

            if (temAcesso) return;

            // Vai remover todos os href(link) da nossa view
            output.Attributes.RemoveAll("href");
            
            // Vai adicionar o atributo dentro desse link cursor: not-allowed que é o cursor que diz que não temos permisão de acesso
            output.Attributes.Add(new TagHelperAttribute("style", "cursor: not-allowed"));

            //Vai adicionar o atributto tittle dizendo que não temos permisão
            output.Attributes.Add(new TagHelperAttribute("title", "Você não tem permissão"));
        }
    }
    #endregion

}
