using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.Business.Models
{
    public abstract class Entity
    {
        #region ## Comentários##
        /*
        Obs1: A class Entity será do tipo abstract porque só poderá ser herdada pelas outras classes(Fornecedor, 
              Endereco e Produto) que iremos implementar.Ela terá um atributo Id do tipo Guid.

        Obs2: O metodo contrustor da classe será um protected e não public para que a apenas as classes que herdarem
              a classe Entity tenham acesso.
        
        Obs3: Toda vez uma classe que herde da classe Entity for estanciada ela já terá setada um novo guid criado
              Aleatoriamente através da implementação Id= Guid.NewGuid();
         */
        #endregion
        public Guid Id { get; set; }

        public Entity()
        {
            Id = Guid.NewGuid();
        }
    }
}
