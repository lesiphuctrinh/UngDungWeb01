using Microsoft.AspNetCore.Authentication.Cookies;
using SV21T1020777.Shop.AppCodes;

namespace SV21T1020777.Shop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllersWithViews()
                .AddMvcOptions(option =>
                {
                    option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
                });
            //Cấu hình Cookie-base
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(option =>
                            {
                                option.Cookie.Name = "ShopAuthenticationCookie";    //Tên Cookie
                                option.LoginPath = "/ShopAccount/Login";            //URL trang đăng nhập
                                option.AccessDeniedPath = "/ShopAccount/AccessDenined"; //URL đến trang cấm truy cập
                                option.ExpireTimeSpan = TimeSpan.FromDays(360);     //Thời gian "sống" của Cookie
                            });
            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(60);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/ShopHome/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=ShopHome}/{action=Index}/{id?}");
            ApplicationContext.Configure
                (
                context: app.Services.GetRequiredService<IHttpContextAccessor>(),
                enviroment: app.Services.GetRequiredService<IWebHostEnvironment>()

                );
            //khởi tạo cấu hình cho BusinessLayer
            string connectionString = builder.Configuration.GetConnectionString("LiteCommerceDB");
            SV21T1020777.BusinessLayers.Configuration.Initialize(connectionString);

            app.Run();
        }
    }
}
