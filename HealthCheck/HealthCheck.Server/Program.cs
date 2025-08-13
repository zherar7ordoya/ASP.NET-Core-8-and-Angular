/* Instantiate a WebApplicationBuilder */
var builder = WebApplication.CreateBuilder(args);

/* Add some services */
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* Use the builder to create a WebApplication object */
var app = builder.Build();

/* Configure the app with the required middleware */
app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");

/* Run the app */
app.Run();
