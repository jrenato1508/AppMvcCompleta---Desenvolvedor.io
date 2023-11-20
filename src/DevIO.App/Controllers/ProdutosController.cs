using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DevIO.App.Data;
using DevIO.App.ViewModels;
using DevIO.Business.Interfaces;
using AutoMapper;
using DevIO.Business.Models;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Authorization;
using DevIO.App.Extension;

namespace DevIO.App.Controllers
{
    #region [Autorize]
    /*
     O Autorize em cima da classe da Controller siguinifica que estamos restringindo o acesso a informações dessa controller para quem não tiver
     autorizado que nesse caso serão os usuários que não estão autenticados no sistema.
     */
    #endregion

    [Authorize]
    [Route("Produtos")]
    public class ProdutosController : BaseController
    {
        #region IProdutoRepository
        /*
         Declaramos o nosso IProdutoRepository para termos um meio de acesso a dados. injetamos eles via construtor
         para que todas vez que a classe for chamada já temos a IProdutoRepository estaciado e pronto para o uso
         */
        #endregion

        #region IProdutoService
        /*
         Implementamos o ProdutoService que ficará responsavel por realizar modificações no banco de dados como exluir, alterar e salvar.
         é uma boa prática esconder os metodos que alterem os status do obj no banco de dados. Para leituras simples de informações,isto é, aquelas
         consultas de informações no banco que não necessite ser tratada ou aplicada alguma regra de negocio para serem exibidas podemos continuar
         utilizando o IProdutoRepository. Caso necessite tratar as informações que vem do banco antes de exibir na view, é melhor fazer essa manipulação
         de informações dentro da camda de negocios na class ProdutoService e etc...
         */
        #endregion

        #region OBSERVAÇÃO
        /*Anotar a observação que ele está fazendo sobre a lista de fornecedores que teremos que trazer no index.


        PAREI NO MINUTO 21:20 - Já construi toda a nossa controller, amanhã a gente volta pro minuto 21h20 pra rever a aula e anotar
        o que não foi anotado

         */
        #endregion

        #region base(notificador)
        /*
         Como ProdutosController herda a classe BaseController e a BaseController  exige um parametro do tipo INotificador no construtor da classe,
         precisamos instanciar no construtor da ProdutosController um parameto do tipo INotificador e passar para a clase BaseController.
         */
        #endregion

        private readonly IProdutoRepository _produtoRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IMapper _mapper;
        private readonly IProdutoService _produtoService;

        public ProdutosController(IProdutoRepository produtoRepository,
                                  IFornecedorRepository fornecedorRepository,
                                  IMapper mapper,
                                  IProdutoService produtoService,
                                  INotificador notificador) : base(notificador)
        {
            _produtoRepository = produtoRepository;
            _fornecedorRepository = fornecedorRepository;
            _mapper = mapper;
            _produtoService = produtoService;
        }

        #region [AllowAnonymous]
        /*
         AllowAnonymous em cima da action siguinifica que estamos permitindo que os usuários que não estejam autorizados que no caso serão
         os usuários que não estejam autenticados no sistema tenham acesso as informações dessa action

         OBS: Vale lembrar que essa Controller esta com o [Authorize], que só é pemitido o acesso ao metodos dessa controller quem estiver
         autenticado. o AllowAnonymous é uma exceção para o usuário ter acesso a determinada action
         */
        #endregion
        [AllowAnonymous]
        [Route("lista-de-produtos")]
        public async Task<IActionResult> Index()
        {
            #region OBS IMPORTANTE
            /* 
            Para gerar uma lista de fornecedores criamos mais um atributo na pasta ProdutoViewModel que retornará uma lista
            de fornecedores.
              ex: public IEnumerable<FornecedorViewModel> Fornecedores { get; set; }

           */
            #endregion


            return View(_mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterProdutosFornecedores()));
        }

        #region [AllowAnonymous]
        /*
         AllowAnonymous em cima da action siguinifica que estamos permitindo que os usuários que não estejam autorizados que no caso serão
         os usuários que não estejam autenticados no sistema tenham acesso as informações dessa action

         OBS: Vale lembrar que essa Controller esta com o [Authorize], que só é pemitido o acesso ao metodos dessa controller quem estiver
         autenticado. o AllowAnonymous é uma exceção para o usuário ter acesso a determinada action
         */
        #endregion
        [AllowAnonymous]
        [Route("dados-do-produto/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var produtoViewModel = await ObterProduto(id);

            if (produtoViewModel == null) return NotFound();

            return View(produtoViewModel);
        }

        #region ClaimsAuthorize
        /*
          ClaimsAuthorize é a configuração para permitir o acesso para aqueles usuários que além de estarem autenticados, precisam ter a autorização
          especifica para ter acesso a essa action. Essa autorização especifica nós atribuimos na tabela AspNetUserClaims do nosso banco de dados.
          
          OBS: Passamos como paramtro da ClaimsAuthorize o Nome da Claim é o valor. Lembrando que uma Claim pode ter vários valores como por
               exemplo Adicionar, Editar e Excluir.

          OBS2: Muito cuidado ao utilizar as Claims para não exagerar nas restrinções. O Broserw gera um cooking que é passado no header de
                um reques. Imagina que o broserw gera um papel e pões no cabeçalho(header) as inforações no cooking e envia para nossa App.
                A nossa App por usa vez vai validar as informações desse header para saber se prosseque como requeste e etc..                   
        
                As Claims são persistidas no cooking e se tiver bastante restrinções na Claims como por exempo, Adicionar, editar,excluir,
                read, x, y,z... o cooking pode ficar muito grande, por ex, ele pode ficar com 200 kb, pensa que estamos trafegando 200kb no header
                de um request a cada request, agora mutiplica isso por 1000 request, isso pode ocacionar uma certa lentidão na aplicação e também 
                pode ocasionar uma dificuldade de gerenciar o numero de claims.
                
                DICA: Usar menos caracteres na Claim por ex ao invés de escrever Adicionar, escrevemos add e ou ad e etcc... Quanto menos informações
                      no cooking mais leve ele fica, lembre-se que ele é um arquivo texto
                
               
         */
        #endregion
        [ClaimsAuthorize("Produto","Adicionar")]
        [Route("novo-produto")]
        public async Task<IActionResult> Create()
        {
            #region COMENTÁRIO
            /*
             Metodo responsavél por buscar no banco a lista de fornecedores para ser exibida e selecionada no view
             no processo de criação do produto
             */
            #endregion
            var produtoViewModel = await PopularFornecedores(new ProdutoViewModel());
            return View(produtoViewModel);
        }


        #region ClaimsAuthorize
        /*
          ClaimsAuthorize é a configuração para permitir o acesso para aqueles usuários que além de estarem autenticados, precisam ter a autorização
          especifica para ter acesso a essa action. Essa autorização especifica nós atribuimos na tabela AspNetUserClaims do nosso banco de dados.
          
          OBS: Passamos como paramtro da ClaimsAuthorize o Nome da Claim é o valor. Lembrando que uma Claim pode ter vários valores como por
               exemplo Adicionar, Editar e Excluir.

          OBS2: Muito cuidado ao utilizar as Claims para não exagerar nas restrinções. O Broserw gera um cooking que é passado no header de
                um reques. Imagina que o broserw gera um papel e pões no cabeçalho(header) as inforações no cooking e envia para nossa App.
                A nossa App por usa vez vai validar as informações desse header para saber se prosseque como requeste e etc..                   
        
                As Claims são persistidas no cooking e se tiver bastante restrinções na Claims como por exempo, Adicionar, editar,excluir,
                read, x, y,z... o cooking pode ficar muito grande, por ex, ele pode ficar com 200 kb, pensa que estamos trafegando 200kb no header
                de um request a cada request, agora mutiplica isso por 1000 request, isso pode ocacionar uma certa lentidão na aplicação e também 
                pode ocasionar uma dificuldade de gerenciar o numero de claims.
                
                DICA: Usar menos caracteres na Claim por ex ao invés de escrever Adicionar, escrevemos add e ou ad e etcc... Quanto menos informações
                      no cooking mais leve ele fica, lembre-se que ele é um arquivo texto
                
               
         */
        #endregion
        [ClaimsAuthorize("Produto", "Adicionar")]
        [Route("novo-produto")]
        [HttpPost]
        [ValidateAntiForgeryToken]// Esse atributo exige que o token seja validado (Segurança)
        public async Task<IActionResult> Create(ProdutoViewModel produtoViewModel)
        {
            #region OBS
            /*
              produtoViewModel = await PopularFornecedores(produtoViewModel); esta buscado uma lista de fornecedores no banco
              e alocando na IEnumerable Fornecedores que criamos recentemente na ProdutoViewModel(OBS IMPORTANTE linha 50).
              Fornecedor que foi selecionado pelo usuário está alocado no atributo fornecedor e não na lista de fornecedores.
             */
            #endregion
            produtoViewModel = await PopularFornecedores(produtoViewModel);
            if (!ModelState.IsValid) return View(produtoViewModel);

            #region IMAGEM
            /*
             Toda imagem precisa ser única. Ou impedimos que o usuárioa suba as imagens no banco da mesma forma fazendo uma
             validação no banco para que não exista uma imagem com o mesmo nome na pasta antes de guardar no banco ou então 
             damos um nome que seja quase quase impossível que duas imagens tenham o mesmo nome(Esse que vamos fazer no código
             abaixo dando um GUID + Nome da imagem. Assim garantimos que a nossa imagem nunca vai se repetir
             */
            #endregion
            var imgPrefixo = Guid.NewGuid() + "_";

            if(!await UpLoadArquivo(produtoViewModel.ImagemUpload, imgPrefixo))
            {
                return View(produtoViewModel);
            }

            #region  produtoViewModel.Image
            /* Passando para o campo imagem o imgPrefixo + o nome do arquivo que foi inserido para  pelo usuário. Apenas esse
               dado que ficará salvo no banco de dados. Lembrando que o arquiso será gravado em disco.           
             */
            #endregion
            produtoViewModel.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;

            #region await _produtoRepository.Adicionar(_mapper.Map<Produto>(produtoViewModel));
            /*
               Subistituimos o _produtoRepository pelo _produtoService para esconder o Repository dos metodos da controller
               que realizem qualquer alteração da entidade no banco de dados.
             */ 
            #endregion
            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            #region OBS OperacaoValida
            /*
             O metodo OperacaoValida é da nossa BaseController que é herdada nessa clase. Ele consulta se foi adicionada alguma mensagem de 
             erro na lista durante a validação de entidade na camada de negocios(DevIO.Business). Se tiver, retornaremos 
             para a view o produtoviewmodel
             */
            #endregion
            if (!OperacaoValida()) View(produtoViewModel);

            return RedirectToAction("Index");
        }


        #region ClaimsAuthorize
        /*
          ClaimsAuthorize é a configuração para permitir o acesso para aqueles usuários que além de estarem autenticados, precisam ter a autorização
          especifica para ter acesso a essa action. Essa autorização especifica nós atribuimos na tabela AspNetUserClaims do nosso banco de dados.
          
          OBS: Passamos como paramtro da ClaimsAuthorize o Nome da Claim é o valor. Lembrando que uma Claim pode ter vários valores como por
               exemplo Adicionar, Editar e Excluir.

          OBS2: Muito cuidado ao utilizar as Claims para não exagerar nas restrinções. O Broserw gera um cooking que é passado no header de
                um reques. Imagina que o broserw gera um papel e pões no cabeçalho(header) as inforações no cooking e envia para nossa App.
                A nossa App por usa vez vai validar as informações desse header para saber se prosseque como requeste e etc..                   
        
                As Claims são persistidas no cooking e se tiver bastante restrinções na Claims como por exempo, Adicionar, editar,excluir,
                read, x, y,z... o cooking pode ficar muito grande, por ex, ele pode ficar com 200 kb, pensa que estamos trafegando 200kb no header
                de um request a cada request, agora mutiplica isso por 1000 request, isso pode ocacionar uma certa lentidão na aplicação e também 
                pode ocasionar uma dificuldade de gerenciar o numero de claims.
                
                DICA: Usar menos caracteres na Claim por ex ao invés de escrever Adicionar, escrevemos add e ou ad e etcc... Quanto menos informações
                      no cooking mais leve ele fica, lembre-se que ele é um arquivo texto
                
               
         */
        #endregion
        [ClaimsAuthorize("Produto", "Editar")]
        [Route("editar-produto/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {

            var produtoViewModel = await ObterProduto(id);

            if (produtoViewModel == null)  return NotFound();
            
            return View(produtoViewModel);
        }


        #region ClaimsAuthorize
        /*
          ClaimsAuthorize é a configuração para permitir o acesso para aqueles usuários que além de estarem autenticados, precisam ter a autorização
          especifica para ter acesso a essa action. Essa autorização especifica nós atribuimos na tabela AspNetUserClaims do nosso banco de dados.
          
          OBS: Passamos como paramtro da ClaimsAuthorize o Nome da Claim é o valor. Lembrando que uma Claim pode ter vários valores como por
               exemplo Adicionar, Editar e Excluir.

          OBS2: Muito cuidado ao utilizar as Claims para não exagerar nas restrinções. O Broserw gera um cooking que é passado no header de
                um reques. Imagina que o broserw gera um papel e pões no cabeçalho(header) as inforações no cooking e envia para nossa App.
                A nossa App por usa vez vai validar as informações desse header para saber se prosseque como requeste e etc..                   
        
                As Claims são persistidas no cooking e se tiver bastante restrinções na Claims como por exempo, Adicionar, editar,excluir,
                read, x, y,z... o cooking pode ficar muito grande, por ex, ele pode ficar com 200 kb, pensa que estamos trafegando 200kb no header
                de um request a cada request, agora mutiplica isso por 1000 request, isso pode ocacionar uma certa lentidão na aplicação e também 
                pode ocasionar uma dificuldade de gerenciar o numero de claims.
                
                DICA: Usar menos caracteres na Claim por ex ao invés de escrever Adicionar, escrevemos add e ou ad e etcc... Quanto menos informações
                      no cooking mais leve ele fica, lembre-se que ele é um arquivo texto
                
               
         */
        #endregion
        [ClaimsAuthorize("Produto", "Editar")]
        [Route("editar-produto/{id:guid}")]
        [HttpPost]
        [ValidateAntiForgeryToken] // Esse atributo exige que o token seja validado (Segurança)
        public async Task<IActionResult> Edit(Guid id, ProdutoViewModel produtoViewModel)
        {
            if(id != produtoViewModel.Id) return NotFound();

            var produtoAtualizacao = await ObterProduto(id);
            produtoViewModel.Fornecedor = produtoAtualizacao.Fornecedor;
            produtoViewModel.Imagem = produtoAtualizacao.Imagem;

            if (!ModelState.IsValid) return View(produtoViewModel);

            if(produtoViewModel.ImagemUpload != null)
            {
                var imgPrefixo = Guid.NewGuid() + "_";
                if(!await UpLoadArquivo(produtoViewModel.ImagemUpload, imgPrefixo))
                {
                    return View(produtoViewModel);
                }

                produtoAtualizacao.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;
            }
            #region OBS
            /*
             Para evitar uma possível falha ou vunerabilidade nós iremos atualizar tudo que foi adicioando pelo usuário na view
             e alterar o objeto que foi consultado no banco, assim não corremos risco de por exemplo tentarem alterar o ID do 
             usuário através do formulário que veio da view.
             */
            #endregion
            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;

            #region     OBS ERROR
            /*
             Tive um problema na hora de salvar a alteração do produto. a solução e explicação foi essa

            DÚVIDA ALTERAÇÃO DE PRODUTO - INVALIDOPERATIONEXCEPTION: THE INSTANCE OF ENTITY TYPE 'FORNECEDOR' CANNOT BE TRACKED 
            BECAUSE ANOTHER INSTANCE WITH THE SAME KEY VALUE FOR {'ID'} IS ALREADY BEING TRACKED
            Eu tive o mesmo problema e encontrei a solução no código do professor. Na classe "MeuDbContext" 
            adicione no construtor as duas linhas:

             public MeuDbContext(DbContextOptions options):base(options)
            {
                ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                ChangeTracker.AutoDetectChangesEnabled = false;
            }
             
            Parece que por algum motivo o EF Core marcou a entidade como "modified" mais de uma vez: "is already being tracked".
            Dessa forma não é necessário alterar a consulta no repositório.
             */
            #endregion

            #region  await _produtoRepository.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));
            /*
             Substituimos o produtoRepository pelo ProdutoService para esconder das controllers os metodos que fazez qualquer inserção ou alteração
             das informasções do objeto no banco de dado
             */
            #endregion
            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));

            #region OBS
            /*
             O metodo OperacaoValida é da nossa BaseController que é herdada nessa clase. Ele consulta se foi adicionada alguma mensagem de 
             erro na lista durante a validação de entidade na camada de negocios(DevIO.Business). Se tiver, retornaremos 
             para a view o produtoviewmodel
             */
            #endregion
            if (!OperacaoValida()) View(produtoViewModel);

            return RedirectToAction("Index");
        }


        #region ClaimsAuthorize
        /*
          ClaimsAuthorize é a configuração para permitir o acesso para aqueles usuários que além de estarem autenticados, precisam ter a autorização
          especifica para ter acesso a essa action. Essa autorização especifica nós atribuimos na tabela AspNetUserClaims do nosso banco de dados.
          
          OBS: Passamos como paramtro da ClaimsAuthorize o Nome da Claim é o valor. Lembrando que uma Claim pode ter vários valores como por
               exemplo Adicionar, Editar e Excluir.

          OBS2: Muito cuidado ao utilizar as Claims para não exagerar nas restrinções. O Broserw gera um cooking que é passado no header de
                um reques. Imagina que o broserw gera um papel e pões no cabeçalho(header) as inforações no cooking e envia para nossa App.
                A nossa App por usa vez vai validar as informações desse header para saber se prosseque como requeste e etc..                   
        
                As Claims são persistidas no cooking e se tiver bastante restrinções na Claims como por exempo, Adicionar, editar,excluir,
                read, x, y,z... o cooking pode ficar muito grande, por ex, ele pode ficar com 200 kb, pensa que estamos trafegando 200kb no header
                de um request a cada request, agora mutiplica isso por 1000 request, isso pode ocacionar uma certa lentidão na aplicação e também 
                pode ocasionar uma dificuldade de gerenciar o numero de claims.
                
                DICA: Usar menos caracteres na Claim por ex ao invés de escrever Adicionar, escrevemos add e ou ad e etcc... Quanto menos informações
                      no cooking mais leve ele fica, lembre-se que ele é um arquivo texto
                
               
         */
        #endregion
        [ClaimsAuthorize("Produto", "Excluir")]
        [Route("excluir-produto/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var produto = await ObterProduto(id);

            if(produto == null) return NotFound();

            return View(produto);
        }


        #region ActionName
        /*
          Apenas para fins de informação. O Meto Delete e DeleteConfirmed são metodos com nomes diferentes mas possuem a mesma assinatura. e um
          é um complemento do outro. o DeleteConfirmed precisa continuar respondendo pelo meto Delete e é por isso que ele tem
          um ActionName("Delete") para que ele continue respondendo pelo metodo Delete. Se não sivesse isso, o DeleteConfirmed seria um outro metodo
         */
        #endregion
        #region ClaimsAuthorize
        /*
          ClaimsAuthorize é a configuração para permitir o acesso para aqueles usuários que além de estarem autenticados, precisam ter a autorização
          especifica para ter acesso a essa action. Essa autorização especifica nós atribuimos na tabela AspNetUserClaims do nosso banco de dados.
          
          OBS: Passamos como paramtro da ClaimsAuthorize o Nome da Claim é o valor. Lembrando que uma Claim pode ter vários valores como por
               exemplo Adicionar, Editar e Excluir.

          OBS2: Muito cuidado ao utilizar as Claims para não exagerar nas restrinções. O Broserw gera um cooking que é passado no header de
                um reques. Imagina que o broserw gera um papel e pões no cabeçalho(header) as inforações no cooking e envia para nossa App.
                A nossa App por usa vez vai validar as informações desse header para saber se prosseque como requeste e etc..                   
        
                As Claims são persistidas no cooking e se tiver bastante restrinções na Claims como por exempo, Adicionar, editar,excluir,
                read, x, y,z... o cooking pode ficar muito grande, por ex, ele pode ficar com 200 kb, pensa que estamos trafegando 200kb no header
                de um request a cada request, agora mutiplica isso por 1000 request, isso pode ocacionar uma certa lentidão na aplicação e também 
                pode ocasionar uma dificuldade de gerenciar o numero de claims.
                
                DICA: Usar menos caracteres na Claim por ex ao invés de escrever Adicionar, escrevemos add e ou ad e etcc... Quanto menos informações
                      no cooking mais leve ele fica, lembre-se que ele é um arquivo texto
                
               
         */
        #endregion
        [ClaimsAuthorize("Produto", "Excluir")]
        [Route("excluir-produto/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Esse atributo exige que o token seja validado (Segurança)
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var produto = await ObterProduto(id);

            if (produto == null) return NotFound();

            #region await _produtoRepository.Remover(id)
            /*
              Subistituimos o await _produtoRepository.Remover(id) pelo _produtoService para qualquer metodo que realize
              alteração no banco seja feito pelo Service para esconder nosso Repository nos metodos de alteração.
            */
            #endregion
            await _produtoService.Remover(id);

            #region OBS
            /*
             O metodo OperacaoValida é da nossa BaseController que é herdada nessa clase. Ele consulta se foi adicionada alguma mensagem de 
             erro na lista durante a validação de entidade na camada de negocios(DevIO.Business). Se tiver, retornaremos 
             para a view o produtoviewmodel
             */
            #endregion
            if (!OperacaoValida()) return View(produto);

            #region OBS
            /*
             Estamos passando a mensagem para nosso TemData que é o único que sobrevive a um RedirectToAction para ser exibida na view index
             de produto através da nossa viewcomponet que chamamos ela através da tagHelper utilizando o <vc:Summary></vc:Summary>
             */
            #endregion
            TempData["Sucesso"] = "Produto Excluido com Sucesso";

            return RedirectToAction("Index");
        }


        #region  METODOS 
        /*Se quisermos futuramente passar esses metodos para uma outra classe para deixar a controller mais organizada.*/

        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            // Na variavel Produto já teremos o produto e o fornecedor do produto
            var produto = _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterProdutoFornecedor(id));

            // Mapeamos na lista que criamos agora(OBS IMPORTANTE,linha 50) uma lista de fornecedores
            produto.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
            return produto;
        }

        private async Task<ProdutoViewModel> PopularFornecedores(ProdutoViewModel produtoViewModel)
        {

            produtoViewModel.Fornecedores = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());
            return produtoViewModel;
        }

        private async Task<bool> UpLoadArquivo(IFormFile arquivo, string imgPrefixo)
        {
            if(arquivo.Length <= 0) return false;


            #region  OBS Path
            //Aqui vamos definor o caminho onde o arquivo será salvo. Será no wwwroot/imagens(VamosCriar)
            #endregion
            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/imagens", imgPrefixo + arquivo.FileName);

            #region  OBS System.IO
            // Validando se já existe um arquivo com o nome da imagem na pasta onde ser]ao guardados os arquivos.
            #endregion
            if (System.IO.File.Exists(path))
            {
                ModelState.AddModelError(string.Empty, "Já existe um arquivo com esse nome!");
                return false;
            }

            #region  OBS using
            //Aqui fazemos a gravação do arquivo no disco. O CopyToAsync copia a estancia que está arquivo para o disco
            //Passando o stream que é o path(diretório) e o tipo de ação que será create(FileMode.Create)
            #endregion
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            return true;
        }

        #endregion

    }
}
