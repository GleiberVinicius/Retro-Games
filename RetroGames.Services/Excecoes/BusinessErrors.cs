using RetroGames.WebApi.ApiModels.ApiModels;
namespace RetroGames.Services.Excecoes;
public static class BusinessErrors
{
    public static BusinessRuleException PlataformaNaoEncontrada() => new(ResponseCodes.PLATAFORMA_NAO_ENCONTRADA.Code, ResponseCodes.PLATAFORMA_NAO_ENCONTRADA.DefaultMessage);
    public static BusinessRuleException PlataformaNomeDuplicado() => new(ResponseCodes.PLATAFORMA_NOME_DUPLICADO.Code,ResponseCodes.PLATAFORMA_NOME_DUPLICADO.DefaultMessage);
    public static BusinessRuleException PlataformaPossuiJogos() => new(ResponseCodes.PLATAFORMA_POSSUI_JOGOS.Code,ResponseCodes.PLATAFORMA_POSSUI_JOGOS.DefaultMessage);
    public static BusinessRuleException JogoNaoEncontrado() => new(ResponseCodes.JOGO_NAO_ENCONTRADO.Code,ResponseCodes.JOGO_NAO_ENCONTRADO.DefaultMessage);
    public static BusinessRuleException PlataformaInexistente() => new(ResponseCodes.PLATAFORMA_INEXISTENTE.Code,ResponseCodes.PLATAFORMA_INEXISTENTE.DefaultMessage);
    public static BusinessRuleException CondicaoInexistente() => new(ResponseCodes.CONDICAO_INEXISTENTE.Code,ResponseCodes.CONDICAO_INEXISTENTE.DefaultMessage);
    public static BusinessRuleException GeneroInexistente() => new(ResponseCodes.GENERO_INEXISTENTE.Code,ResponseCodes.GENERO_INEXISTENTE.DefaultMessage);
    public static BusinessRuleException CredenciaisInvalidas() => new(ResponseCodes.CREDENCIAIS_INVALIDAS.Code,ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage);
}
