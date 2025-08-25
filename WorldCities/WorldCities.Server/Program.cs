using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using WorldCities.Server.Data;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    //options.JsonSerializerOptions.WriteIndented = true;
    //options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(
    options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

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

ExcelPackage.License.SetNonCommercialPersonal("Gerardo Tordoya");

app.Run();
