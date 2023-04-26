using eventRadar.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using eventRadar.Auth.Model;
using eventRadar.Data;
using eventRadar.Data.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.EntityFrameworkCore;
using eventRadar;
using eventRadar.DB;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddControllers();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<WebDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<WebDbContext>();

builder.Services.AddAuthentication(configureOptions: options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters.ValidAudience = builder.Configuration["JWT:ValidAudience"];
        options.TokenValidationParameters.ValidIssuer = builder.Configuration["JWT:ValidIssuer"];
        options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]));
    });

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IBlacklistedPageRepository, BlacklistedPageRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IChangedEventRepository, ChangedEventRepository>();
builder.Services.AddTransient<IEventRepository, EventRepository>();
builder.Services.AddTransient<IFollowedEventRepository, FollowedEventRepository>();
builder.Services.AddTransient<ILocationRepository, LocationRepository>();
builder.Services.AddTransient<IVisitedEventRepository, VisitedEventRepository>();
builder.Services.AddTransient<IWebsiteRepository, WebsiteRepository>();
builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<AuthDbSeeder>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.ResourceUser, policy => policy.Requirements.Add(new ResourceUserRequirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, ResourceUserAuthorizationHandler>();

builder.Services.AddCors(p => p.AddPolicy("corspolicy", build =>
{
    build.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();

app.UseCors("corspolicy");

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<WebDbContext>();

var dbSeeder = app.Services.CreateScope().ServiceProvider.GetRequiredService<AuthDbSeeder>();
await dbSeeder.SeedAsync();

app.Run();
