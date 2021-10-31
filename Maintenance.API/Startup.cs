using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Maintenance.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSwaggerApiDocumentation()
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Audience = "https://iamdhanukagmail.onmicrosoft.com/maintenance.api";
                    options.Authority = "https://login.microsoftonline.com/iamdhanukagmail.onmicrosoft.com";
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        NameClaimType = "Id", // map custom "Id" claim to "User.Identity.Name", else User.Identity.Name will retrieve null
                        ValidateIssuer = true,
                        ValidIssuer = "https://login.microsoftonline.com/iamdhanukagmail.onmicrosoft.com",
                        ValidateAudience = true,
                        //ValidateIssuerSigningKey = true,
                        //IssuerSigningKey = new SymmetricSecurityKey(
                        //    Encoding.ASCII.GetBytes(authSettings[nameof(AuthSettings.SecretKey)])
                        //)
                        //, //signin key
                        RequireExpirationTime = false,
                        ValidateLifetime = true,
                        //ClockSkew = TimeSpan.Zero
                    };
                });
            services.AddHttpClient("placeholder_api", d =>
            {
                d.BaseAddress = new Uri("https://jsonplaceholder.typicode.com");
                d.Timeout = TimeSpan.FromSeconds(200); // default 100
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Maintenance API v1");
            });
        }

    }

    static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwaggerApiDocumentation(this IServiceCollection services)
        {
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Maintenance API",
                    Version = "v1",
                    Description = "An API to perform Maintenance operations",
                    Contact = new OpenApiContact
                    {
                        Name = "Dhanuka Jayasinghe",
                        Email = "hasitha2kandy@gmail.com"
                    }
                });
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Scheme = "Bearer",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }

                        },
                        new string[]{}
                    }
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}
