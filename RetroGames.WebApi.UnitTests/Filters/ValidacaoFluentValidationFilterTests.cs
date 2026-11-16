using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Filters;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Filters;

public sealed class ValidacaoFluentValidationFilterTests
{
    [Fact]
    public void JsonMalformado_DeveRetornarCodigoDeModeloInvalidoSemCampos()
    {
        var modelState = new ModelStateDictionary();
        modelState.TryAddModelError("$", new JsonException("JSON inválido."), new EmptyModelMetadataProvider().GetMetadataForType(typeof(object)));

        var resultado = Assert.IsType<BadRequestObjectResult>(ValidacaoFluentValidationFilter.CriarRespostaModelState(modelState));
        var envelope = Assert.IsType<EnvelopeResposta<object>>(resultado.Value);

        Assert.Equal(ResponseCodes.MODEL_STATE_INVALIDO.Code, envelope.ResponseCode);
        Assert.Null(envelope.Data);
    }

    [Fact]
    public void CorpoAusente_DeveRetornarCodigoDeModeloNulo()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("modelo", "A non-empty request body is required.");

        var resultado = Assert.IsType<BadRequestObjectResult>(ValidacaoFluentValidationFilter.CriarRespostaModelState(modelState));
        var envelope = Assert.IsType<EnvelopeResposta<object>>(resultado.Value);

        Assert.Equal(ResponseCodes.MODEL_NULL.Code, envelope.ResponseCode);
    }

    [Fact]
    public void ErroDeCampo_DeveRetornarNomeEmCamelCase()
    {
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Pagina", "A página deve ser maior ou igual a 1.");

        var resultado = Assert.IsType<BadRequestObjectResult>(ValidacaoFluentValidationFilter.CriarRespostaModelState(modelState));
        var envelope = Assert.IsType<EnvelopeResposta<object>>(resultado.Value);
        var erros = Assert.IsType<Dictionary<string, string[]>>(envelope.Data);

        Assert.Contains("pagina", erros.Keys);
    }
}
