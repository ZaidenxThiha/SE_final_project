#nullable disable

namespace AWEfinal.AdminWinForms
{
    partial class LoginForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            statusLabel = new Label();
            txtUsername = new TextBox();
            txtPassword = new TextBox();
            btnLogin = new Button();
            SuspendLayout();
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(237, 238, 206);
            ClientSize = new Size(780, 540);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AWE Electronics - Admin Portal";
            // 
            // Header panel
            // 
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 130,
                BackColor = Color.FromArgb(7, 54, 52),
                Padding = new Padding(24, 20, 24, 16)
            };

            var headerTitle = new Label
            {
                Text = "AWE Admin",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold, GraphicsUnit.Point),
                AutoSize = true,
                Dock = DockStyle.Top
            };

            var headerSubtitle = new Label
            {
                Text = "Secure access to manage products, orders and analytics",
                ForeColor = Color.Gainsboro,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point),
                AutoSize = true,
                Dock = DockStyle.Top,
                Padding = new Padding(0, 6, 0, 0)
            };

            headerPanel.Controls.Add(headerSubtitle);
            headerPanel.Controls.Add(headerTitle);
            Controls.Add(headerPanel);

            // 
            // Card panel
            // 
            var cardPanel = new Panel
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(30),
                Size = new Size(640, 350),
                Location = new Point((ClientSize.Width - 640) / 2, 150)
            };
            cardPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            var introLabel = new Label
            {
                Text = "Sign in to continue",
                AutoSize = true,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(7, 54, 52),
                Dock = DockStyle.Top
            };

            var formLayout = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 3,
                Dock = DockStyle.Top,
                Padding = new Padding(0, 20, 0, 10),
                AutoSize = true
            };
            formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // label1 - username
            label1.AutoSize = true;
            label1.Text = "Admin Email";
            label1.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Padding = new Padding(0, 10, 0, 6);

            txtUsername.Margin = new Padding(3);
            txtUsername.Width = 320;
            txtUsername.PlaceholderText = "admin@electrostore.com";
            txtUsername.BorderStyle = BorderStyle.FixedSingle;

            // label2 - password
            label2.AutoSize = true;
            label2.Text = "Password";
            label2.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Padding = new Padding(0, 10, 0, 6);

            txtPassword.Margin = new Padding(2);
            txtPassword.Width = 320;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.PlaceholderText = "Enter password";
            txtPassword.BorderStyle = BorderStyle.FixedSingle;

            btnLogin.Text = "Login";
            btnLogin.AutoSize = true;
            btnLogin.Padding = new Padding(14, 6, 14, 6);
            btnLogin.BackColor = Color.FromArgb(237, 238, 206); // match dashboard secondary buttons
            btnLogin.ForeColor = Color.Black;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderColor = Color.Black;
            btnLogin.FlatAppearance.BorderSize = 2;
            btnLogin.Margin = new Padding(6, 12, 3, 3);
            btnLogin.TextAlign = ContentAlignment.MiddleCenter;
            btnLogin.Click += btnLogin_Click;

            formLayout.Controls.Add(label1, 0, 0);
            formLayout.Controls.Add(txtUsername, 1, 0);
            formLayout.Controls.Add(label2, 0, 1);
            formLayout.Controls.Add(txtPassword, 1, 1);
            formLayout.Controls.Add(btnLogin, 1, 2);

            // statusLabel
            statusLabel.AutoSize = false;
            statusLabel.TextAlign = ContentAlignment.MiddleCenter;
            statusLabel.Dock = DockStyle.Bottom;
            statusLabel.Height = 50;
            statusLabel.ForeColor = Color.DimGray;

            cardPanel.Controls.Add(statusLabel);
            cardPanel.Controls.Add(formLayout);
            cardPanel.Controls.Add(introLabel);
            Controls.Add(cardPanel);

            // label3 - footer credits
            label3.AutoSize = true;
            label3.ForeColor = Color.Gray;
            label3.Text = "Developed by Thiha and Sandi";
            label3.Dock = DockStyle.Bottom;
            label3.TextAlign = ContentAlignment.MiddleCenter;
            label3.Padding = new Padding(0, 0, 0, 10);
            Controls.Add(label3);

            AcceptButton = btnLogin;
            ResumeLayout(false);
        }

        #endregion
    }
}
