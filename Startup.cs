using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using webAPIAutores.Filters;
using webAPIAutores.Middlewares;
using webAPIAutores.Servicios;
using webAPIAutores.Utilidades;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace webAPIAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers(opciones =>
            {
                opciones.Conventions.Add(new SwaggerAgruparPorVersion());
            }
            ).AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opciones => opciones.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Configuration["llaveJWT"])),
                ClockSkew = TimeSpan.Zero
            });

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WebApiAutores",
                    Version = "v1",
                    Description = "Esta es una API para gestionar libros y sus autores",
                    Contact = new OpenApiContact
                    {
                        Name = "Saiter Bello",
                        Email = "saiterbellomateo@gmail.com",
                        Url = new Uri("https://MyLandingPage.com")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Licencia: @Ni de conducir"
                    },
                    TermsOfService = new Uri("https://NoAceptamosLoleros.com")
                });

                c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebApiAutores", Version = "v2" });

                c.OperationFilter<AgregarParametroXVersion>();
                c.OperationFilter<AgregarParametroHATEOAS>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string []{}
                    }
                });

                var archivoXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var rutaXML = Path.Combine(AppContext.BaseDirectory, archivoXML);
                c.IncludeXmlComments(rutaXML);

            });

            services.AddAutoMapper(typeof(Startup));

            services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddAuthorization(opciones =>
            {
                opciones.AddPolicy("EsAdminNoBulto", politica => politica.RequireClaim("joderSoyAdmin"));
                opciones.AddPolicy("EsVendedor", politica => politica.RequireClaim("soyVendedor"));
            });

            services.AddDataProtection();
            services.AddTransient<HashService>();

            services.AddCors(opciones =>
            {
                opciones.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("https://www.apirequest.io").AllowAnyMethod().AllowAnyHeader()
                    .WithExposedHeaders(new string[] { "cantidadTotalRegistros" });
                });
            });

            services.AddTransient<GestionadorDeEnlaces>();
            services.AddTransient<HATEOASAutorFilterAttribute>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddApplicationInsightsTelemetry(Configuration["ApplicationInsights:ConnectionString"]);
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {

            app.UseLogResponseHTTP();



            if (env.IsDevelopment())
            {

            }
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAutores v1");
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "WebApiAutores v2");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
