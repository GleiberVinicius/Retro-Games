using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Antiforgery;
using RetroGames.Services.Excecoes;
using RetroGames.WebApi.ApiModels.ApiModels;
namespace RetroGames.WebApi.Tratamento;
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context,Exception exception,CancellationToken cancellationToken)
    {
        int status=500,codigo=ResponseCodes.ERRO; string mensagem="Ocorreu um erro inesperado.";
        if(exception is BusinessRuleException regra){codigo=regra.Codigo;mensagem=regra.Message;status=regra.Codigo switch { 4=>401,5=>403,6=>401,7 or 10=>404,8 or 9=>409,11 or 12 or 13=>400,_=>400 };}
        else if(exception is AntiforgeryValidationException){status=400;codigo=ResponseCodes.MODEL_STATE_INVALIDO.Code;mensagem=ResponseCodes.MODEL_STATE_INVALIDO.DefaultMessage;context.Response.Headers["X-CSRF-INVALIDO"]="true";}
        else logger.LogError(exception,"Erro inesperado ao processar a requisição.");
        context.Response.StatusCode=status; await context.Response.WriteAsJsonAsync(EnvelopeResposta<object>.Erro(codigo,mensagem),cancellationToken);return true;
    }
}
