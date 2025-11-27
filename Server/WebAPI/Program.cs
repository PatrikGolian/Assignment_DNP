
using EfcRepositories;
using FileRepositories;
using RepositoryContracts;
using AppContext = EfcRepositories.AppContext;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IPostRepository, EfcPostRepository>();
builder.Services.AddScoped<IUserRepository, EfcUserRepository>();
builder.Services.AddScoped<ICommentRepository, EfcCommentRepository>();
builder.Services.AddDbContext<AppContext>();

var app = builder.Build();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.Run();

//https://localhost:5154/openapi/v1.json
//https://localhost:7162/swagger/index.html