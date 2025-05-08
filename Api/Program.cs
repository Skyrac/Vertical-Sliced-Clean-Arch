using System.Text;
using Application.Extensions;
using Carter;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder
    .Services.AddCarter()
    .AddApplicationRegistration()
    .AddInfrastructureRegistration(builder.Configuration);

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "SportNest",
            ValidAudience = "SportNest",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("Your-Secret-Key-Here")
            ),
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.MapScalarApiReference();

app.MapCarter();

app.Run();
