using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MiniMesTrainApi.Persistance;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MiniMesTrainApi.Repository;

namespace MiniMesTrainApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            string? connString = builder.Configuration.GetConnectionString("MiniProduction");
            if (connString == null)
            {
                Console.WriteLine("There is no ConnectionString called MiniProduction in appsettings.json");
            }
            else
            {
                builder.Services.AddDbContext<MiniProductionDbContext>(options => options.UseSqlServer(connString));
            }

            builder.Services.AddScoped<ProcessRepository, ProcessRepository>();
            builder.Services.AddScoped<OrderRepository, OrderRepository>();
            builder.Services.AddScoped<MachineRepository, MachineRepository>();
            builder.Services.AddScoped<ParameterRepository, ParameterRepository>();
            builder.Services.AddScoped<ProcessParameterRepository, ProcessParameterRepository>();
            builder.Services.AddScoped<ProductRepository, ProductRepository>();

            var app = builder.Build();
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options => 
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = string.Empty;
                });
            }

            app.UseCors("AllowAllOrigins");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
