using SmartX.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<TelemetryService>();
builder.Services.AddSingleton<SensorService>();
builder.Services.AddSingleton<DeploymentValidationService>();
builder.Services.AddSingleton<FileStorageService>();
builder.Services.AddSingleton<AttachmentService>();
builder.Services.AddSingleton<CommandService>();
builder.Services.AddSingleton<MeshTopologyService>();

builder.Services.AddHostedService<TelemetrySimulator>();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("SmartXClient", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("SmartXClient");

app.MapControllers();

app.Run();
