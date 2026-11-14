using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using RetroGames.Services.Excecoes;
using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Tratamento;
using Xunit;

namespace RetroGames.WebApi.UnitTests.Tratamento;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task RegraDeNegocio_DeveUsarStatusCodigoEMensagemDaRegra()
    {
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);

        await handler.TryHandleAsync(
            contexto,
            new BusinessRuleException(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA),
            TestContext.Current.CancellationToken);

        Assert.Equal(StatusCodes.Status404NotFound, contexto.Response.StatusCode);
        contexto.Response.Body.Position = 0;
        using var documento = await JsonDocument.ParseAsync(contexto.Response.Body, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(100, documento.RootElement.GetProperty("responseCode").GetInt32());
        Assert.Equal("Plataforma não encontrada.", documento.RootElement.GetProperty("errorMessage").GetString());
    }

    [Fact]
    public async Task JogoNaoEncontrado_DeveResponder404ComCodigo103()
    {
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);

        await handler.TryHandleAsync(
            contexto,
            new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO),
            TestContext.Current.CancellationToken);

        Assert.Equal(StatusCodes.Status404NotFound, contexto.Response.StatusCode);
        contexto.Response.Body.Position = 0;
        using var documento = await JsonDocument.ParseAsync(contexto.Response.Body, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(103, documento.RootElement.GetProperty("responseCode").GetInt32());
        Assert.Equal("Jogo não encontrado.", documento.RootElement.GetProperty("errorMessage").GetString());
    }

    [Fact]
    public async Task ExcecaoInesperada_DeveResponderComMensagemGenericaSemDetalhes()
    {
        var contexto = new DefaultHttpContext();
        contexto.Response.Body = new MemoryStream();
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);

        await handler.TryHandleAsync(contexto, new InvalidOperationException("Detalhe interno"), TestContext.Current.CancellationToken);

        Assert.Equal(StatusCodes.Status500InternalServerError, contexto.Response.StatusCode);
        contexto.Response.Body.Position = 0;
        using var documento = await JsonDocument.ParseAsync(contexto.Response.Body, cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(ResponseCodes.ERRO, documento.RootElement.GetProperty("responseCode").GetInt32());
        Assert.Equal("Ocorreu um erro inesperado.", documento.RootElement.GetProperty("errorMessage").GetString());
        Assert.DoesNotContain("Detalhe interno", documento.RootElement.GetRawText());
    }
}
