using TaskManagement.API.Constants;
using TaskManagement.API.Extensions;
using TaskManagement.API.Middleware;
using TaskManagement.Application;
using TaskManagement.Infrastructure;
using TaskManagement.Application.Common.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.AddJwtAuthentication();
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

if (app.Configuration.GetValue<bool>(ConfigurationKeys.MigrateOnStartup))
{
    await app.Services.ApplyMigrationsAsync();
}

// Pipeline'ın en başında: sonraki her şeydeki hatayı yakalar
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>(ConfigurationKeys.SwaggerEnabled))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Konteynerde TLS yok; production'da HTTPS'i genelde reverse proxy sonlandırır
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();