using Ecommerce.Application.Mappings;
using Ecommerce.Application.Interfaces;
using Ecommerce.Application.Services;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

// FluentValidation — এই Assembly তে থাকা সব Validator ক্লাস auto-register হবে
builder.Services.AddValidatorsFromAssembly(typeof(MappingProfile).Assembly);

// Application Services — Scoped
builder.Services.AddScoped<IProductService, ProductService>();

// ⚠️ IUnitOfWork এখনো register করা হয়নি — Infrastructure layer (E3) বানানোর পর যোগ হবে:
// builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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