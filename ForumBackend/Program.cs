using ForumBackend.Ef;
using ForumBackend.Services.Comment;
using ForumBackend.Services.Post;
using ForumBackend.Services.Topic;
using ForumBackend.Services.User;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContext<ForumDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("ForumDb")))
    .AddTransient<IUserService, UserService>()
    .AddTransient<IPostService, PostService>()
    .AddTransient<ICommentService, CommentService>()
    .AddTransient<ITopicService, TopicService>()
    ;

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(context =>
    {
        context.Response.StatusCode = 500;
        return context.Response.WriteAsync("An unexpected fault happened.");
    });
});

app.UseSwagger().UseSwaggerUI();

app.MapControllers();

app.MapGet("/", () => "Hello World!");

app.Run();