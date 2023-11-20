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
using System.Drawing;
using DevIO.Business.Models;
using AspNetCoreHero.ToastNotification.Abstractions;
using DevIO.Data.Repository;
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
    [Route("Fornecedores")]
    public class FornecedoresController : BaseController
    {
        #region IFornecedorRepository
        /*
         Declaramos o nosso IFornecedorRepository para termos um meio de acesso a dados. injetamos eles via construtor
         para que todas vez que a classe for chamada já temos a IFornecedorRepository estaciado e pronto para o uso.
          
         OBS: Configuramos também a injeção de dependencia na nossa class Program ou então em nossa class de configuração
         DependencyInjectionConfig
         
         */
        #endregion

        #region IMapper
        /*
         Utilizamos o AutoMapper para configurar a relação das Models utilizadas na Camada App(ViewModels) e camada
         Bussines(Models). Mas também precisaremos usar aqui na controlle para realizar uma conversão. Repare que
         nosso metodo ObterTodos() dentroo da task index retorna uma lista de fornecedores e
         nossa view Index de fornecedor a nosa model está apontando para ForncededorViewModel ex:
         @model IEnumerable<DevIO.App.ViewModels.FornecedorViewModel> (Primeira linha da pagina index)
         
         Desas forma precisaremos realizar a conversão para conseguirmos exibir as informações que serão retonadars desse metodo
         e vice e versa.

         */
        #endregion

        #region INotyfService 
        /*
         O INotyfService é uma implementação de notificação que implementamos onde é exibida uma mensagem de sucesso, erro, aviso e etc..
         
         Para implementar devemos seguir os seguintes passos:
         
         1- Baixar o pacote Nuget AspNetCoreHero.ToastNotification para a camada DevIo.App

         2- Configurar na Class Program:
            builder.Services.AddNotyf(config =>
            {
                config.DurationInSeconds = 3;
                config.IsDismissable = true;
                config.Position = NotyfPosition.TopCenter;
            });
         
         3- Configurar na view que você deseja utilizar. 
            OBS:Em nosso caso configurei na _Layout porque é a nossa view padrão que usaremos em todo o projeto

            Ex:@await Component.InvokeAsync("Notyf") <!--Config notificação-->

         4- Configurar na controller que iremos utilizar. 
            - Criando o private readonly INotyfService _notfy;
            - Injetando na controller
               ex: public FornecedoresController(INotyfService _notfy;)
                   {
                        _notfy = notyf;
                   }
            ex: private readonly INotyfService _notfy;

         5- Depois é só chamar _notfy com o tipo de mensagem que você quer passar adicionando os parametros.

            _notfy.Success("Sucess Notification");
            _notfy.Success("Sucess Notification thar closes in 4 Secondes",4);

            _notfy.Error("Some Error Notification");
            _notfy.Warning("Some Warning Notification");
            _notfy.Information("Some Information,close in 5 Secondes",5);

            _notfy.Custom("Some Custom Notification");
            _notfy.Custom("Some Custom Notification, close in 5 Seconds,",5,"whitesmoke","fa fa-gear");
            _notfy.Custom("Some Custom Notification, close in 7 Seconds,", 5, "#B600F", "fa fa-hone");
                
         */
        #endregion

        #region IFornecedorService
        /*
         Implementamos o FornecedorService que ficará responsavel por realizar modificações no banco de dados como exluir, alterar e salvar.
         é uma boa prática esconder os metodos que alterem o status  do obj no banco de dados. Para leituras de informações simples, isto é,
         aquelas consultas de informações no banco que não necessite ser tratada ou aplicada alguma regra de negocio para serem exibidas
         podemos continuar utilizando o IFornecedorService. Caso necessite tratar as informações que vem do banco antes de exibir na view,
         é melhor fazer essa manipulação de informações dentro da camda de negocios na class ProdutoService e etc...
         */
        #endregion

        #region base(notificador)
        /*
         Como FornecedoresController herda a classe BaseController e a BaseController  exige um parametro do tipo INotificador no construtor da classe,
         precisamos instanciar no construtor da FornecedoresController um parameto do tipo INotificador e passar para a clase BaseController.
         */
        #endregion

        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IMapper _mapper;
        //private readonly IEnderecoRepository _enderecoRepository;
        private readonly INotyfService _notfy;
        private readonly IFornecedorService _fornecedorService;

        public FornecedoresController(IFornecedorRepository fornecedorRepository,
                                      //IEnderecoRepository endereco,
                                      IMapper mapper,
                                      INotyfService notyf,
                                      IFornecedorService fornecedorService,
                                      INotificador notificador) :base(notificador) 
        {
            _fornecedorRepository = fornecedorRepository;
            //_enderecoRepository = endereco;
            _mapper = mapper;
            _notfy = notyf;
            _fornecedorService = fornecedorService;
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
        [Route("lista-de-fornecedores")]
        public async Task<IActionResult> Index()
        {
            #region  Conversão usando o Mapper
            /*
             Conforme dito na OBS IMapper(linha25) Uma vez que fizemos a configuração do mapeamento das nossas models(pastaModels)
             podemos fazer a conversão da lista de fonecedores que retorna através do Obter todos, para FornecedorViewModel que é
             a model da nossa classe index
             */
            #endregion

            

            return View(_mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos()));
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
        [Route("dados-do-fornecedor/{id:guid}")]
        #region OBS
        //public async Task<IActionResult> Details(Guid? id) - Não precisamos de um Guid nulo por que nunca vai ser null
        #endregion
        public async Task<IActionResult> Details(Guid id)
        {

            var fornecedorViewModel = await ObterFornecedorEndereco(id);
            if (fornecedorViewModel == null)
            {
                return NotFound();
            }

            return View(fornecedorViewModel);
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
        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        [Route("novo-fornecedor")]
        public IActionResult Create()
        {
            return View();
        }


        #region OBS
        //Tela do post/Envio das informações preenchidas na tela para salvar no banco
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
        [ClaimsAuthorize("Fornecedor", "Adicionar")]
        [Route("novo-fornecedor")]
        [HttpPost]
        [ValidateAntiForgeryToken] // Esse atributo exige que o token seja validado (Segurança)
        public async Task<IActionResult> Create(FornecedorViewModel fornecedorViewModel)
        {
            if (!ModelState.IsValid) return NotFound();

            var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);
            
            //await _fornecedorRepository.Adicionar(fornecedor);
            await _fornecedorService.Adicionar(fornecedor);

            if (!OperacaoValida()) return View(fornecedorViewModel);

            return RedirectToAction("Index");
        }


        #region OBS
        // Metodo Responsavel para fornecer a view de Edição
        //public async Task<IActionResult> Edit(Guid? id) Não precisamos de um Guid nulo por que nunca vai ser null
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
        [ClaimsAuthorize("Fornecedor", "Editar")]
        [Route("editar-fornecedo/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {

            var fornecedorViewModel = await ObterFornecedorProdutosEndereco(id);

            if (fornecedorViewModel == null) return NotFound();

            return View(fornecedorViewModel);
        }


        #region OBS
        //Tela do post/Envio das informações preenchidas na tela para salvar no banco
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
        [ClaimsAuthorize("Fornecedor", "Editar")]
        [Route("editar-fornecedor/{id:guid}")]
        [HttpPost]
        [ValidateAntiForgeryToken] // Esse atributo exige que o token seja validado (Segurança)
        public async Task<IActionResult> Edit(Guid id, FornecedorViewModel fornecedorViewModel)
        {
            if (id != fornecedorViewModel.Id) return NotFound();
            
            if(!ModelState.IsValid) 
            {
                _notfy.Error("Ops...Algo Aconteceu");
                return View(fornecedorViewModel);
            }
            var fornecedor = _mapper.Map<Fornecedor>(fornecedorViewModel);

            //await _fornecedorRepository.Atualizar(fornecedor);
            await _fornecedorService.Atualizar(fornecedor);

            if (!OperacaoValida()) View(fornecedorViewModel);

            _notfy.Success("Fornecedor Atualizado com Sucesso!");
            return RedirectToAction("Index");
            
        }


        #region     OBS DELETE
        /*
          //public async Task<IActionResult> Delete(Guid? id) Nunca será nulo
         
          Metodo Responsavel de consultar o fornecedor no banco e retornar todas as informações  e mostar as informações 
          e mostrar na view par ao usuário confirmar se é o fornecedor desejado.
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
        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [Route("excluir-fornecedor/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {

            var fornecedorViewModel = await ObterFornecedorEndereco(id);


            if (fornecedorViewModel == null) return NotFound();
            
            return View(fornecedorViewModel);
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
        [ClaimsAuthorize("Fornecedor", "Excluir")]
        [Route("excluir-fornecedor/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Esse atributo exige que o token seja validado (Segurança)
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var fornecedorViewModel = await ObterFornecedorEndereco(id);

            if (fornecedorViewModel == null) return NotFound();

            //await _fornecedorRepository.Remover(id);
            await _fornecedorService.Remover(id);

            if (!OperacaoValida()) View(fornecedorViewModel);

            return RedirectToAction(nameof(Index));
        }


        #region AULA
        /*
         Utilizando Moda Window de edição.
         */
        #endregion
        [Route("obter-endereco-fornecedor/{id:guid}")]
        public async Task<IActionResult> ObterEndereco(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);

            if (fornecedor == null)
            {
                return NotFound();
            }

            return PartialView("_DetalhesEndereco", fornecedor);
        }


        [Route("atualizar-endereco-fornecedor/{id:guid}")]
        public async Task<IActionResult> AtualizarEndereco(Guid id)
        {
            var fornecedor = await ObterFornecedorEndereco(id);

            if (fornecedor == null)
            {
                return NotFound();
            }

            #region Comentário
            /*
             Caso o fornercedor endereço não seja nulo, nós iremos retornar uma nova estancia de FornecedorEnderecoViewModel
             com apenas o campo endereço preenchido.
             */
            #endregion          
            return PartialView("_AtualizarEndereco", new FornecedorViewModel { Endereco = fornecedor.Endereco });
        }

        [Route("atualizar-endereco-fornecedor/{id:guid}")]
        [HttpPost]
        [ValidateAntiForgeryToken] // Esse atributo exige que o token seja validado (Segurança)
        public async Task<IActionResult> AtualizarEndereco(FornecedorViewModel fornecedorViewModel)
        {
            #region OBS ModelState
            /*
             Como nesse caso nós queremos passar apenas o endereço para ser exebido na modal. Precisamos remover a validação do modelstate
             do Nome e Documento para que o fornecedorViewModel passe pela validação do modelState.
             */
            #endregion
            ModelState.Remove("Nome");
            ModelState.Remove("Documento");

            if (!ModelState.IsValid) return PartialView("_AtualizarEndereco", fornecedorViewModel);

            #region OBS
            /*
                await _enderecoRepository.Atualizar(_mapper.Map<Endereco>(fornecedorViewModel.Endereco));
                Subistituimos o _enderecoRepository pelo _enderecoService para esconder o Repository dos metodos da controller
                que aletem o estado do obj no banco de dados.
            */
            #endregion
            await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(fornecedorViewModel.Endereco));

            #region OBS
            //if (!OperacaoValida()) return PartialView("_AtualizarEndereco", fornecedorViewModel);

            /*
              será passado para a url o metodo ObterEndereço(linha 157) que vai buscar no banco o novo endereço do fornecedor que acabou de
              ser atualizado e depois esse resultado será carregado pelo jquey e exibido na view edit

            <div id="EnderecoTarget">
            <partial name="_DetalhesEndereco" />
            </div>
             */
            #endregion
            var url = Url.Action("ObterEndereco", "Fornecedores", new { id = fornecedorViewModel.Endereco.FornecedorId });
            
            return Json(new { success = true, url });
        }


        private async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
        }

        private async Task<FornecedorViewModel> ObterFornecedorProdutosEndereco(Guid id)
        {
            return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
        }
    }
}
