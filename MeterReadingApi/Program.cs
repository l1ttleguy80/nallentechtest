using MeterReadingApi.Options;
using MeterReadingApi.Services.CsvService;
using MeterReadingApi.Services.CustomerService;
using MeterReadingApi.Services.MeterReadingService;
using MeterReadingApi.Services.SupplierDatabase;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOptions();
builder.Services.Configure<DatabaseOptions>(builder.Configuration.GetSection("Database"));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddSingleton<ICsvService, CsvService>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IMeterReadingService, MeterReadingService>();

builder.Services.AddTransient<ISupplierDatabase, SupplierDatabase>(sc =>
{
    return new SupplierDatabase(sc.GetService<IOptions<DatabaseOptions>>()?.Value.ConnectionString ?? string.Empty);
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

