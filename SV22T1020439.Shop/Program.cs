using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews()
    .AddMvcOptions(option =>
    {
        option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.Cookie.Name = "CustomerAuth";
        option.LoginPath = "/Account/Login";
        option.AccessDeniedPath = "/Account/Error";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Khởi tạo cấu hình cho BusinessLayer
string connectionString = builder.Configuration.GetConnectionString("LiteCommerceDB");
SV22T1020439.BusinessLayers.Configuration.Initialize(connectionString);

var app = builder.Build();

// Thiết lập ApplicationContext
IHttpContextAccessor httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
IWebHostEnvironment webHostEnvironment = app.Services.GetRequiredService<IWebHostEnvironment>();
IConfiguration configuration = app.Configuration;
SV22T1020439.Shop.ApplicationContext.Configure(httpContextAccessor, webHostEnvironment, configuration);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

// Cấu hình định dạng ngày tháng, tiền tệ... (Culture)
var cultureInfo = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Serve images from Admin project FIRST to ensure we get the latest data from Admin
string adminProductsPath = Path.Combine(app.Environment.ContentRootPath, "..", "SV22T1020439.Admin", "wwwroot", "images", "products");
if (Directory.Exists(adminProductsPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(adminProductsPath),
        RequestPath = "/images/products"
    });
}

app.UseStaticFiles(); // Serve local static files (wwwroot) SECOND

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
