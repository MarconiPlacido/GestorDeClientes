using System.Text;
using GestorDeClientes.Core.Data;
using GestorDeClientes.Core.Services;
using GestorDeClientes.Core.Services.Cadastrar;
using GestorDeClientes.Core.Services.CadastrarCliente;
using GestorDeClientes.Core.Services.CadastrarContato;
using GestorDeClientes.Core.Services.importacaoCorrigida;
using GestorDeClientes.Core.Services.importacaoXML;
using GestorDeClientes.Core.Services.MinIo;
using GestorDeClientes.Core.Services.Senha;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Minio;
using GestorDeClientes.API.Services.RabbitPublicar;
using GestorDeClientes.Core.Repositorios.Mongo;
using GestorDeClientes.Core.Services.ImportacaoHistorico;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// Swagger + JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Gestor de Clientes API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite: Bearer {seu token JWT}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(GestorDeClientes.Core.Profiles.AutoMapperProfiles).Assembly);

// DI
builder.Services.AddScoped<ICadastrar, CadastrarService>();
builder.Services.AddScoped<ISenha, SenhaService>();
builder.Services.AddScoped<Icliente, ClienteService>();
builder.Services.AddScoped<Icontato, ContatoService>();
builder.Services.AddScoped<IImportacao, ImportacaoService>();
builder.Services.AddScoped<IProcessadorImportacao, ProcessadorImportacao>();
builder.Services.AddScoped<IMinio, MinioService>();
builder.Services.AddScoped<IRabbitService, RabbitService>();
builder.Services.AddScoped<IRabbitMqPublisher,RabbitMqPublisher>();
builder.Services.AddScoped<IImportacaoRepository, ImportacaoRepository>();
builder.Services.AddScoped<IImportacaoHistoricoService, ImportacaoHistoricoService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 10,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
        }
    );
});

builder.Services.AddSingleton<IConnection>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    var factory = new ConnectionFactory
    {
        HostName = config["RabbitMQ:Host"] ?? "rabbitmq",
        UserName = config["RabbitMQ:User"] ?? "gestor",
        Password = config["RabbitMQ:Password"] ?? "gestor123",
        DispatchConsumersAsync = true
    };

    return factory.CreateConnection();
});


builder.Services.AddSingleton<IMinioClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    return new MinioClient()
        .WithEndpoint(configuration["Minio:Endpoint"])
        .WithCredentials(
            configuration["Minio:AccessKey"],
            configuration["Minio:SecretKey"]
        )
        .WithSSL(false) // importante no Docker local
        .Build();
});

builder.Services.AddSingleton<MongoDbContext>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new MongoDbContext(configuration);
});

builder.Services.AddControllers()
    .AddControllersAsServices();

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
    throw new Exception("Jwt:Key não configurado");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
