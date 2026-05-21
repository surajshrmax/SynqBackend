using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Synq.Api.Hubs;
using Synq.Api.Middlewares;
using Synq.Api.Realtime;
using Synq.Application.Common.Interfaces;
using Synq.Application.DependencyInjection;
using Synq.Infrastructure.DependencyInjection;
using Synq.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR()
  .AddStackExchangeRedis(
      builder.Configuration.GetSection("Redis")["ConnectionString"]!,
      options => options.Configuration.ChannelPrefix = RedisChannel.Literal("Synq"));

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<IRealTimeMessageNotifier, RealtimeMessageNotifier>();
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

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetSection("Redis")["ConnectionString"]!));


builder.Logging.AddFilter("Microsoft.AspNetCore.SignalR", LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Http.Connections", LogLevel.Debug);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();
app.MapHub<MessageHub>("/messageHub");
app.Run();
