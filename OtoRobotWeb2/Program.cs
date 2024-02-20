using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using OtoRobotWeb2.Helpers;
using OtoRobotWeb2.Models;
using OtoRobotWeb2.Models.Inputs;
using OtoRobotWeb2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
    builder.Services.AddDbContext<DataContext>(options =>
      options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection")));
    builder.Services.AddScoped<OfferService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x =>
{
    //x.Cookie.Name = "NetCoreMvc.Auth";
    x.Cookie.HttpOnly = true;
    x.LoginPath = "/Login/index";
    //x.ExpireTimeSpan= TimeSpan.FromSeconds(15);
    x.SlidingExpiration = true;
});
builder.Services.AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("AdminOnly"));
    options.AddPolicy("SuperAdmin", policy => policy.RequireClaim("SuperAdmin"));

});

var app = builder.Build(); 
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "File")),
    RequestPath = "/StaticFiles",

});
 


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Login/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
Tools.TvmDoldur();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
