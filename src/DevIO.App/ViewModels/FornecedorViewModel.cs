using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DevIO.App.ViewModels
{
    public class FornecedorViewModel
    {
        #region KEY
        // Para o controle e entendimento da view vamos dizer que esse atributo é uma chave para saber que
        // ID será uma chave e não um campo
        #endregion

        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(14, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 11)]
        public string Documento { get; set; }

        #region  TipoFornecedor

        //public TipoFornecedor TipoFornecedor { get; set; }
        #endregion

        [DisplayName("Tipo")]
        public int TipoFornecedor { get; set; }

        [DisplayName("Ativo?")]
        public bool Ativo { get; set; }

        /* Entity FrameWorks Relations */
        public EnderecoViewModel Endereco { get; set; }

        public IEnumerable<ProdutoViewModel> Produtos { get; set; }
    }
}
