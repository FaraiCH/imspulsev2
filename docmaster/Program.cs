
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using docmaster.Data;
using docmaster.Areas.Identity.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Syncfusion.EJ2;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("OTk5NkAzMjMwMkUzMjJFMzBNWndBMi9jT0t0OVJ4Q2FFSGlhSGJ6aW8vTkhhS1FBSjd4dmw2eGZsTTFNPQ==");

string LData = "PExpY2Vuc2U+CjxEYXRhPgo8TGljZW5zZWRUbz5BdmVQb2ludDwvTGljZW5zZWRUbz4KPEVtYWlsVG8+aXRfYmlsbGluZ0BhdmVwb2ludC5jb208L0VtYWlsVG8+CjxMaWNlbnNlVHlwZT5EZXZlbG9wZXIgT0VNPC9MaWNlbnNlVHlwZT4KPExpY2Vuc2VOb3RlPkxpbWl0ZWQgdG8gMSBkZXZlbG9wZXIsIHVubGltaXRlZCBwaHlzaWNhbCBsb2NhdGlvbnM8L0xpY2Vuc2VOb3RlPgo8T3JkZXJJRD4xOTA1MjAwNzE1NDY8L09yZGVySUQ+CjxVc2VySUQ+MTU0ODI2PC9Vc2VySUQ+CjxPRU0+VGhpcyBpcyBhIHJlZGlzdHJpYnV0YWJsZSBsaWNlbnNlPC9PRU0+CjxQcm9kdWN0cz4KPFByb2R1Y3Q+QXNwb3NlLlRvdGFsIGZvciAuTkVUPC9Qcm9kdWN0Pgo8L1Byb2R1Y3RzPgo8RWRpdGlvblR5cGU+RW50ZXJwcmlzZTwvRWRpdGlvblR5cGU+CjxTZXJpYWxOdW1iZXI+Y2JmMzVkNWYtOWE2Ni00ZTI4LTg1ZGQtM2ExN2JiZTM0MTNhPC9TZXJpYWxOdW1iZXI+CjxTdWJzY3JpcHRpb25FeHBpcnk+MjAyMDA2MDQ8L1N1YnNjcmlwdGlvbkV4cGlyeT4KPExpY2Vuc2VWZXJzaW9uPjMuMDwvTGljZW5zZVZlcnNpb24+CjxMaWNlbnNlSW5zdHJ1Y3Rpb25zPmh0dHBzOi8vcHVyY2hhc2UuYXNwb3NlLmNvbS9wb2xpY2llcy91c2UtbGljZW5zZTwvTGljZW5zZUluc3RydWN0aW9ucz4KPC9EYXRhPgo8U2lnbmF0dXJlPnpqZDMrdWgzNTdiZHhqR3JWTTZCN3I2c250TkRBTlRXU2MyQi9RWS9hdmZxTnA0VHk5Z0kxR2V1NUdOaWVwRHArY1JrRFBMdjBDRTZ2MHNjYVZwK1JNTkF5SzdiUzdzeGZSL205Z0NtekFNUlptdUxQTm1laEtZVTNvOGJWVDJvWmRJeEY2dVRTMDhIclJxUnk5SWt6c3BxYmRrcEZFY0lGcHlLbDF2NlF2UT08L1NpZ25hdHVyZT4KPC9MaWNlbnNlPg==";

Stream stream = new MemoryStream(Convert.FromBase64String(LData));
Stream stream2 = new MemoryStream(Convert.FromBase64String(LData));
Stream stream3 = new MemoryStream(Convert.FromBase64String(LData));

stream.Seek(0, SeekOrigin.Begin);
new Aspose.Cells.License().SetLicense(stream);
new Aspose.Words.License().SetLicense(stream2);
new Aspose.Slides.License().SetLicense(stream3);
new Aspose.Pdf.License().SetLicense(stream);
var builder = WebApplication.CreateBuilder(args);
var connetionString = builder.Configuration.GetConnectionString("Default");

// Adding CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy",
        builder => builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());
});
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseMySql(connetionString, ServerVersion.AutoDetect(connetionString)));

builder.Services.AddDefaultIdentity<docmasterUser>(options => options.SignIn.RequireConfirmedAccount = true).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();
builder.Services.AddMvc().AddRazorPagesOptions(options =>
{
    options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "");
});
//DB Context
//builder.Services.AddDbContext<AppDbContext>(options => {
//    var connetionString = builder.Configuration.GetConnectionString("Default");
//    options.UseMySql(connetionString, ServerVersion.AutoDetect(connetionString));
//});

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

// using Microsoft.AspNetCore.HttpOverrides;

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseAuthentication();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// Shows UseCors with named policy.
app.UseCors("MyPolicy");
app.UseAuthentication();;

app.UseAuthorization();
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
 
    var context = services.GetRequiredService<AuthDbContext>();
    var userManager = services.GetRequiredService<UserManager<docmasterUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await ContextSeed.SeedSuperAdminAsync(userManager, roleManager);
 
}
app.Run();
