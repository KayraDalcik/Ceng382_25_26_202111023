using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Week5.Data;
using Week5.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔗 Veritabanı bağlantısı
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));

// 🔐 Identity (kullanıcı + rol desteği dahil)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<SchoolDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); // Login, Register, Logout UI'si

// 📄 Razor Pages
builder.Services.AddRazorPages();

// 🧠 Session desteği (isteğe bağlı ama sen kullanıyorsun)
builder.Services.AddSession();

var app = builder.Build();

// 🌐 Ortam kontrolü
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔐 Kimlik doğrulama ve yetkilendirme
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
