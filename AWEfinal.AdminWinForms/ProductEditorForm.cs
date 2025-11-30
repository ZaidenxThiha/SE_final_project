using System.Globalization;
using System.Text.Json;
using System.Drawing;
using System.Windows.Forms;
using AWEfinal.DAL.Models;

namespace AWEfinal.AdminWinForms
{
    public partial class ProductEditorForm : Form
    {
        private readonly Product _model;

        private TextBox _nameInput = null!;
        private ComboBox _categoryInput = null!;
        private TextBox _priceInput = null!;
        private NumericUpDown _stockInput = null!;
        private TextBox _colorsInput = null!;
        private TextBox _featuresInput = null!;
        private TextBox _storageInput = null!;
        private NumericUpDown _ratingInput = null!;
        private CheckBox _inStockCheck = null!;
        private TextBox _descriptionInput = null!;
        private TableLayoutPanel _rootLayout = null!;
        private Panel _cardPanel = null!;
        private Label _titleLabel = null!;
        private TableLayoutPanel _formLayout = null!;
        private FlowLayoutPanel _footerPanel = null!;
        private Button _saveButton = null!;
        private Button _cancelButton = null!;

        public Product? Product { get; private set; }

        public ProductEditorForm(Product? existing = null)
        {
            _model = existing == null ? new Product() : Clone(existing);
            InitializeComponent();
            RefreshFormTitle();
            LoadProduct();
        }

        private static Color Hex(string value) => ColorTranslator.FromHtml(value);

        private void LoadProduct()
        {
            _nameInput.Text = _model.Name;
            _categoryInput.Text = _model.Category;
            _priceInput.Text = _model.Price == 0 ? string.Empty : _model.Price.ToString("F2");
            _stockInput.Value = _model.StockQuantity;
            
            _colorsInput.Text = ParseJsonList(_model.Colors);
            _featuresInput.Text = ParseJsonList(_model.Features);
            _storageInput.Text = _model.Storage;
            _ratingInput.Value = (decimal)_model.Rating;

            _inStockCheck.Checked = _model.InStock;
            _descriptionInput.Text = _model.Description;
        }

        private string ParseJsonList(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return string.Empty;
            try
            {
                var list = JsonSerializer.Deserialize<List<string>>(json);
                return list != null ? string.Join(", ", list) : string.Empty;
            }
            catch
            {
                return json; // Fallback if not valid JSON
            }
        }

        private void SaveProduct()
        {
            if (string.IsNullOrWhiteSpace(_nameInput.Text) || string.IsNullOrWhiteSpace(_categoryInput.Text) || string.IsNullOrWhiteSpace(_priceInput.Text))
            {
                MessageBox.Show("Name, Category, and Price are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(_priceInput.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out var price) || price <= 0)
            {
                MessageBox.Show("Enter a valid price greater than zero.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _model.Name = _nameInput.Text.Trim();
            _model.Category = _categoryInput.Text.Trim();
            _model.Price = price;
            _model.StockQuantity = (int)_stockInput.Value;
            _model.InStock = _inStockCheck.Checked;
            _model.Description = string.IsNullOrWhiteSpace(_descriptionInput.Text) ? "No description" : _descriptionInput.Text.Trim();
            
            _model.Colors = JsonSerializer.Serialize(_colorsInput.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            _model.Features = JsonSerializer.Serialize(_featuresInput.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            _model.Storage = _storageInput.Text.Trim();
            _model.Rating = _ratingInput.Value;

            _model.Images ??= "[]";

            Product = _model;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void RefreshFormTitle()
        {
            var isNew = _model.Id == 0;
            Text = isNew ? "Create Product" : "Edit Product";
            _titleLabel.Text = isNew ? "New Product" : "Edit Product";
            _saveButton.Text = isNew ? "Create Product" : "Save Changes";
        }

        private void SaveButton_Click(object? sender, EventArgs e) => SaveProduct();

        private void CardPanel_Paint(object? sender, PaintEventArgs e)
        {
            using var pen = new Pen(Color.Black, 2);
            e.Graphics.DrawRectangle(pen, 1, 1, _cardPanel.Width - 3, _cardPanel.Height - 3);
        }

        private Button CreatePrimaryButton(string text)
        {
            var button = new Button
            {
                Text = text,
                AutoSize = true,
                Margin = new Padding(10, 0, 0, 0),
                Padding = new Padding(16, 6, 16, 6),
                BackColor = Hex("#073634"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            button.FlatAppearance.BorderSize = 2;
            button.FlatAppearance.BorderColor = Color.Black;
            return button;
        }

        private Button CreateSecondaryButton(string text)
        {
            var button = new Button
            {
                Text = text,
                AutoSize = true,
                Margin = new Padding(10, 0, 0, 0),
                Padding = new Padding(16, 6, 16, 6),
                BackColor = Hex("#EDEECE"),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            button.FlatAppearance.BorderSize = 2;
            button.FlatAppearance.BorderColor = Color.Black;
            return button;
        }


        private static Product Clone(Product product)
        {
            return new Product
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Category = product.Category,
                Description = product.Description,
                Storage = product.Storage,
                Colors = product.Colors,
                Sizes = product.Sizes,
                Rating = product.Rating,
                Images = product.Images,
                Features = product.Features,
                InStock = product.InStock,
                StockQuantity = product.StockQuantity,
                CreatedAt = product.CreatedAt
            };
        }
    }
}
