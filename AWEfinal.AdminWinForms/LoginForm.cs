using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using AWEfinal.BLL.Services;
using AWEfinal.DAL.Models;
using Microsoft.Extensions.DependencyInjection;

namespace AWEfinal.AdminWinForms
{
    public partial class LoginForm : Form
    {
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;

        public LoginForm(IUserService userService, IServiceProvider serviceProvider)
        {
            _userService = userService;
            _serviceProvider = serviceProvider;
            InitializeComponent();
        }

        private async void btnLogin_Click(object? sender, EventArgs e)
        {
            await HandleLoginAsync();
        }

        private async Task HandleLoginAsync()
        {
            var email = txtUsername.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                statusLabel.ForeColor = Color.Firebrick;
                statusLabel.Text = "Email and password are required.";
                return;
            }

            btnLogin.Enabled = false;
            statusLabel.ForeColor = Color.DimGray;
            statusLabel.Text = "Validating credentials...";

            try
            {
                var user = await _userService.LoginUserAsync(email, password);
                var isAdmin = user != null && user.Role.Equals("admin", StringComparison.OrdinalIgnoreCase);
                var isStaff = user != null && user.Role.Equals("staff", StringComparison.OrdinalIgnoreCase);

                if (!isAdmin && !isStaff)
                {
                    statusLabel.ForeColor = Color.Firebrick;
                    statusLabel.Text = "Invalid credentials or insufficient privileges.";
                    return;
                }

                LaunchDashboard(user);
            }
            catch (Exception ex)
            {
                statusLabel.ForeColor = Color.Firebrick;
                statusLabel.Text = $"Login failed: {ex.Message}";
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private void LaunchDashboard(User user)
        {
            var dashboard = _serviceProvider.GetRequiredService<DashboardForm>();
            dashboard.SetLoggedInUser(user);
            dashboard.FormClosed += (_, _) => Close();
            Hide();
            dashboard.Show();
        }
    }
}
