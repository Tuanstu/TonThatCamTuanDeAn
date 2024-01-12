using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using TonThatCamTuanDeAn.Areas.Admin.Models.TaiKhoan;
using TonThatCamTuanDeAn.Models.Entity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient("Client").AddHttpMessageHandler<AuthHandled>(); //tao cai AuthHandled de no tu dong gan cai header vo cho cai httpClient do cua TaiKhoanController a
builder.Services.AddTransient<AuthHandled>();
// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(o => //khi no login vo thi se tu dong ghi cai token
    {
        o.LoginPath = "/tai-khoan/dang-nhap";
        o.AccessDeniedPath = "/tai-khoan/access-denied"; //tu dong kiem cai role trong token voi cai role dc Authorize; ko khop thi di toi "/tai-khoan/access-denied"
        o.LogoutPath = "/tai-khoan/logout";
    });

builder.Services.AddDbContext<TonThatCamTuanDeAnContext>(
    c => c.UseSqlServer(builder.Configuration.GetConnectionString("connectString"))
);
var app = builder.Build();

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
app.UseAuthentication();
app.UseAuthorization();

//Mo Trang hien len dau tien, co the chon cai area luc moi khoi tao
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
