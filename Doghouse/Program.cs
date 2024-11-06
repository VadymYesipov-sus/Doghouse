using Doghouse.Data;
using Doghouse.Filters;
using Doghouse.Helpers;
using Doghouse.Interfaces;
using Doghouse.Repositories;
using Doghouse.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDogRepository, DogRepository>();

builder.Services.AddScoped<DogCreationValidationFilter>();

builder.Services.AddScoped<GeneralExceptionFilter>();

builder.Services.AddScoped<QueryValidationFilterAttribute>();

builder.Services.AddScoped<IDogService, DogService>();

builder.Services.Configure<ApiBehaviorOptions>(options
    => options.SuppressModelStateInvalidFilter = true);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNextJs",
        builder => builder.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowNextJs");

app.UseMiddleware<RateLimiter>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
