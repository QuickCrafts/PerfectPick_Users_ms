using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("_PerfectPickUsers_MSContextConnection") ?? throw new InvalidOperationException("Connection string '_PerfectPickUsers_MSContextConnection' not found.");



// Add services to the container.

builder.Configuration.AddJsonFile("appsettings.json");
var secretKey = builder.Configuration.GetSection("Settings").GetSection("secretKey").ToString();
if (string.IsNullOrEmpty(secretKey))
{
    throw new Exception("Secret key not found in enviornment");
}
var keyBytes = Encoding.UTF8.GetBytes(secretKey);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    config.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}
    ).AddJwtBearer(config =>
    {
        config.RequireHttpsMetadata = false;
        config.SaveToken = true;
        config.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    }).AddCookie()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = Environment.GetEnvironmentVariable("authClientID") ?? throw new Exception("Couldn't get google auth id");
        googleOptions.ClientSecret = Environment.GetEnvironmentVariable("authClientSecret") ?? throw new Exception("Couldn't get google auth secret");
    })
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.AppId = Environment.GetEnvironmentVariable("authFBClientID") ?? throw new Exception("Couldn't get facebook auth id");
        facebookOptions.AppSecret = Environment.GetEnvironmentVariable("authFBClientSecret") ?? throw new Exception("Couldn't get facebook auth secret");
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
