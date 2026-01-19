
using Ats.Application;
using Ats.Infrastructure;
using Ats.Infrastructure.AtsDatabase;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<MongoSettings>()
    .Bind(builder.Configuration.GetSection("MongoSettings"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddInfrastructure();
builder.Services.AddApplication();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p =>
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod());
});



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
