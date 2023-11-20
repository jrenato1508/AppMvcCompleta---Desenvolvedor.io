using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using DevIO.App.Extension;

namespace DevIO.App.ViewModels
{
    public class ProdutoViewModel
    {

        #region  Comentário
        /*
            public Guid FornecedorId { get; set; }
        
         O FornecedorId não será necessário porque aqui nessa models nós não estaremos fazendo as cofigurações das colunas do
        baco de dados igual fizemos no modelo AppMvcBasica, aqui será a configuração das models apenas para exebição das
        informações na view. O FornecedorId tem dentro de fornecedores(ultimo atributo dessa class).

        Mas agora precisaremos informar  id do Produto visto que essa classe será uma classe de representação e ela não 
        possui herança com entity que seta o id automaticamente.
        public Guid Id { get; set; }
        */

        #endregion

        #region KEY
        // Para o controle e entendimento da view vamos dizer que esse atributo é uma chave para saber que
        // ID será uma chave e não um campo a ser gerado via Scafold
        #endregion

        [Key]
        public Guid Id { get; set; }

        [DisplayName("Fornecedor")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid FornecedorId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(200, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Nome { get; set; }

        [DisplayName("Descrição")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(1000, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Descricao { get; set; }

        #region  OBS IMAGEM
        /*
         O campo imagem está sendo mapeado como propriedade que se refere a propriedade imagem da nossa entidade, porém,
         para trabalharmos com Upload de Arquivos o campo imagem não pode ser um string. Para o campo imagem receber um
         campo de upload precisaremos trabalhar de uma outra forma, mas não podemos perder o campo imagem porque ele vai
         continuar mapeado o banco de dados. Então iremos duplicar esse campo sendo que um deles vai ser do tipo IFormFile.
         Mais um exemplo do OBS do item 12 do txt.
         
        OBS: Não tem problema a classe ProdutoViewModel não refleti fielmente a classe Produto desde que ela cumpra o papel dela
        de exibir coisas na tela
         */
        #endregion
        [DisplayName("Imagem do Produto")]
        public IFormFile ImagemUpload { get; set; }

        public string Imagem { get; set; }

        [Moeda]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public decimal Valor { get; set; }

        #region ScaffoldColumn
        // Na hora do scafold para gerar as telas essa campo será ignorado         
        #endregion

        [ScaffoldColumn(false)]
        public DateTime DataCadastro { get; set; }

        [DisplayName("Ativo?")]
        public bool Ativo { get; set; }

        /*relacionamento do EntityFramework*/
        public FornecedorViewModel Fornecedor { get; set; }

        public IEnumerable<FornecedorViewModel> Fornecedores { get; set; }
    }
}
