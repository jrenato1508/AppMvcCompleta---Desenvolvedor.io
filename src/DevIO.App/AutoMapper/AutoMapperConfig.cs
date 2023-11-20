using AutoMapper;
using DevIO.App.ViewModels;
using DevIO.Business.Models;
using DevIO.Data.Mapping;

namespace DevIO.App.AutoMapper
{
    #region  OBS Profile
    /*
      a Herança Profile é do AutoMapper. Essa classe vai ser uma classe de configuração de um perfil de mapeamento
     */
    #endregion
    public class AutoMapperConfig : Profile
    {
        #region  COMENTÁRIO ReverseMap()
        /*
          As configurações do Automapper é um caminho de mão única, ou seja. Precisaremos configurar
          por exemplo, fornecer é representado por FornecedorViewModel e FornecedorViewModel é
          representado por fornecer. Antigamente era criado dois perfis, um indo e outro voltado,
          agora podemos usar o ReverseMap(); desde que o processo de trasformação deja o mesmo
          ou seja, não haja um metodo construtor  parametrizado dentre as class que estamos fazendo
          as configurações

        OBS:(pensamento meu) Acho que o projeto DevIO.App consegue enxergar as class Fornecedor
     Produtos e Endereco que estão no projeto(ClassLibery) DevIO.Bussines através
     da referencia de projeto DevIO.Data. Se formos nas referências do projeto DevIO.App, iremos encontrar
     o projeto DevIO.Data que foi ferecenciado por nós, ao abrir o dropDown da referencia do DevIo.Data
     iremos enxergar o projeto DevIo.Bussines
         */
        #endregion
        public AutoMapperConfig() 
        { 
            CreateMap<Fornecedor, FornecedorViewModel>().ReverseMap();
            CreateMap<Produto, ProdutoViewModel>().ReverseMap();
            CreateMap<Endereco, EnderecoViewModel>().ReverseMap();
        }

       // OBS: A classe AutoMapperConfig é a clase de mapeamento do nosso Automapper
    }
}
