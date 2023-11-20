using DevIO.Business.Interfaces;
using DevIO.Business.Models;
using DevIO.Business.Models.Validations;

namespace DevIO.Business.Services
{
    public class FornecedorService : BaseService, IFornecedorService
    {
        #region  OBS IFornecedorRepository, IEnderecoRepository
        /*
         Configuramos os Repository de fornecedor e endereço na classe FornecedorService para que as ações que modifiquem informações no banco
         sejam feitas por aqui. Na Controller de fornecedores nós iremos chamar o FornecedorService nas Action que faça qualquer população de
         informação no banco de dados, como por exemplo, Create e Edit
         */
        #endregion
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IEnderecoRepository _enderecoRepository;

        #region base(notificador)
        /*
         Como FornecedorService herda a classe BaseService e a BaseService  exige um parametro do tipo INotificador no construtor da classe,
         precisamos instanciar no construtor da FornecedorService um parameto do tipo INotificador e passar para a clase BaseSerivice.
         */
        #endregion
        public FornecedorService(IFornecedorRepository fornecedorRepository,
                                 IEnderecoRepository enderecoRepository,
                                 INotificador notificador) : base(notificador)
        {
            _fornecedorRepository = fornecedorRepository;
            _enderecoRepository = enderecoRepository;
        }

        public async Task Adicionar(Fornecedor fornecedor)
        {
            #region  Validando no Modo Fácil
            /*
            Dentro do FornecedorValidation existe uma metodo do tipo ValidationResult onde o ValidationResult possui vários atributos que 
            podemos usar para validar, uma delas é uma lista de possíveis erros(public List<ValidationFailure> Errors) encontrados de um obj
            ao validar  com base na validação que criamos na class FornecedorValidation. Podemos pegar essa lista e tratarmos da forma que quisermos,
            porém para esse projeto iremos fazer a validação Através de lançamento de eventos. Passando essa resposabilidade para o BaseService
            que é herdado tando no FornecedorService quando no ProdutoService
            
            var validator = new FornecedorValidation();
            var result = validator.Validate(fornecedor);

            if( !result.IsValid )
            {
                result.Errors;
            }
            */

            #endregion

            
            if (!ExecutarValidacao(new FornecedorValidation(), fornecedor)
                || !ExecutarValidacao(new EnderecoValidation(), fornecedor.Endereco)) return;

            if (_fornecedorRepository.Buscar(f => f.Documento == fornecedor.Documento).Result.Any())
            {
                Notificar("Já existe um fornecedor com este documento infomado.");
                return;
            }

            await _fornecedorRepository.Adicionar(fornecedor);
        }

        public async Task Atualizar(Fornecedor fornecedor)
        {
            if (!ExecutarValidacao(new FornecedorValidation(), fornecedor)) return;

            if (_fornecedorRepository.Buscar(f => f.Documento == fornecedor.Documento && f.Id != fornecedor.Id).Result.Any())
            {
                Notificar("Já existe um fornecedor com este documento infomado.");
                return;
            }

            await _fornecedorRepository.Atualizar(fornecedor);
        }

        public async Task AtualizarEndereco(Endereco endereco)
        {
            if (!ExecutarValidacao(new EnderecoValidation(), endereco)) return;

            await _enderecoRepository.Atualizar(endereco);
        }

        public async Task Remover(Guid id)
        {
            if (_fornecedorRepository.ObterFornecedorProdutosEndereco(id).Result.Produtos.Any())
            {
                
                Notificar("O fornecedor possui produtos cadastrados!");
                return;
            }

            #region _enderecoRepository
            /*
            
            var endereco = await _enderecoRepository.ObterEnderecoPorFornecedor(id);

            if (endereco != null)
            {
                await _enderecoRepository.Remover(endereco.Id);
            }

            */

            #endregion

            var endereco = await _enderecoRepository.ObterFornecedorEnderecoPorFonecedor(id);

            if (endereco != null)
            {
                await _enderecoRepository.Remover(endereco.Id);
            }

            await _fornecedorRepository.Remover(id);
        }

        public void Dispose()
        {
            _fornecedorRepository?.Dispose();
            _enderecoRepository?.Dispose();
        }
    }
}

