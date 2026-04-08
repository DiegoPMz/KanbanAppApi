using KanbanAppApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using KanbanAppApi.Features.Auth;
using KanbanAppApi.Features.Auth.Infrastructure;
using KanbanAppApi.Features.Board;
using KanbanAppApi.Features.Task;
using KanbanAppApi.Features.User;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//-----> Security Services
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Name = "kanban-app-session";
        options.ExpireTimeSpan = TimeSpan.FromDays(3);
        
        options.SlidingExpiration = true;
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(p =>
    {
        p.WithOrigins(builder.Configuration["Client:CorsOrigin"]!) 
            .AllowAnyHeader()
            .WithMethods("GET", "POST", "PUT", "DELETE")
            .AllowCredentials();
    });
});


builder.Services.AddAuthorization();

//-----> Database Services
builder.Services.AddDbContext<ApplicationContextDb>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("KanbanDbConnection")));

//-----> infrastructure Services
builder.Services.AddHttpClient();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance =
            $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
    };
});

builder.Services.AddScoped<Login.IExternalAuthProvider, GoogleAuthProvider>();

builder.Services.AddMediator(options => 
{
    options.ServiceLifetime = ServiceLifetime.Scoped; 
});

//-----> Documentation Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Kanban API", Version = "v1" });
});


var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.MapSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

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

Login.LoginEndPoint.Map(app);
Logout.LogoutEndPoint.Map(app);

GetUser.GetUserEndPoint.Map(app);

app.Run();