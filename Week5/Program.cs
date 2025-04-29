using Microsoft.EntityFrameworkCore;
using Week5.Data; // ← Proje namespace'ini burada doğru yaz!

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// 👉 EKLENEN KISIM: DbContext servisini ekle
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolDbConnection")));

// 👉 Session servisini ekle
builder.Services.AddSession(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 👉 Session middleware’i Authorization’dan önce gelmeli
app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
