using RetroGames.Model.Model;
using RetroGames.Services.Modelos;
using RetroGames.WebApi.ApiModels.ApiModels;

namespace RetroGames.WebApi.Mapeadores;

public static class PlataformaMapper
{
    public static Plataforma ParaModel(this InserirPlataformaRequestApiModel modelo)
        => new()
        {
            Nome = modelo.Nome.Trim(),
            Fabricante = modelo.Fabricante.Trim(),
            AnoLancamento = modelo.AnoLancamento,
        };

    public static Plataforma ParaModel(this AlterarPlataformaRequestApiModel modelo, int id)
        => new()
        {
            Id = id,
            Nome = modelo.Nome.Trim(),
            Fabricante = modelo.Fabricante.Trim(),
            AnoLancamento = modelo.AnoLancamento,
        };

    public static ListarPlataformaResponseApiModel ParaResponse(this PlataformaServiceModel plataforma)
        => new(plataforma.Id, plataforma.Nome, plataforma.Fabricante, plataforma.AnoLancamento);
}
