using NSwag.Generation.Processors.Security;
using NSwag;
using ApiCubosExamen.Data;
using Microsoft.EntityFrameworkCore;
using ApiCubosExamen.Helpers;
using ApiCubosExamen.Repositories;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

HelperActionService helper = new HelperActionService(builder.Configuration);
builder.Services.AddSingleton<HelperActionService>(helper);
builder.Services.AddAuthentication(helper.GetAuthenticateSchema())
    .AddJwtBearer(helper.GetJwtBearerOptions());

builder.Services.AddTransient<RepositoryCubos>();


// Add services to the container.
builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient
    (builder.Configuration.GetSection("KeyVault"));
});

//DEBEMOS PODER RECUPERAR UN OBJETO INYECTADO EN CLASES 
//QUE NO TIENEN CONSTRUCTOR
SecretClient secretClient =
builder.Services.BuildServiceProvider().GetService<SecretClient>();
KeyVaultSecret secret =
    await secretClient.GetSecretAsync("Keyconnection");
string connectionString = secret.Value;

//string connectionString =
//    builder.Configuration.GetConnectionString("SqlAzure");

builder.Services.AddDbContext<CubosContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddOpenApiDocument(document =>
{
    document.Title = "Api Cubos";
    document.Description = "Api Examen Cubos ";
    document.Version = "v1";
    document.AddSecurity("JWT", Enumerable.Empty<string>(),
        new NSwag.OpenApiSecurityScheme
        {
            Type = OpenApiSecuritySchemeType.ApiKey,
            Name = "Authorization",
            In = OpenApiSecurityApiKeyLocation.Header,
            Description = "Copia y pega el Token en el campo 'Value:' así: Bearer {Token JWT}."
        }
    );
    document.OperationProcessors.Add(
    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
});

var app = builder.Build();

app.UseOpenApi();
app.UseSwaggerUI(
    options =>
    {
        options.InjectStylesheet("/css/bootstrap.css");
        options.InjectStylesheet("/css/material3x.css");

        options.SwaggerEndpoint(
             url: "/swagger/v1/swagger.json", name: "Api OAuth Empleados v1");
        options.RoutePrefix = "";

    }
    );

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
