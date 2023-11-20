using DevIO.App.Extension;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace DevIO.App.Extension
{
    #region  Validação para trabalhar no lado do Server

    #region OBS
    /*
     Construimos essa classe para tarbalhar a validação do lado do Server através do DataAnnottion.
     Em nossa ProdutoViewModels podemos adicionar essa validação através de dataannotation
     ex:
        [Moeda]
        public decimal Valor { get; set; }
     */
    #endregion
    public class MoedaAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                // o metodo tentará converter o value do paramento levando em consideração a cultura que new CultureInfo("pt-br")
                var moeda = Convert.ToDecimal(value, new CultureInfo("pt-br"));
            }
            catch (Exception)
            {
                return new ValidationResult("Moeda em formato invalido");
            }
            return ValidationResult.Success;
        }
    }
    #endregion

}

#region  Class Necessária para trabalhar no lado do Client

#region OBS:
/*
    Classe necessária para trazer a validação para o lado do Client funcionando com javascript
 */
#endregion
public class MoedaAttributeAdapter : AttributeAdapterBase<MoedaAttribute>
{

    public MoedaAttributeAdapter(MoedaAttribute attribute, IStringLocalizer stringLocalizer) : base(attribute, stringLocalizer)
    {

    }

    public override void AddValidation(ClientModelValidationContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-moeda", GetErrorMessage(context));
        MergeAttribute(context.Attributes, "data-val-number", GetErrorMessage(context));
    }

    public override string GetErrorMessage(ModelValidationContextBase validationContext)
    {
        return "Moeda em formato inválido";
    }
}
#endregion


#region Adapter

#region OBS:
/*
  Precisamos dessa classe para fazer o MoedaAttributeAdapter funcionar
 */
#endregion
public class MoedaValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
{
    private readonly IValidationAttributeAdapterProvider _baseProvider = new ValidationAttributeAdapterProvider();
    public IAttributeAdapter GetAttributeAdapter(ValidationAttribute attribute, IStringLocalizer stringLocalizer)
    {
        if (attribute is MoedaAttribute moedaAttribute)
        {
            return new MoedaAttributeAdapter(moedaAttribute, stringLocalizer);
        }

        return _baseProvider.GetAttributeAdapter(attribute, stringLocalizer);
    }
}
#endregion

