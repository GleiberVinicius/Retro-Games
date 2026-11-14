using Microsoft.Extensions.DependencyInjection;
using RetroGames.Repository.Conexao;
using RetroGames.Repository.Interfaces;
using RetroGames.Repository.Repositorios;
using RetroGames.Services.Interfaces;
using RetroGames.Services.Servicos;

namespace RetroGames.WebApi.Configuracao;

public static class InjecaoDependenciaConfig
{
    public static IServiceCollection AddRepositorios(this IServiceCollection servicos, string connectionString)
    {
        servicos.AddSingleton(new RepositoryConfig(connectionString));
        servicos.AddScoped<IRepositorioSet, RepositorioSet>();
        servicos.AddScoped(sp => sp.GetRequiredService<IRepositorioSet>().Plataformas);
        servicos.AddScoped(sp => sp.GetRequiredService<IRepositorioSet>().Jogos);
        servicos.AddScoped(sp => sp.GetRequiredService<IRepositorioSet>().Generos);
        servicos.AddScoped(sp => sp.GetRequiredService<IRepositorioSet>().Condicoes);
        servicos.AddScoped(sp => sp.GetRequiredService<IRepositorioSet>().Usuarios);
        servicos.AddScoped(sp => sp.GetRequiredService<IRepositorioSet>().Perfis);
        servicos.AddScoped(sp => sp.GetRequiredService<IRepositorioSet>().JogosGeneros);

        return servicos;
    }

    public static IServiceCollection AddServicosDeNegocio(this IServiceCollection servicos)
    {
        servicos.AddScoped<IPlataformaService, PlataformaService>();
        servicos.AddScoped<IJogoService, JogoService>();
        servicos.AddScoped<IGeneroService, GeneroService>();
        servicos.AddScoped<ICondicaoJogoService, CondicaoJogoService>();
        servicos.AddScoped<IAutenticacaoService, AutenticacaoService>();

        return servicos;
    }
}
