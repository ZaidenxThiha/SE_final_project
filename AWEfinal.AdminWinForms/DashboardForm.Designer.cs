#nullable disable
using System.ComponentModel;

namespace AWEfinal.AdminWinForms
{
    partial class DashboardForm
    {
        private IContainer components = null!;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();
            SuspendLayout();
            BuildLayout();
            ResumeLayout(false);
        }
    }
}
