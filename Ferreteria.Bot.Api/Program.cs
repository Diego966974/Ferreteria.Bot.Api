using Ferreteria.Bot.Api.Data;   // DbContext
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Configuración de servicios
// -----------------------------

// Configuración de DbContext con MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 43)) // ajusta según tu versión MySQL
    )
);

// Registro de AIClient para comunicarse con FastAPI
builder.Services.AddHttpClient<AIClient>(client =>
{
    client.BaseAddress = new Uri("http://localhost:8000"); // URL donde corre FastAPI
});

// Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -----------------------------
// Construcción de la app
// -----------------------------

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();


