using ForumBackend.Ef;
using ForumBackend.Services.User;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<ForumDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ForumDb")))
    .AddTransient<IUserService, UserService>();

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger().UseSwaggerUI();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();
