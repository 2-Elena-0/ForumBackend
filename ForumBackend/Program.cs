using System.Text;
using ForumBackend.Ef;
using ForumBackend.Services.Comment;
using ForumBackend.Services.Post;
using ForumBackend.Services.Role;
using ForumBackend.Services.Topic;
using ForumBackend.Services.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<ForumDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ForumDb")))
    .AddTransient<IUserService, UserService>()
    .AddTransient<IPostService, PostService>()
    .AddTransient<ICommentService, CommentService>()
    .AddTransient<ITopicService, TopicService>()
    .AddTransient<IRoleService, RoleService>()
    ;

builder.Services.AddCors(options =>
    options.AddPolicy("CorsPolicy", p => p
        .WithOrigins("http://localhost:5173")
        .AllowAnyMethod()
        .AllowAnyHeader()));

builder.Services.AddAuthentication("Bearer") 
    .AddJwtBearer("Bearer", options => 
    { 
        options.TokenValidationParameters = new TokenValidationParameters 
        { 
            ValidateIssuerSigningKey = true, 
            ValidateAudience = true, 
            ValidateLifetime = true, 
            ValidateIssuer = true, 
            ValidIssuer = builder.Configuration["Jwt:Issuer"], 
            ValidAudience = builder.Configuration["Jwt:Audience"], 
            IssuerSigningKey = new SymmetricSecurityKey( 
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]) 
            ) 
        }; 
    }); 
builder.Services.AddAuthorization(); 

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(context =>
    {
        context.Response.StatusCode = 500;
        return context.Response.WriteAsync("An unexpected fault happened.");
    });
});

app.UseCors("CorsPolicy");

app.UseSwagger().UseSwaggerUI();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();