using Lisa.AI.Blazor.Server.Components;
using Lisa.AI.Config;
using Lisa.AI.Config.ModelSettings;
using Lisa.AI.FunctionCall;
using Lisa.AI.Middleware;
using Lisa.AI.Services;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace Lisa.AI.Blazor.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            #region ADD FOR THIS PROJECT

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Configure services
            builder.Services.Configure<List<LLmModelSettings>>(
                builder.Configuration.GetSection(nameof(LLmModelSettings))
            );
            builder.Services.Configure<List<ToolPromptConfig>>(
                builder.Configuration.GetSection(nameof(ToolPromptConfig))
            );
            // Initialize global settings
            GlobalSettings.InitializeGlobalSettings(builder.Configuration);
            // Configure ApiKey
            var apiKey = builder.Configuration.GetValue<string>("ApiKey");

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Lisa.AI.Blazor.Server",
                    Version = "v1",
                    Description = "Lisa.AI.Blazor.Server API",
                    
                });

                // Ensure XML comments are optional for Swagger UI to function
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                if (File.Exists(Path.Combine(AppContext.BaseDirectory, xmlFilename)))
                {
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                }
                // Add model comments
                var xmlModelPath = Path.Combine(AppContext.BaseDirectory, "Lisa.AI.xml");
                options.IncludeXmlComments(xmlModelPath);

                if (string.IsNullOrEmpty(apiKey)) return;

                var securityScheme = new OpenApiSecurityScheme()
                {
                    Description = "API Key Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {API_KEY}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                };
                options.AddSecurityDefinition("API_KEY", securityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "API_KEY"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            builder.Services.AddSingleton<ILLmModelService, LLmModelDecorator>();
            builder.Services.AddSingleton<ToolPromptGenerator>();

            // CORS Configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowCors",
                    policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()
                );
            });

            // HttpClient
            builder.Services.AddHttpClient();
            #endregion


            var app = builder.Build();

            #region ADD FOR THIS PROJECT
            app.UseCors();

            
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Lisa.AI.Blazor.Server API v1");
                options.RoutePrefix = "swagger-ui"; // Optional, change this to your desired URL prefix
            });


            if (!string.IsNullOrEmpty(apiKey))
            {
                app.Use(async (context, next) =>
                {
                    var found = context.Request.Headers.TryGetValue("Authorization", out var key);
                    // If not found, try retrieving "api-key" header
                    if (!found)
                    {
                        found = context.Request.Headers.TryGetValue("api-key", out key);
                    }

                    key = key.ToString().Split(" ")[^1];

                    if (found && key == apiKey)
                    {
                        await next(context);
                    }
                    else
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }
                });
            }

            // Handle "stop" parameter
            app.UseMiddleware<TypeConversionMiddleware>();
            app.MapControllers();
            app.UseRouting();
            #endregion



            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
