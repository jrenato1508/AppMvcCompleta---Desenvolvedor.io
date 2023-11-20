using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using DevIO.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.Data.Repository
{
    #region Comentário IMPORTANTE : base(db)
    /* 
     Comentário da classe Repository
     Esse metodo contrutor irá causar um problema nas 3 classes(FornecedorRepository,ProdutoRepository,EnderecoRepository)
     que herdarão o repository, visto que a classe Repository recebe uma assinatura(parametro) no metodo construtor. Tendo
     em conta que a clase se inicia através do metodo construtor, as classes que herdarem Repository, terão que passar para
     a clase base(Repository) a assinatura do metodo.
     Ex: public produtosRepository(MeuDbContext db) : base(db) {}
         public FornecedorRepository(MeuDbContext db) : base(db) {}
         public EnderecoRepository(MeuDbContext db) : base(db) {}
     */
    #endregion
    public class ProdutosRepository : Repository<Produto>, IProdutoRepository
    {
        #region  Comentário  base(db)
        

        #endregion
        public ProdutosRepository(MeuDbContext db) : base(db){}

        public async Task<Produto> ObterProdutoFornecedor(Guid id)
        {
            #region Comentário
            /*
             O _context(Dbcontex) está disponivel aqui por a class produtorepository é herdada de repository

            A ideia da Query a baixo é ir em produtos(_context.Produtos) e fazer um join em fornecedores (f => f.Fornecedor)
            onde o produto id(p => p.Id == id) é igual ao id indormado no metodo(GUID id)
             */

            #endregion

            return await _context.Produtos.AsNoTracking().Include(f => f.Fornecedor)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Produto>> ObterProdutosFornecedores()
        {
            return await _context.Produtos.AsNoTracking().Include(p => p.Fornecedor)
                .OrderBy(p => p.Nome).ToListAsync();
        }

        public async Task<IEnumerable<Produto>> ObterProdutoPorFornecedor(Guid FornecedorId)
        {
            return await Buscar(p => p.FornecedorId == FornecedorId);
        }
    }
}
