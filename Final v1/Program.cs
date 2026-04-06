using Final_v1.Application.Interface;
using Final_v1.Application.Services;
using Final_v1.Application.Services.EmployeeService;
using Final_v1.Application.Services.ProjectService;
using Final_v1.Application.Services.SkillService;
using Final_v1.Domain.Interface;
using Final_v1.Infrastructure.Database;
using Final_v1.Infrastructure.Repositories;
using Final_v1.Middleware;
using Microsoft.AspNetCore.Builder;
using static Final_v1.Application.Services.AllocationService;

var builder = WebApplication.CreateBuilder(args);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendAndFlask", policy =>
    {
        policy.WithOrigins(
                //"http://127.0.0.1:5001"
                "http://localhost:5001",
                 "http://localhost:5501"


            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add services to the container.

builder.Services.AddControllers();



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IEmployee, EmployeeService>();
builder.Services.AddScoped<IProject, ProjectService>();
builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IAllocationRepository, AllocationRepository>();
builder.Services.AddScoped<IAllocation, AllocationService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReport, ReportService>();



builder.Services.AddScoped<EmployeeService>(); // or AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<SkillService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<AllocationService>();
builder.Services.AddScoped<ReportService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendAndFlask", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontendAndFlask");

app.UseAuthorization();

app.UseMiddleware<GlobalExceptionHandling>();

app.MapControllers();

app.Run();
