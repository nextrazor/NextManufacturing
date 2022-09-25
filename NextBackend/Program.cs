using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Localization;
using NextBackend.DAL;

namespace NextBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddLocalization();
            builder.Services.AddDbContext<NmaContext>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error ??
                    throw new Exception("Error while exception handling");
                await context.Response.WriteAsJsonAsync(new
                {
                    error = exception.Message,
                    stack = exception.StackTrace?.Split("\r\n"),
                    source = exception.Source,
                    data = exception.Data
                });
            }));

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}