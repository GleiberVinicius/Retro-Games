namespace RetroGames.WebApi.ApiModels.ApiModels;
public sealed record EnvelopeResposta<T>(int ResponseCode, T? Data, string? ErrorMessage)
{
    public static EnvelopeResposta<T> Sucesso(T? data = default) => new(ResponseCodes.SUCESSO, data, null);
    public static EnvelopeResposta<T> Erro(int codigo, string mensagem) => new(codigo, default, mensagem);
}
