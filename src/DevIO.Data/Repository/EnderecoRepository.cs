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
    public class EnderecoRepository : Repository<Endereco>, IEnderecoRepository
    {
        public EnderecoRepository(MeuDbContext db) : base(db){}

        public async Task<Endereco> ObterFornecedorEnderecoPorFonecedor(Guid fornecedorId)
        {
            return await _context.Enderecoes.AsNoTracking()
                .FirstOrDefaultAsync(f => f.FornecedorId == fornecedorId);
                
        }
    }
}
