using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using DevIO.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.Data.Repository
{
    #region COMENTÁRIO
    /*
     A nossa class Repository será do tipo abstract que só poderá ser herdada por outras class, além disso
     ela será generica (Repository<TEntity>) e implementaremos a interface de
     IRepository<TEntity>(DevIO.Business) onde o TEntity do IRepository tem que ser filha de Entity,
     e depois disso implementar o contrato IRepository*/
    #endregion
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
    {
        protected readonly MeuDbContext _context;
        protected readonly DbSet<TEntity> _DbSet;

        public Repository( MeuDbContext db)
        {
            _context = db;
            _DbSet = db.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> Buscar(Expression<Func<TEntity, bool>> predicate)
        {
            /*O metodo irá ao banco de dados para aquela entidade especifica onde a expressão(predicate) que passarmos
            retornará uma lista assicrona*/
            return await _DbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        public virtual async Task<TEntity> ObterPorId(Guid id)
        {
            return await _DbSet.FindAsync(id);
        }

        public virtual async Task<List<TEntity>> ObterTodos()
        {
            return await _DbSet.ToListAsync();
        }
        public virtual async Task Adicionar(TEntity entity)
        {
            _context.Add(entity); // Guarda o obj na memória para ficar disponivél pra salvar
            await SaveChanges();
        }

        public virtual async Task Atualizar(TEntity entity)
        {
            _context.Update(entity);  // Guarda o obj na memória para ficar disponivél pra salvar
            await SaveChanges();
        }

        public virtual async Task Remover(Guid id)
        {
            #region COMENTÁRIO
            /*Metodo que vai no banco consultar o obj através do ID para só depois remover
            DbSet.Remove(await DbSet.FindAsync(id)); */

            /*Metod que passa um objeto de forma generica com o id que será informado.
             Para esse metodo funcionar, precisará ser adicionado a palavra new() na declaração da class Repository
             Ex: public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
             Dessa maneira não precisamos ir no banco buscar o obj através do ID informado para só depois excluir o obj.
             Através de um obj generico informamos que o id que foi informado é igual o id do obj generico criado.
             */
            #endregion
            _DbSet.Remove(new TEntity { Id = id });
            await SaveChanges();
        }

        public async Task<int> SaveChanges()
        {
            #region COMENTÁRIO
            /* Retorna a quantidade de linhas afetadas
             * Concentramos a chamada desse metodo em nos outros metodos para que se por ventura viermos ter que modifica esse
               codigo, alterando aqui surtirá o feito em todos os outros lugares que ele for chamado, tendo assim uma maior
               reotilidade do código. 
             */
            #endregion
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
