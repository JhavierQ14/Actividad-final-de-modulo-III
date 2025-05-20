using Microsoft.AspNetCore.Mvc; // Agrega este using si no lo tienes
using Microsoft.Extensions.Options;
using MONGO_API.EmpleadoServicio;
using MONGO_API.modelos;
using MONGO_API.Settings;

var builder = WebApplication.CreateBuilder(args);

// Configura System.Text.Json para ignorar mayúsculas/minúsculas
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

// 2. Registrar EmployeeService como Singleton
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<MongoDbSettings>>().Value)
    .AddSingleton<IEmployeeService, EmployeeService>();
    
builder.Services.AddSingleton<MongoDB.Driver.MongoClient>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("MongoDB");
    return new MongoDB.Driver.MongoClient(connectionString);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// --- Endpoints CRUD --- //


// Obtener todos los empleados
app.MapGet("/employees", async (IEmployeeService svc) =>
    await svc.GetAsync());


// Obtener un empleado por ID
app.MapGet("/employees/{id:length(24)}", async (string id, IEmployeeService svc) =>
    await svc.GetAsync(id) is Empleados emp
        ? Results.Ok(emp)
        : Results.NotFound());


// Crear uno o varios empleados
// Para un solo empleado
app.MapPost("/employees", async (Empleados empleado, IEmployeeService svc) =>
{
    empleado.Id = null; // Asegurarse que el ID sea null para que MongoDB lo genere
    await svc.CreateAsync(empleado);
    return Results.Created($"/employees/{empleado.Id}", empleado);
});

// Para varios empleados
app.MapPost("/employees/bulk", async (List<Empleados> empleados, IEmployeeService svc) =>
{
    foreach (var emp in empleados)
        await svc.CreateAsync(emp);
    return Results.Created("/employees", empleados);
});


// Actualizar un empleado existente
app.MapPut("/employees/{id:length(24)}", async (string id, Empleados emp, IEmployeeService svc) =>
{
    var existing = await svc.GetAsync(id);
    if (existing is null) return Results.NotFound();
    emp.Id = id;  // asegurar mismo Id
    await svc.UpdateAsync(id, emp);
    return Results.NoContent();
});


// Eliminar un empleado
app.MapDelete("/employees/{id:length(24)}", async (string id, IEmployeeService svc) =>
{
    var existing = await svc.GetAsync(id);
    if (existing is null) return Results.NotFound();
    await svc.RemoveAsync(id);
    return Results.NoContent();
});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
