using Sam.GitHubApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Custom GitHub service
builder.Services.AddGitHubApi(x =>
{
    x.Token = builder.Configuration["GitHubApi:Token"];
    x.Owner = builder.Configuration["GitHubApi:Owner"];
    x.Repo = builder.Configuration["GitHubApi:Repo"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
