
using FC.CodeFlix.Catalog.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAndConfigureControllers();
builder.Services.AddUseCases();
builder.Services.AddAppConnections(builder.Configuration);

var app = builder.Build();

app.UseDocumentation();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public partial class Program;
