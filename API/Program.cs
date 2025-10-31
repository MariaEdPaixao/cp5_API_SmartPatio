using Aplicacao.Servicos;
using Dominio.Interfaces;
using Dominio.Persistencia;
using DotNetEnv;
using Infraestrutura.Contexto;
using Infraestrutura.Repositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using API;
using API.Saude;
using Aplicacao.Servicos.Mottu;
using Dominio.Interfaces.Mottu;
using HealthChecks.UI.Client;
using Infraestrutura.Repositorios.Mottu;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Infraestrutura.Configuracoes;
using Infraestrutura.Repositorios.MongoDb;

// TODO: adicionar logica de vinculo usuario patio
// TODO: adicionar logica de moto com patio
// TODO: adicionar logica de moto com carrapato
// TODO: adicionar logica de carrapato com patio

Env.Load();

var builder = WebApplication.CreateBuilder(args);

string? connectionString = null;

// Add services to the container.
builder.Services.AddControllers();

// Configuração MongoDB para Carrapato
builder.Services.Configure<MongoDbConfiguracoes>(options =>
{
    options.ConnectionString = Environment.GetEnvironmentVariable("ConnectionString__Mongo") ?? "";
    options.DatabaseName = Environment.GetEnvironmentVariable("MongoDb__DatabaseName") ?? "MottuDB";
});
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<CarrapatoMongoRepositorio>();
builder.Services.AddScoped<CarrapatoMongoService>();

// Versão da API
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader(); // usa segmento '/api/v{version}/resource' no padrão das rotas dos controllers
});

// Explorer para versões (necessário para o Swagger)
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // ex: v1.0
    options.SubstituteApiVersionInUrl = true;
});

// Registrar configurador que cria um SwaggerDoc por versão
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHealthChecks();

builder.Services.AddSwaggerGen(swagger =>
{
    // XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    swagger.IncludeXmlComments(xmlPath);

    // Inclui controllers nas docs corretas por versão
    swagger.DocInclusionPredicate((docName, apiDesc) =>
    {
        // Use the ApiExplorer-assigned GroupName (e.g. "v1") to decide which actions belong to each Swagger doc.
        // apiDesc.GroupName is populated by the VersionedApiExplorer when AddVersionedApiExplorer is registered.
        if (!string.IsNullOrEmpty(apiDesc.GroupName))
            return string.Equals(apiDesc.GroupName, docName, StringComparison.OrdinalIgnoreCase);

        // If no group name is present, include the endpoint only in the default doc (optional).
        return false;
    });
});

try
{
    connectionString = Environment.GetEnvironmentVariable("ConnectionString__Oracle") ??
                           builder.Configuration.GetConnectionString("Oracle");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseOracle(connectionString));
}
catch (ArgumentNullException)
{
    throw new Exception("Falha ao buscar a variável de ambiente");
}

// Injeção de repositórios
builder.Services.AddScoped<IMotoRepositorio, MotoRepositorio>();
builder.Services.AddScoped<IRepositorio<Patio>, PatioRepositorio>();
builder.Services.AddScoped<IMottuRepositorio, MotoMottuRepositorio>();
builder.Services.AddScoped<IRepositorioUsuario, UsuarioRepositorio>();
builder.Services.AddScoped<IRepositorioCarrapato, CarrapatoRepositorio>();

// Injeção de serviços
builder.Services.AddScoped<MotoServico>();
builder.Services.AddScoped<PatioServico>();
builder.Services.AddScoped<MotoMottuServico>();
builder.Services.AddScoped<UsuarioServico>();
builder.Services.AddScoped<CarrapatoServico>();

// HealthCheck
builder.Services.AddHealthChecks()
    .AddOracle(
        connectionString: connectionString,
        name: "Oracle",
        tags: new[] { "ready", "oracle-database" })
    .AddCheck<CarrapatoHealthCheck>(
        "carrapato_repositorio",
        tags: new[] { "ready" });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();

    // mostra todas as versões do Swagger dinamicamente
    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwaggerUI(c =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"API de filiais e motos Mottu {description.GroupName}");
        }
    });
}

app.UseHttpsRedirection();


// Liveness: retorna 200 so se o app estiver rodando
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false, // sem check de dependencia, so liveness
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Readiness: verifica dependencias com tag "ready"
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseAuthorization();

app.MapControllers();

app.Run();