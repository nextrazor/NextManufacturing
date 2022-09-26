using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Localization;
using NextBackend.Controllers;
using NextBackend.DAL;

namespace NextBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Create CORS policy _myAllowSpecificOrigins
            var  MyAllowAllOrigins = "_myAllowAllOrigins";
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowAllOrigins,
                    policy  =>
                    {
                        policy.AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            // Add services to the container.

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
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
                // Add MyAllowAllOrigins CORS policy to app
                app.UseCors(MyAllowAllOrigins);
                
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            var supportedCultures = new[] { "en", "ru" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);

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