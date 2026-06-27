using E_commerce.Services;
using E_commerceEntity;
using E_commerceEntity.DataBase;
using E_commerceEntity.Entity;
using E_commerceEntity.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace E_commerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            builder.Services.AddDbContext<E_CommerceContext>(
                s =>
                {
                    s.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);

                    s.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLServer"));
                });

            builder.Services.AddIdentity<APP_User, IdentityRole>(options =>
            {

                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 3;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;

            })
.AddEntityFrameworkStores<E_CommerceContext>()
.AddDefaultTokenProviders();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<SessionCartService>();
            builder.Services.AddScoped<DatabaseCartService>();
            builder.Services.AddScoped<IOrderService, OrderService>();


            builder.Services.AddScoped<ICartService, CartService>();



            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(7);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapRazorPages();
            app.MapControllerRoute(


                  name: "areas",
                  pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"

            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();



            app.Run();
        }
    }
}
