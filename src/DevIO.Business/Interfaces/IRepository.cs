using DevIO.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.Business.Interfaces
{
    #region  COMENTÁRIO IDisposable
    /*
     OBS:Esse repositorio por padrão terá implementado a interface Idisposable que é para obrigar que ele 
         faça o dispose para liberar a memória e também ele vai ser específico ou seja onde onde essa
         TEntity só possa ser utilizadas se ela for  uma classe filha de Entity(where TEntity : Entity).
         então nesse repositório não poderia passar qualquer coisa.

    OBS2: Vamos trabalhar com metodos assicronos para garantir uma melhor performace e uma melhor saude do
          nosso servidor
	
	OBS3: Lembre-se que a camada DevIO.Data conhece a camada DevIO.Business porque precisamos adicinar as 
          referencias da camda DevIO.Business na camada Data para configurarmos o nosso DbSet do dbcontext
     */
    #endregion
    public interface IRepository<TEntity> :IDisposable where TEntity : Entity
    {
        Task Adicionar(TEntity entity);
        Task<TEntity> ObterPorId(Guid id);
        Task<List<TEntity>> ObterTodos();
        Task Atualizar(TEntity entity);
        Task Remover(Guid id);
        
        //Iremos trabalhar com o metodo buscar onde passaremos uma expression que trabalhará com uma expressão
        //lamba que irá comparar e entidade e retornará um bolean
        Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate);
        Task<int> SaveChanges();

        #region COMENTÁRIO
        /*
         OBS: Ai a Pergunta, Mas por que o IRepository está na camada de Negocios(DevIO.Business)
         e não de dados(DevIO.Data).
         R: Porque a Camada de Negocios(DevIO.Business) não conhece a camada
         de Dados(DevIO.Data), porém a cadama de dados conhece a camada de negocios. Existe uma referencia
         da camada de negocios adicionada na camada de dados. Então a única forma delas conversarem será
         através dessa interface via injeção de dependecia.
        */
        #endregion
    }
}
