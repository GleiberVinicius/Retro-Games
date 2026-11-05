using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RetroGames.WebApi.ApiModels.ApiModels;
namespace RetroGames.WebApi.Filters;
public sealed class ValidacaoFluentValidationFilter(IServiceProvider servicos) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext contexto,ActionExecutionDelegate proximo)
    {
        if(!contexto.ModelState.IsValid)
        {
            var errosModelo=contexto.ModelState.Where(x=>x.Value?.Errors.Count>0).ToDictionary(x=>char.ToLowerInvariant(x.Key[0])+x.Key[1..],x=>x.Value!.Errors.Select(e=>string.IsNullOrWhiteSpace(e.ErrorMessage)?"Valor inválido.":e.ErrorMessage).ToArray());
            contexto.Result=new BadRequestObjectResult(new EnvelopeResposta<object>(ResponseCodes.MODEL_STATE_INVALIDO.Code,errosModelo,ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage));return;
        }
        foreach(var parametro in contexto.ActionDescriptor.Parameters)
        {
            var tipoParametro=parametro.ParameterType;
            if(tipoParametro.IsClass && tipoParametro!=typeof(string) && contexto.ActionArguments.TryGetValue(parametro.Name,out var valorNulo) && valorNulo is null && servicos.GetService(typeof(IValidator<>).MakeGenericType(tipoParametro)) is not null)
            {
                contexto.Result=new BadRequestObjectResult(EnvelopeResposta<object>.Erro(ResponseCodes.MODEL_NULL.Code,ResponseCodes.MODEL_NULL.DefaultMessage));return;
            }
        }
        foreach(var valor in contexto.ActionArguments.Values.Where(x=>x is not null))
        {
            var tipo=typeof(IValidator<>).MakeGenericType(valor!.GetType());
            if(servicos.GetService(tipo) is not IValidator validador)continue;
            var resultado=await validador.ValidateAsync(new ValidationContext<object>(valor),contexto.HttpContext.RequestAborted);
            if(!resultado.IsValid)
            {
                var erros=resultado.Errors.GroupBy(e=>char.ToLowerInvariant(e.PropertyName[0])+e.PropertyName[1..]).ToDictionary(g=>g.Key,g=>g.Select(e=>e.ErrorMessage).ToArray());
                contexto.Result=new BadRequestObjectResult(new EnvelopeResposta<object>(ResponseCodes.MODEL_STATE_INVALIDO.Code,erros,ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage));return;
            }
        }
        await proximo();
    }
}
