using Engine.Model;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
});
builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy
    .WithOrigins("http://localhost:4200", "http://localhost:8100",
        "https://localhost:4200", "https://localhost:8100",
        "http://127.0.0.1:4200", "http://127.0.0.1:8100")
    .AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();
app.MapGet("/health", () => Results.Ok(new { Servico = "FinanceTalkEngine.Local", Status = "OK" }));
app.MapPost("/talk", Interpretar);
app.Run();

static IResult Interpretar(LambdaRequest pedido, ILogger<Program> logger)
{
    if (string.IsNullOrWhiteSpace(pedido.Frase) || string.IsNullOrWhiteSpace(pedido.Usuario))
        return Results.BadRequest(new { message = "Informe a frase e o usuário." });
    try
    {
        var lancamento = new Lancamento(pedido.Frase, pedido.Usuario, pedido.Cartoes,
            pedido.Contas, pedido.Pessoas, pedido.Categorias, pedido.SubCategorias);
        lancamento.ProcessarFrase();
        return Results.Ok(new { Frase = lancamento.ToString(), Dados = lancamento });
    }
    catch (Exception exception)
    {
        logger.LogWarning(exception, "Falha ao interpretar frase no Engine local.");
        return Results.UnprocessableEntity(new { message = "Não foi possível interpretar a frase. Revise os dados e tente novamente." });
    }
}
