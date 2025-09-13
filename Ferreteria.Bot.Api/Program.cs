using Ferreteria.Bot.Api.Data;   // 👈 importa tu carpeta Data donde está AppDbContext
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 👇 Configuración del DbContext con MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 43)) // ajusta según tu versión MySQL
    )
);

// 👇 Agregamos el cliente HTTP y nuestro servicio AI
builder.Services.AddHttpClient();
builder.Services.AddSingleton<AIService>(sp =>
    new AIService(
        sp.GetRequiredService<IHttpClientFactory>(),
        builder.Configuration["OpenAI:ApiKey"]  // 🔑 Asegúrate de poner tu clave en appsettings.json
    )
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

