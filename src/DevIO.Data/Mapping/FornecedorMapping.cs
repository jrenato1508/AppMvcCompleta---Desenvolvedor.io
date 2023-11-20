using DevIO.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevIO.Data.Mapping
{
    public class FornecedorMapping : IEntityTypeConfiguration<Fornecedor>
    {
        public void Configure(EntityTypeBuilder<Fornecedor> builder)
        {
            builder.HasKey(f => f.Id);  // O entity iria tentender esse mapeamento na hora de gerar a migration, só estavmos fazendo por garantia

            builder.Property(f => f.Nome)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(f => f.Documento)
                .IsRequired()
                .HasColumnType("varchar(14)");

            // Configuração Relacional 1 : 1 => Fornecedor : Endereco
            builder.HasOne(f => f.Endereco)  // -Fornecedor tem um endereço
                .WithOne(e => e.Fornecedor); // -Endereco tem um fornecedor. Ligação uma pra um 

            // Configuração Relacional 1 : N => Fornecedor : Produtos (um fornecedor para muitos produtos)			
            builder.HasMany(f => f.Produtos) // Fornecedor tem muitos produtos(Hasmany)
                .WithOne(p => p.Fornecedor) // Produto Tem um fornecedor (whitone)
                .HasForeignKey(p => p.FornecedorId);  // Apontando que a ForeignKey de Fornecedor em produtos é o FornecedorId

            builder.ToTable("Fornecedores"); // NOME DA TABELA DEVE SER SEMPRE NO PLURAL
        }
    }
}

//PAREI NO COMANDO 10 iremos facilitar para que as camadas...