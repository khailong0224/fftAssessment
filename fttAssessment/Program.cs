using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using fttAssessment.Services;
using fttAssessment.Helpers;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
// Add log4net configuration
var logRepository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());
log4net.Config.XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
builder.Services.AddControllers();

builder.Services.AddScoped<ITransactionService, TransactionService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use the routing system and map controllers for API
app.UseRouting();

// Enable HTTP request pipeline for API
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); 
});


app.Run();
