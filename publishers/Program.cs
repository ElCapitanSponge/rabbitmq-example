using AspNetCore.Swagger.Themes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(ModernStyle.Dark, options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "RabbitMQ Example Publishers");
    });
    /* app.MapScalarApiReference(options => */
    /* { */
    /* 	options */
    /* 		.WithTitle("RabbitMQ Example Publishers") */
    /* 		.WithTheme(ScalarTheme.Kepler) */
    /* 		.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient); */
    /* }); */
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
