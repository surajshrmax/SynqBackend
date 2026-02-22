using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Synq.Api.Hubs;
using Synq.Application.Common.Interfaces;
using Synq.Application.DependencyInjection;
using Synq.Infrastructure.DependencyInjection;
using Synq.Infrastructure.Identity;
using Synq.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
  };

  options.Events = new JwtBearerEvents
  {
    OnMessageReceived = context =>
    {
      var accessToken = context.Request.Query["access_token"];
      var path = context.HttpContext.Request.Path;

      if (string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/messageHub"))
      {
        context.Token = accessToken;
      }

      return Task.CompletedTask;
    }
  };
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

  if (db is AppDbContext concretDb)
  {
    concretDb.Database.Migrate();
    TempDbSeeder.Seed(concretDb);
  }
}

app.MapControllers();
app.MapHub<MessageHub>("/messageHub");
app.UseAuthentication();
app.UseAuthorization();
app.Run();
