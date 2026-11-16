using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RetroGames.WebApi.ApiModels.ApiModels;
namespace RetroGames.WebApi.Filters;
public sealed class ValidacaoFluentValidationFilter(IServiceProvider servicos) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext contexto,ActionExecutionDelegate proximo)
    {
        if(!contexto.ModelState.IsValid)
        {
            contexto.Result=CriarRespostaModelState(contexto.ModelState);return;
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

    public static IActionResult CriarRespostaModelState(ModelStateDictionary modelState)
    {
        var erros=modelState.Values.SelectMany(valor=>valor?.Errors??[]).ToArray();
        if(erros.Any(erro=>ContemExcecaoJson(erro.Exception)))
            return new BadRequestObjectResult(EnvelopeResposta<object>.Erro(ResponseCodes.MODEL_STATE_INVALIDO.Code,ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage));

        if(erros.Any(erro=>erro.ErrorMessage.Contains("request body is required",StringComparison.OrdinalIgnoreCase)
            || erro.ErrorMessage.Contains("corpo da requisição é obrigatório",StringComparison.OrdinalIgnoreCase)))
            return new BadRequestObjectResult(EnvelopeResposta<object>.Erro(ResponseCodes.MODEL_NULL.Code,ResponseCodes.MODEL_NULL.DefaultMessage));

        var errosModelo=modelState.Where(item=>item.Value?.Errors.Count>0)
            .ToDictionary(item=>NomeCampo(item.Key),item=>item.Value!.Errors
                .Select(erro=>string.IsNullOrWhiteSpace(erro.ErrorMessage)?"Valor inválido.":erro.ErrorMessage).ToArray());
        return new BadRequestObjectResult(new EnvelopeResposta<object>(ResponseCodes.MODEL_STATE_INVALIDO.Code,errosModelo,ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage));
    }

    private static bool ContemExcecaoJson(Exception? excecao)
    {
        while(excecao is not null)
        {
            if(excecao is JsonException)return true;
            excecao=excecao.InnerException;
        }
        return false;
    }

    private static string NomeCampo(string nome)=>string.IsNullOrEmpty(nome)?nome:char.ToLowerInvariant(nome[0])+nome[1..];
}
