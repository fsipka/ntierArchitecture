using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using System;
using System.Globalization;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.RateLimiting;
using System.Xml.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Day.API.Middlewares;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using SipkaTemplate.API.Filters;
using SipkaTemplate.API.Middlewares;
using SipkaTemplate.Core.Models;
using SipkaTemplate.API.Modules;
using FluentValidation.AspNetCore;
using SipkaTemplate.Service.Mappings;
using SipkaTemplate.Core.Services;
using SipkaTemplate.Service.Services;
using FluentValidation;
using SipkaTemplate.Service.Validations;
using SipkaTemplate.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(x => _ = true)));

builder.Services.AddControllers().AddNewtonsoftJson(o =>
{
    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});


//AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

builder.Services.AddDbContext<AppDbContext>(x =>
{
    var appDbContextAssembly = Assembly.GetAssembly(typeof(AppDbContext));
    if (appDbContextAssembly == null)
        throw new InvalidOperationException("AppDbContext assembly bulunamadı.");

    x.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), option =>
    {
        option.MigrationsAssembly(appDbContextAssembly.GetName().Name);
        //option.UseCompatibilityLevel(130);
    });
});

#region Localization

builder.Services.AddLocalization();

var localizationOptions = new RequestLocalizationOptions();
var supportedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("tr-TR"),
    new CultureInfo("de-DE")
};

localizationOptions.SupportedCultures = supportedCultures;
localizationOptions.SupportedUICultures = supportedCultures;
localizationOptions.SetDefaultCulture("tr-TR");
localizationOptions.ApplyCurrentCultureToResponseHeaders = true;

#endregion

#region Custom Update

#region Base 
builder.Services.AddScoped<ICustomUpdateService<User>, CustomUpdateService<User>>(); 
#endregion

#endregion

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var securityKey = builder.Configuration["Token:SecurityKey"]
    ?? throw new InvalidOperationException("Token:SecurityKey config değeri bulunamadı.");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Token:Issuer"],
        ValidAudience = builder.Configuration["Token:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey)),
        ClockSkew = TimeSpan.Zero,

    };
});

builder.Services.Configure<ApiBehaviorOptions>(options =>
{ options.SuppressModelStateInvalidFilter = true; });

builder.Services.AddSignalR();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SipkaTemplate API", Version = "v1" });


    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
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
            new string[] {}
        }
    });
});

builder.Services.AddRateLimiter(op =>
{
    op.AddFixedWindowLimiter("Basic", _options =>
    {
        _options.Window = TimeSpan.FromSeconds(10);
        _options.PermitLimit = 100;
        _options.QueueLimit = 10;
        _options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;

    });
});

builder.Services.AddOutputCache(op =>
{
    op.AddBasePolicy(builder => { builder.Expire(TimeSpan.FromSeconds(10)); });
    //op.AddPolicy("custom", c => { c.Expire(TimeSpan.FromDays(3)); });
});
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateFilterAttribute>();
});
builder.Services.AddFluentValidationAutoValidation();
//ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");
var validatorAssembly = typeof(ForgotPasswordDtoValidator).Assembly;
builder.Services.AddValidatorsFromAssembly(validatorAssembly);

builder.Services.AddScoped(typeof(NotFoundFilter<>)); 
builder.Services.AddAutoMapper(typeof(MapProfile));
builder.Services.AddHttpContextAccessor();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
    containerBuilder.RegisterModule(new RepoServiceModule()));

var app = builder.Build();

app.UseExceptionHandler("/error");

app.UseCustomException();

app.Map("/error", (HttpContext context) =>
{
    return Results.Problem("Default override");
});


app.UseRequestLocalization(localizationOptions);
app.UseRateLimiter();
app.UseOutputCache();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "contents")),
    RequestPath = "/contents"
});


app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<UserMiddleware>();
app.UseMiddleware<QueryStringBlocker>();

app.MapControllers(); 

app.Run();