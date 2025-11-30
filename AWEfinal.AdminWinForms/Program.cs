using System.Windows.Forms;
using AWEfinal.BLL.Services;
using AWEfinal.DAL;
using AWEfinal.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AWEfinal.AdminWinForms
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            ApplicationConfiguration.Initialize();

            var builder = Host.CreateApplicationBuilder();
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var connectionString = builder.Configuration.GetConnectionString("LocalDBConn")
                ?? throw new InvalidOperationException("Connection string 'LocalDBConn' not found.");

            builder.Services.AddDbContext<AWEfinalDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddScoped<LoginForm>();
            builder.Services.AddScoped<DashboardForm>();

            using var host = builder.Build();
            using var scope = host.Services.CreateScope();
            var loginForm = scope.ServiceProvider.GetRequiredService<LoginForm>();
            Application.Run(loginForm);
        }
    }
}
