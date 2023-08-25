global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.IdentityModel.Tokens;
global using System.Text;
global using System.IdentityModel.Tokens.Jwt;
global using System.Security.Claims;
global using System.Security.Cryptography;
global using API_Test1.DbContext;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Mvc;
global using System.ComponentModel.DataAnnotations;
global using API_Test1.Models.Entities;
global using API_Test1.Constant;
global using MimeKit.Text;
global using MimeKit;
global using API_Test1.Services.AccountServices;
global using API_Test1.Services.MailServices;
global using API_Test1.Models.ViewModels;
global using MailKit.Net.Smtp;
global using Microsoft.AspNetCore.Authorization;
global using API_Test1.Models.DTOs;
using Microsoft.AspNetCore.CookiePolicy;
using API_Test1.Services.FileServices;
using API_Test1.Services.ProductServices;
using API_Test1.Services.ProductServices.ProductTypeServices.ProductTypeServices;
using API_Test1.Services.CartServices;
using API_Test1.Services.OrderServices;
using API_Test1.Services.OrderstatusServices;
using API_Test1.Services.JwtServices;
using API_Test1.Services.PaymentServices.MOMO;
using API_Test1.Services.PaymentServices;
using API_Test1.Services.ProductServices.ProductImageServices;
using API_Test1.Services.PaymentServices.MOMO.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//Entity FW
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConnStr")));
//life cycle
builder.Services.AddScoped<IAccountServices, AccountServices>();
builder.Services.AddScoped<IMailServices, MailServices>();
builder.Services.AddScoped<IFileServices, FileServices>();
builder.Services.AddScoped<IProductServices, ProductService>();
builder.Services.AddScoped<IProductTypeServices, ProductTypeServices>();
builder.Services.AddScoped<ICartServices, CartServices>();
builder.Services.AddScoped<IOrderServices, OrderServices>();
builder.Services.AddScoped<IJwtServices, JwtServices>();
builder.Services.AddScoped<IMoMoServices, MoMoServices>();
builder.Services.AddScoped<IPaymentServices, PaymentServices>();
builder.Services.AddScoped<IOrderstatusServices, OrderstatusServices>();
builder.Services.AddScoped<IProductImageServices, ProductImageServices>();

//http
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
//cookies policy
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "Login";
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.Secure = CookieSecurePolicy.Always;
});
//configure
builder.Services.Configure<MomoOptionModel>(builder.Configuration.GetSection("MomoAPI"));
///identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

//adding authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    //adding jwtbearer
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    });
//add cors
string[] urlCors = new string[]
{
    "https://localhost:44311",
    "https://test-payment.momo.vn",
    "https://localhost:7203",
    "http://localhost:5070"
};
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(urlCors)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors();
app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
