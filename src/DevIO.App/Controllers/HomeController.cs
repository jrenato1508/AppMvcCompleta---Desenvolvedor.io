using DevIO.App.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DevIO.App.Controllers
{
    public class HomeController : Controller
    {
        #region  OBS CUSTOMIZAÇÃO DA HOMECONTROLLER
        /*
         Nós modificamos a HomeController para tratamento de erros. Precisaremos mexer também na Class ErroViewModel para adicionar os
         atributos Mensagem,Titulo e ErroCode e também mexemos na View de Erro que fica dentro da pasta View => Shared => Erro.cshtml
         */
        #endregion
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Route("erro/{id:length(3,3)}")]
        public IActionResult Errors(int id)
        {
            var modelErro = new ErrorViewModel();
            if (id == 500) //	Erro do Servidor Interno
            {
                modelErro.Mensagem = "Ocorreu um erro! Tente novamente mais tarde ou contate nosso suporte.";
                modelErro.Titulo = "Ocorreu um erro!";
                modelErro.ErroCode = id;
            }
            else if (id == 404) //Não encontrado
            {
                modelErro.Mensagem = "A página que está procurando não existe! <br />Em caso de dúvidas entre em contato com nosso suporte";
                modelErro.Titulo = "Ops! Página não encontrada.";
                modelErro.ErroCode = id;
            }
            else if (id == 403) //Proibido
            {
                modelErro.Mensagem = "Você não tem permissão para fazer isto.";
                modelErro.Titulo = "Acesso Negado";
                modelErro.ErroCode = id;
            }
            else if (id == 503) //Serviço Indisponível
            {
                modelErro.Mensagem = "Ops! O Serviço parece está temporariamente indisponível. <br /> Tente mais tarde ou entre em contato com nosso suporte ";
                modelErro.Titulo = "serviço temporariamente indisponível.";
                modelErro.ErroCode = id;
            }
            else
            {
                return StatusCode(500);
            }

            return View("Error", modelErro);
        }
    }
}