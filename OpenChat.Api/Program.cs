using Microsoft.AspNetCore.Cors.Infrastructure;
using OpenChat.Api;

const string DEVELOPMENT_CORS_POLICY_NAME = "CorsDevelopmentPolicy";
const string PRODUCTION_CORS_POLICY_NAME = "CorsProductionPolicy";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => 
{
    options.AddPolicy(DEVELOPMENT_CORS_POLICY_NAME, GetDevelopmentCorsPolicy);
    options.AddPolicy(PRODUCTION_CORS_POLICY_NAME, GetProductionCorsPolicy);
});

builder.Services.AddInfrastructureLayer();
builder.Services.AddDomainLayer();

var app = builder.Build();

var corsPolicyName = PRODUCTION_CORS_POLICY_NAME;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    corsPolicyName = DEVELOPMENT_CORS_POLICY_NAME;
    app.UseSwagger();
    app.UseSwaggerUI();
}         

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(corsPolicyName);

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Welcome to OpenChat Api!");

app.Run();

void GetDevelopmentCorsPolicy(CorsPolicyBuilder policyBuilder)
{
    policyBuilder
        .WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader();
}

void GetProductionCorsPolicy(CorsPolicyBuilder policyBuilder)
{
    policyBuilder
        .WithOrigins("https://openchat.com", "http://openchat.com")
        .AllowAnyMethod()
        .AllowAnyHeader();
}