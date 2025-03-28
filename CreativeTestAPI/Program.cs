using Application.Services;
using Domain.Interfaces;
using Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICoberturaService, CoberturaServices>();
builder.Services.AddScoped<ICotacaoService, CotacaoServices>();
builder.Services.AddScoped<IBeneficiarioService, BeneficiarioServices>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CreativeTestAPI",
        Version = "v1",
        Description = "API de teste com Swagger"
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => { 
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CreativeTestAPI");
    });
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
