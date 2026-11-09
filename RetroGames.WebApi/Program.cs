using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.Repository.Conexao;
using RetroGames.Repository.Interfaces;
using RetroGames.Repository.Repositorios;
using RetroGames.Services.Interfaces;
using RetroGames.Services.Servicos;
using RetroGames.WebApi.ApiModels.ApiModels;
using RetroGames.WebApi.Filters;
using RetroGames.WebApi.Tratamento;
using RetroGames.WebApi.Validators;

var construtor = WebApplication.CreateBuilder(args);
construtor.WebHost.UseUrls("http://localhost:5080");
var connectionString = construtor.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidOperationException("Configure ConnectionStrings:Default em user-secrets ou variável de ambiente.");

construtor.Services.AddSingleton(new RepositoryConfig(connectionString));
construtor.Services.AddScoped<IRepositorioSet, RepositorioSet>();
construtor.Services.AddScoped<IPlataformaService, PlataformaService>();
construtor.Services.AddScoped<IJogoService, JogoService>();
construtor.Services.AddScoped<IGeneroService, GeneroService>();
construtor.Services.AddScoped<ICondicaoJogoService, CondicaoJogoService>();
construtor.Services.AddScoped<IAutenticacaoService, AutenticacaoService>();
construtor.Services.AddValidatorsFromAssemblyContaining<PlataformaEntradaValidator>();
construtor.Services.AddScoped<ValidacaoFluentValidationFilter>();
construtor.Services.AddControllers(opcoes => { opcoes.Filters.AddService<ValidacaoFluentValidationFilter>(); opcoes.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute()); });
construtor.Services.AddAntiforgery(opcoes => { opcoes.HeaderName = "X-CSRF-TOKEN"; opcoes.Cookie.Name = "RETRO-CSRF"; opcoes.Cookie.HttpOnly = true; opcoes.Cookie.SameSite = SameSiteMode.Strict; opcoes.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; });
construtor.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(opcoes =>
{
    opcoes.Cookie.Name = "RETRO-SESSAO"; opcoes.Cookie.HttpOnly = true; opcoes.Cookie.SameSite = SameSiteMode.Strict; opcoes.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    opcoes.ExpireTimeSpan = TimeSpan.FromHours(2); opcoes.SlidingExpiration = true;
    opcoes.Events.OnRedirectToLogin = contexto => { contexto.Response.StatusCode = StatusCodes.Status401Unauthorized; return Task.CompletedTask; };
    opcoes.Events.OnRedirectToAccessDenied = contexto => { contexto.Response.StatusCode = StatusCodes.Status403Forbidden; return Task.CompletedTask; };
});
construtor.Services.AddAuthorization(opcoes => opcoes.AddPolicy("SomenteAdministrador", politica => politica.RequireAuthenticatedUser().RequireRole("Administrador")));
construtor.Services.AddExceptionHandler<GlobalExceptionHandler>();
construtor.Services.AddProblemDetails();
construtor.Services.AddEndpointsApiExplorer();
construtor.Services.AddSwaggerGen();

var app = construtor.Build();
using (var escopoInicial = app.Services.CreateScope())
{
    var config = escopoInicial.ServiceProvider.GetRequiredService<IConfiguration>();
    var nomeAdmin = config["AdminInicial:Nome"]; var emailAdmin = config["AdminInicial:Email"]; var senhaAdmin = config["AdminInicial:Senha"];
    if (!string.IsNullOrWhiteSpace(nomeAdmin) && !string.IsNullOrWhiteSpace(emailAdmin) && !string.IsNullOrWhiteSpace(senhaAdmin))
        await escopoInicial.ServiceProvider.GetRequiredService<IAutenticacaoService>().InicializarAdministradorAsync(nomeAdmin,emailAdmin,senhaAdmin);
}
app.UseExceptionHandler();
app.UseStatusCodePages(async contexto =>
{
    var resposta = contexto.HttpContext.Response;
    if (resposta.HasStarted) return;
    int codigo = resposta.StatusCode switch { 401 => ResponseCodes.NAO_AUTENTICADO.Code, 403 => ResponseCodes.ACESSO_NEGADO.Code, _ => ResponseCodes.ERRO };
    string mensagem = resposta.StatusCode switch { 401 => ResponseCodes.NAO_AUTENTICADO.DefaultMessage, 403 => ResponseCodes.ACESSO_NEGADO.DefaultMessage, _ => "Requisição inválida." };
    resposta.ContentType = "application/json";
    await resposta.WriteAsJsonAsync(EnvelopeResposta<object>.Erro(codigo, mensagem));
});
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
