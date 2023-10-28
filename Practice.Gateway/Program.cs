using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Practice.Gateway.Aggregator;

var builder = WebApplication.CreateBuilder(args);
app.useStaticfiles()
// Add services to the container.
builder.Configuration.AddJsonFile("ocelot.json");
// Add services to the container.
builder.Services.AddOcelot()
    .AddSingletonDefinedAggregator<UsersPostsAggregator>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
await app.UseOcelot();
app.MapControllers();

app.Run();
