using ESys.Persistence;

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


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = new ConfigurationBuilder().Build();
builder.Services.AddPersistenceServices(configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHsts();
app.UseCors("ESysCorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
