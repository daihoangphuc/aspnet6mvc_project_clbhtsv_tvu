﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Configuration;
using website_CLB_HTSV;
using website_CLB_HTSV.Data;
using website_CLB_HTSV.Services;
using static Dropbox.Api.TeamLog.EventCategory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();


builder.Services.AddSingleton<IEmailSender, EmailSender>();

//Add signalr
builder.Services.AddSignalR();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "AspNetCore.Identity.Application";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.SlidingExpiration = true;
});

// Add session services
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian sống của Session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Đánh dấu cookie session là thiết yếu cho ứng dụng
});

// Thiết lập giấy phép cho EPPlus
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseSession();
app.UseMiddleware<SessionTimeoutMiddleware>();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Thêm dòng sau để đảm bảo route cho action Search
    endpoints.MapControllerRoute(
        name: "newsSearch",
        pattern: "News/Search",
        defaults: new { controller = "TinTucs", action = "Search" });
    // Thêm dòng sau để đảm bảo route cho action Search
    endpoints.MapControllerRoute(
        name: "sinhVienSearch",
        pattern: "SinhViens/Search",
        defaults: new { controller = "SinhViens", action = "Search" });
    endpoints.MapHub<ChatHub>("/chatHub");
});

app.MapRazorPages();

app.Run();
