using Microsoft.AspNetCore.Mvc.Razor;

namespace DevIO.App.Extension
{
    public static class RazorExtensions
    {
        #region OBS
        /*Criamos uma classe com exetensão do Razor onde podemos adicionar tudo que quisermos extender para o 
         * Razor que será sempre através de metodos de extenção portnado uma class estatica.
         * Note que estamos usando a biblioteca do Razor.
         
          Metodo responsável por formatar o numero do documento conforme o seu tipo. Feito isso, basta irmos na
          view onde queremos utilizar e declaramos @using DevIO.App.Extension(logo na primeira linha da pagina)
          na view. Usaremos na view Index de fornecer. e em seguida basta adicionar a configuração.
          ex:
            @using DevIO.App.Extension (primeira linha da view)
            @this.FormataDocumento(item.TipoFornecedor, item.Documento)
         */
        #endregion
        public static string FormataDocumento(this RazorPage page,int tipoPessoa, string documento)
        {
            return tipoPessoa == 1 ? Convert.ToUInt64(documento).ToString(@"000\.000\.000\-00") :
                                     Convert.ToUInt64(documento).ToString(@"00\.000\.000\/0000\-00");
        }
    }
}
