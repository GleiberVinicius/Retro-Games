using Microsoft.AspNetCore.Http;
using RetroGames.WebApi.ApiModels.ApiModels;

namespace RetroGames.Services.Excecoes;

/// <summary>Um erro de negócio com código, mensagem e status HTTP de resposta.</summary>
public sealed record BusinessError(int Codigo, string Mensagem, int StatusHttp);

/// <summary>Catálogo dos erros de negócio usados pelos Services.</summary>
public static class BusinessErrors
{
    public static readonly BusinessError PLATAFORMA_NAO_ENCONTRADA = new(100, "Plataforma não encontrada.", StatusCodes.Status404NotFound);
    public static readonly BusinessError PLATAFORMA_NOME_DUPLICADO = new(ResponseCodes.PLATAFORMA_NOME_DUPLICADO.Code, ResponseCodes.PLATAFORMA_NOME_DUPLICADO.DefaultMessage, StatusCodes.Status409Conflict);
    public static readonly BusinessError PLATAFORMA_POSSUI_JOGOS = new(ResponseCodes.PLATAFORMA_POSSUI_JOGOS.Code, ResponseCodes.PLATAFORMA_POSSUI_JOGOS.DefaultMessage, StatusCodes.Status409Conflict);
    public static readonly BusinessError JOGO_NAO_ENCONTRADO = new(ResponseCodes.JOGO_NAO_ENCONTRADO.Code, ResponseCodes.JOGO_NAO_ENCONTRADO.DefaultMessage, StatusCodes.Status404NotFound);
    public static readonly BusinessError CONDICAO_NAO_ENCONTRADA = new(ResponseCodes.CONDICAO_INEXISTENTE.Code, ResponseCodes.CONDICAO_INEXISTENTE.DefaultMessage, StatusCodes.Status404NotFound);
    public static readonly BusinessError GENERO_NAO_ENCONTRADO = new(ResponseCodes.GENERO_INEXISTENTE.Code, ResponseCodes.GENERO_INEXISTENTE.DefaultMessage, StatusCodes.Status404NotFound);

    // Métodos mantidos para os Services já existentes no projeto.
    public static BusinessRuleException PlataformaNaoEncontrada() => new(PLATAFORMA_NAO_ENCONTRADA);
    public static BusinessRuleException PlataformaNomeDuplicado() => new(PLATAFORMA_NOME_DUPLICADO);
    public static BusinessRuleException PlataformaPossuiJogos() => new(PLATAFORMA_POSSUI_JOGOS);
    public static BusinessRuleException JogoNaoEncontrado() => new(JOGO_NAO_ENCONTRADO);
    public static BusinessRuleException PlataformaInexistente() => new(ResponseCodes.PLATAFORMA_INEXISTENTE.Code, ResponseCodes.PLATAFORMA_INEXISTENTE.DefaultMessage, StatusCodes.Status404NotFound);
    public static BusinessRuleException CondicaoInexistente() => new(CONDICAO_NAO_ENCONTRADA);
    public static BusinessRuleException GeneroInexistente() => new(GENERO_NAO_ENCONTRADO);
    public static BusinessRuleException CredenciaisInvalidas() => new(ResponseCodes.CREDENCIAIS_INVALIDAS.Code, ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage, StatusCodes.Status401Unauthorized);
}
