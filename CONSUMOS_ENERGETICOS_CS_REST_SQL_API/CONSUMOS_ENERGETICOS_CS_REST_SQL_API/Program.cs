using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.DbContexts;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Interfaces;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Repositories;
using CONSUMOS_ENERGETICOS_CS_REST_SQL_API.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//Aqui agregamos los servicios requeridos

//El DBContext a utilizar
builder.Services.AddSingleton<PgsqlDbContext>();

//Los repositorios
builder.Services.AddScoped<IResumenRepository, ResumenRepository>();
builder.Services.AddScoped<IPeriodoRepository, PeriodoRepository>();
//builder.Services.AddScoped<IServicioRepository, ServicioRepository>();
//builder.Services.AddScoped<IComponenteRepository, ComponenteRepository>();

//Aqui agregamos los servicios asociados para cada ruta
builder.Services.AddScoped<ResumenService>();
builder.Services.AddScoped<PeriodoService>();
//builder.Services.AddScoped<ServicioService>();
//builder.Services.AddScoped<ComponenteService>();

// Agregamos los servicios al contenedor de la aplicación
builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Consumos Energéticos - Versión en PostgreSQL",
        Description = "API para la gestión de consumos energéticos"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Modificamos el encabezado de las peticiones para ocultar el web server utilizado
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("Server", "EnergyServer");
    await next();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
