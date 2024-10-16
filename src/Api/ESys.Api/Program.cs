using ESys.Api.Middleware;
using ESys.Persistence;
using FastEndpoints;
using Scalar.AspNetCore;
using ESys.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ESysCorsPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5000",
                "https://localhost:5000",
                "http://localhost:3000",
                "https://localhost:3000",
                "http://localhost:5001",
                "https://localhost:5001",
                "http://localhost:5005",
                "https://localhost:5006",
                $"http://{Environment.GetEnvironmentVariable("DEPLOY_CDN")}",
                $"https://{Environment.GetEnvironmentVariable("DEPLOY_CDN")}"
            )
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddAuthorization();
// builder.Services.AddFastEndpoints();
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


// Add related projects' services
var configuration = new ConfigurationBuilder().Build();
builder.Services.AddPersistenceServices(configuration)
    .AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger(options => { options.RouteTemplate = "openapi/{documentName}.json"; });
    // app.MapScalarApiReference();

    app.UseSwagger();
    app.UseSwaggerUI();


    app.UseCors("ESysCorsPolicy");
}

app.UseHttpsRedirection();
app.UseCustomExceptionHandler();
app.UseHsts();


app.UseAuthorization();
// app.UseFastEndpoints(c =>
// {
//     c.Endpoints.RoutePrefix = "api";
// });
app.MapControllers();


app.Run();