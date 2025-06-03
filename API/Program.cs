using API.Aplicacao.Repositorios;
using API.Application;
using API.Infrastructure.Context;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;


// TODO: Refatorar: Abstrair valida��es dos controllers para uma camada Service (corrigir para a sprint 2)
// TODO: Refatorar: Abstrair convers�es de DTO para uma camada Service (corrigir para a sprint 2)
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de filiais e motos Mottu",
        Version = "v1",
        Description = "API para gerenciar filiais e motos da Mottu nos p�tios",
        Contact = new OpenApiContact
        {
            Name = "Prisma.Code",
            Email = "prismacode3@gmail.com"
        },
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; // Obt�m o nome do arquivo XML de documenta��o
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile); // Cria o caminho completo para o arquivo XML
    swagger.IncludeXmlComments(xmlPath); // Inclui o arquivo XML de documenta��o no Swagger
});

// Busca as credenciais pelo documento .env
try
{
    var connectionString = Environment.GetEnvironmentVariable("ConnectionString__Oracle");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseOracle(connectionString)); // Configura o DbContext para usar o Oracle com a string de conex�o definida no appsettings.json
}
catch (ArgumentNullException)
{
    throw new Exception("Falha ao buscar a var�avel de ambiente");
}

builder.Services.AddScoped<MotoRepositorio>();// Registra o reposit�rio de motos como um servi�o com escopo
builder.Services.AddScoped<FilialRepositorio>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
