using DevIO.Business.Notifiacoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.Business.Interfaces
{
    public interface INotificador
    {
        //Validar se tem notificação e retornará true ou false
        bool TemNotificacao();

        // Obter as notificações retornando uma lista com os erros e notificações
        List<Notificacao> ObterNotificacoes();

        //Manipular uma notificação quando ela for lançada
        void Handle(Notificacao notificacao);
    }
}
