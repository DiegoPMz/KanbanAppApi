using KanbanAppApi.Data;
using KanbanAppApi.Repositories;
using KanbanAppApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json.Serialization;
using KanbanAppApi.Features.Board;
using KanbanAppApi.Features.Task;
using Mediator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationContextDb>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("KanbanDbConnection")));

// Add services to the container.
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance =
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
    };
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHttpClient();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtIssuer"]!,
            ValidAudience = builder.Configuration["JwtAudience"]!,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSecretKey"]!)),
        };
        
        options.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                var accessTokenExist = context.Request.Cookies.TryGetValue("access_token", out var accessToken);
                if (accessTokenExist)
                {
                    context.Token = accessToken;
                }
                
                return Task.CompletedTask;                
            }
        };
    });
builder.Services.AddMediator(options => 
{
    options.ServiceLifetime = ServiceLifetime.Scoped; 
});

var app = builder.Build();

app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

CreateBoard.CreateBoardEndpoint.Map(app);
UpdateBoard.UpdateBoardEndpoint.Map(app);
DeleteBoard.DeleteBoardEndpoint.Map(app);
GetBoardsPaginated.GetBoardPaginatedEndpoint.Map(app);

AddColumn.AddColumnEndpoint.Map(app);
RemoveColumn.RemoveColumnEndpoint.Map(app);
UpdateColumn.UpdateColumnEndpoint.Map(app);
ReorderColumn.ReorderColumnEndpoint.Map(app);

CreateTask.CreateTaskEndpoint.Map(app);
GetTask.GetTaskEndpoint.Map(app);
UpdateTask.UpdateTaskEndpoint.Map(app);
DeleteTask.DeleteTaskEndpoint.Map(app);

AddSubTask.AddSubTaskEndpoint.Map(app);
RemoveSubTask.RemoveSubTaskEndpoint.Map(app);
UpdateSubTask.UpdateSubTaskEndpoint.Map(app);

app.Run();