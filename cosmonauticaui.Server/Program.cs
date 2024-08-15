using cosmonauticaui.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Add CORS policies
var localhostOrigin = "_localhostOrigin";
builder.Services.AddCors(options =>
{
	options.AddPolicy(name: localhostOrigin,
		policy => policy.WithOrigins("https://localhost:5173")
	);
});
// Add services to the container.c
builder.Services.AddControllers();
builder.Services.AddSingleton<AzureBlobService>();
builder.Services.AddSingleton<CosmosDBService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();

	app.UseCors(localhostOrigin);
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
