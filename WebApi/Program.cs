using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BackendExample.Services;
using WebApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var jwtSection = builder.Configuration.GetSection("Jwt");
var supaSection = builder.Configuration.GetSection("Supabase");

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Http clients
builder.Services.AddHttpClient("supabase", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Supabase:Url"]);
    client.DefaultRequestHeaders.Add("apikey", builder.Configuration["Supabase:AnonKey"]);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["Supabase:AnonKey"]}");
});

builder.Services.AddHttpClient("inventory", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Microservices:InventoryServiceBaseUrl"] ?? "https://localhost:5002");
});

// App services
builder.Services.AddScoped<ISupabaseService, SupabaseService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMicroserviceClient, MicroserviceClient>();

// JWT Auth
var secret = builder.Configuration["Jwt:Secret"];
var key = Encoding.ASCII.GetBytes(secret!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

// Pipeline
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