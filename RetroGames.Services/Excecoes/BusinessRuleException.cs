using Microsoft.AspNetCore.Http;

namespace RetroGames.Services.Excecoes;

/// <summary>Exceção lançada quando uma regra de negócio é violada.</summary>
public sealed class BusinessRuleException : Exception
{
    public BusinessError Erro { get; }

    public BusinessRuleException(BusinessError erro) : base(erro.Mensagem)
    {
        Erro = erro;
    }

    // Compatibilidade com erros preexistentes no projeto durante esta etapa.
    public BusinessRuleException(int codigo, string mensagem)
        : this(codigo, mensagem, StatusCodes.Status409Conflict)
    {
    }

    public BusinessRuleException(int codigo, string mensagem, int statusHttp)
        : this(new BusinessError(codigo, mensagem, statusHttp))
    {
    }

    public int Codigo => Erro.Codigo;
    public int StatusHttp => Erro.StatusHttp;
}
