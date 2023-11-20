using DevIO.Business.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.Data.Context
{
    public class MeuDbContext: DbContext
    {
        public MeuDbContext(DbContextOptions options):base(options)
        {
            #region OBS
            /*
             Tivemos um problema para salvar a alterações feita na action produto e essa foi a solução.
             Para mais explarecimento consultar OBS ERRO Na ProdutoController linha 170
             */
            #endregion
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public DbSet<Fornecedor> Fornecedores { get; set;}
        public DbSet<Produto> Produtos { get; set;}
        public DbSet<Endereco> Enderecoes { get; set;}

        #region COMENTÁRIO
        /*OBS1: Esse Metodo vai olhar para todas as entidades que estão mapeadas no DBContext(ex: DbSet<produo>)
        e vai buscar aquelas  entidades que herdem de IEntityTypeConfiguration que estão relacionadas no
        DbContext e registrará todas as entidades de uma vez.
        
         OBS2: Esse comando vai procurar por relações(relationship) dentro do modelBuilder pegando o tipo das
        nossas entidades(GetEntityTypes) dando um selectMany que resultará em uma lista onde através 
        das foreignKeys que foram identificadas que existe relação entre as entidades vai pegar dentro
        dessa relação o deletebehavior que é o comportamento após a exclusão e vai ser setada como
        DeleteBehavior.ClientSetNull(vai ser nulo). Poderia ser Cascade,Restrict. Dessa forma estamos
        empedindo que uma classe que esta sendo representado por uma tabela no banco ao excluir esse 
        item ele leve os filhos juntos

        OBS3: Toda propriedade do tipo string de cada entidade vai ser relacionada com o tipo de coluna
        varchar(100) caso não haja nenhuma configuração feita via FluentApi
        
         */
        #endregion
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //OBS1
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MeuDbContext).Assembly);
            
            
            //OBS2
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys())) relationship.DeleteBehavior = DeleteBehavior.ClientSetNull;

            //OBS3
            foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties()
            .Where(p => p.ClrType == typeof(string))))
                property.SetColumnType("Varchar(100)");
            

            base.OnModelCreating(modelBuilder);
        }


        #region SaveChangesAsync
        /*
          Quando o metodo SaveChangesAsync for chamado nas class Service, esse metodo irá consultar o estado da entidade, se o estado for de
          added(Adicionar) ele irá procurar a propriedade com o nome "DataCadastro" dendo ta tabela que esta sendo utilizado, caso ache ele irá
          adicionar a data atual da ação. E a mesma coisa acontece quando o estado da entidade estiver como Modified(Modificado).
         */
        #endregion
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null))
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now; 
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
