namespace RetroGames.Services.Excecoes;
public sealed class BusinessRuleException(int codigo, string mensagem) : Exception(mensagem)
{
    public int Codigo { get; } = codigo;
}
