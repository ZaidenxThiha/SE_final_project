#nullable disable
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AWEfinal.AdminWinForms
{
    partial class ProductEditorForm
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
            _rootLayout = new TableLayoutPanel();
            _cardPanel = new Panel();
            _formLayout = new TableLayoutPanel();
            _footerPanel = new FlowLayoutPanel();
            _titleLabel = new Label();
            
            _nameInput = new TextBox();
            _categoryInput = new ComboBox();
            _priceInput = new TextBox();
            _stockInput = new NumericUpDown();
            _colorsInput = new TextBox();
            _featuresInput = new TextBox();
            _storageInput = new TextBox();
            _ratingInput = new NumericUpDown();
            _inStockCheck = new CheckBox();
            _descriptionInput = new TextBox();
            
            _saveButton = new Button();
            _cancelButton = new Button();
            
            ((ISupportInitialize)_stockInput).BeginInit();
            ((ISupportInitialize)_ratingInput).BeginInit();
            SuspendLayout();
            
            // 
            // root layout
            // 
            _rootLayout.AutoSize = true;
            _rootLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _rootLayout.ColumnCount = 1;
            _rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _rootLayout.Dock = DockStyle.Fill;
            _rootLayout.Padding = new Padding(32);
            _rootLayout.RowCount = 1;
            _rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _rootLayout.Controls.Add(_cardPanel, 0, 0);
            
            // 
            // card panel
            // 
            _cardPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _cardPanel.AutoScroll = true;
            _cardPanel.BackColor = Color.White;
            _cardPanel.Dock = DockStyle.Fill;
            _cardPanel.MinimumSize = new Size(400, 0);
            _cardPanel.Padding = new Padding(36);
            _cardPanel.Paint += CardPanel_Paint;
            _cardPanel.Controls.Add(_formLayout);
            _cardPanel.Controls.Add(_titleLabel);
            _cardPanel.Controls.Add(_footerPanel);
            
            // 
            // title
            // 
            _titleLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            _titleLabel.AutoSize = true;
            _titleLabel.Dock = DockStyle.Top;
            _titleLabel.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold, GraphicsUnit.Point);
            _titleLabel.Margin = new Padding(0, 0, 0, 24);
            
            // 
            // form layout
            // 
            _formLayout.AutoSize = true;
            _formLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _formLayout.Dock = DockStyle.Fill;
            _formLayout.ColumnCount = 2;
            _formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _formLayout.RowCount = 6;
            for (var i = 0; i < 6; i++)
            {
                _formLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }
            _formLayout.Padding = new Padding(0, 0, 0, 12);

            // Row 0
            AddFieldBlock("Product Name", _nameInput, 0, 0);
            AddFieldBlock("Price (USD)", _priceInput, 1, 0);

            // Row 1
            _stockInput.Minimum = 0;
            _stockInput.Maximum = 100000;
            _stockInput.DecimalPlaces = 0;
            AddFieldBlock("Stock Quantity", _stockInput, 0, 1);

            _categoryInput.DropDownStyle = ComboBoxStyle.DropDown;
            _categoryInput.Items.AddRange(new object[] { "Smartphones", "Laptops", "Tablets", "Accessories", "Wearables", "Audio" });
            AddFieldBlock("Category", _categoryInput, 1, 1);

            // Row 2
            _colorsInput.PlaceholderText = "e.g. Red, Blue, Black";
            AddFieldBlock("Colors (comma-separated)", _colorsInput, 0, 2);

            _featuresInput.PlaceholderText = "e.g. 5G, Waterproof, NFC";
            AddFieldBlock("Features (comma-separated)", _featuresInput, 1, 2);

            // Row 3
            AddFieldBlock("Storage", _storageInput, 0, 3);

            _ratingInput.Minimum = 0;
            _ratingInput.Maximum = 5;
            _ratingInput.DecimalPlaces = 1;
            _ratingInput.Increment = 0.1M;
            AddFieldBlock("Rating (0-5)", _ratingInput, 1, 3);

            // Row 4
            _descriptionInput.Multiline = true;
            _descriptionInput.MinimumSize = new Size(0, 120);
            _descriptionInput.ScrollBars = ScrollBars.Vertical;
            AddFieldBlock("Description", _descriptionInput, 0, 4, 2);

            // Row 5
            _inStockCheck.Text = "In Stock";
            _inStockCheck.AutoSize = true;
            _inStockCheck.Margin = new Padding(0, 12, 0, 0);
            _formLayout.Controls.Add(_inStockCheck, 0, 5);
            _formLayout.SetColumnSpan(_inStockCheck, 2);

            // 
            // footer
            // 
            _footerPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            _footerPanel.AutoSize = true;
            _footerPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _footerPanel.Dock = DockStyle.Bottom;
            _footerPanel.FlowDirection = FlowDirection.RightToLeft;
            _footerPanel.Padding = new Padding(0);
            _footerPanel.WrapContents = false;
            _footerPanel.Margin = new Padding(0, 24, 0, 0);

            _saveButton = CreatePrimaryButton("Save");
            _saveButton.Click += SaveButton_Click;
            _cancelButton = CreateSecondaryButton("Cancel");
            _cancelButton.DialogResult = DialogResult.Cancel;

            _footerPanel.Controls.Add(_saveButton);
            _footerPanel.Controls.Add(_cancelButton);

            // 
            // form settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Hex("#EDEECE");
            ClientSize = new Size(1000, 850);
            Controls.Add(_rootLayout);
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimumSize = new Size(900, 800);
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;
            AcceptButton = _saveButton;
            CancelButton = _cancelButton;
            
            ((ISupportInitialize)_stockInput).EndInit();
            ((ISupportInitialize)_ratingInput).EndInit();
            ResumeLayout(false);
        }

        private void AddFieldBlock(string labelText, Control input, int col, int row, int colSpan = 1)
        {
            var panel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 24, 16), // Right margin for spacing between columns
                WrapContents = false
            };

            var label = new Label
            {
                Text = labelText,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 6),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular)
            };

            input.Dock = DockStyle.None;
            input.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            input.Width = 400; // Initial width, will be stretched by TLP if Dock=Fill, but here we are in Flow. 
                               // Actually, to make it stretch in FlowLayoutPanel is tricky.
                               // Better to use TableLayoutPanel or simple Panel with Dock=Top.
            
            // Let's use a simple Panel instead of FlowLayoutPanel to ensure width stretching
            var container = new Panel
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, col == 0 ? 24 : 0, 16) // Add right margin only to first column
            };
            
            label.Dock = DockStyle.Top;
            input.Dock = DockStyle.Top;
            input.BringToFront(); // Ensure input is below label (Dock=Top stacks from top, so last added is bottom? No, first added is top)
            
            // Re-add in correct order for Dock=Top
            container.Controls.Add(input);
            container.Controls.Add(label);
            
            _formLayout.Controls.Add(container, col, row);
            if (colSpan > 1)
            {
                _formLayout.SetColumnSpan(container, colSpan);
            }
        }
    }
}
