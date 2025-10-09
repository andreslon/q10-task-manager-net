using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Q10.TaskManager.Api.Configurations
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1.0.0",
                    Title = "TaskManager API",
                    Description = "API para la gestión de tareas y configuraciones del sistema TaskManager. " +
                                "Proporciona endpoints para la administración de configuraciones del entorno " +
                                "y monitoreo del estado de la aplicación.",
                    Contact = new OpenApiContact
                    {
                        Name = "Cedesistemas",
                        Email = "info@cedesistemas.com"
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }

                options.EnableAnnotations();
            });

            return services;
        }

        public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API v1");
                    options.RoutePrefix = "swagger";
                    options.DocumentTitle = "TaskManager API Documentation";
                    options.DisplayRequestDuration();
                });
            }

            return app;
        }
    }
}
