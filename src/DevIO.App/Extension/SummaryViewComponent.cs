using DevIO.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.App.Extension
{
    public class SummaryViewComponent: ViewComponent
    {
        private readonly INotificador _notificador;
        public SummaryViewComponent(INotificador notificador)
        {
            _notificador = notificador;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Obtemos todas as notificações encontradas durante a validação da entidade na camada de negócios
            var notificacoes = await Task.FromResult(_notificador.ObterNotificacoes());

            //Com o ForEach vamos colocar cada mensagem de erro dentro da ModelState como se fosse um erro de model. Ou seja, ele irá tratar no
            // formulário como se fosse um erro de preenchimento de campo só que sem um campo especifico
            notificacoes.ForEach(c => ViewData.ModelState.AddModelError(string.Empty, c.Mensagem));

            return View();
        }
    }
}

