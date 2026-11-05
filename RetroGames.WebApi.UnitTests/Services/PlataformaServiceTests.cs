using Moq;
using RetroGames.Model.Exceptions;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Excecoes;
using RetroGames.Services.Servicos;
using Xunit;
namespace RetroGames.WebApi.UnitTests.Services;
public sealed class PlataformaServiceTests
{
    [Fact] public async Task Inserir_NomeDuplicado_DeveLancarRegraDeNegocio()
    {
        var plataformas=new Mock<IPlataformaRepository>(); var set=new Mock<IRepositorioSet>(); set.SetupGet(x=>x.Plataformas).Returns(plataformas.Object); plataformas.Setup(x=>x.ExisteNomeAsync("Nintendo",null,It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var servico=new PlataformaService(set.Object); var erro=await Assert.ThrowsAsync<BusinessRuleException>(()=>servico.InserirAsync(new Plataforma{Nome="Nintendo",Fabricante="Nintendo",AnoLancamento=1985}));
        Assert.Equal(8,erro.Codigo); plataformas.Verify(x=>x.InserirAsync(It.IsAny<Plataforma>(),It.IsAny<CancellationToken>()),Times.Never);
    }
    [Fact] public async Task Inserir_DuplicidadeNaGravacao_DeveConverterExcecao()
    {
        var plataformas=new Mock<IPlataformaRepository>();var set=new Mock<IRepositorioSet>();set.SetupGet(x=>x.Plataformas).Returns(plataformas.Object);plataformas.Setup(x=>x.ExisteNomeAsync("Nintendo",null,It.IsAny<CancellationToken>())).ReturnsAsync(false);plataformas.Setup(x=>x.InserirAsync(It.IsAny<Plataforma>(),It.IsAny<CancellationToken>())).ThrowsAsync(new RegistroDuplicadoException("duplicado"));
        var erro=await Assert.ThrowsAsync<BusinessRuleException>(()=>new PlataformaService(set.Object).InserirAsync(new Plataforma{Nome="Nintendo"}));Assert.Equal(8,erro.Codigo);
    }
}
