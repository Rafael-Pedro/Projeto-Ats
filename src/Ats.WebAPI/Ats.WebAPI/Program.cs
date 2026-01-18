
using Ats.Infrastructure;
using Ats.Infrastructure.AtsDatabase;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<MongoSettings>()
    .Bind(builder.Configuration.GetSection("MongoSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddInfrastructure();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
