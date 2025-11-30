using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AWEfinal.BLL.Services;
using AWEfinal.DAL.Models;

namespace AWEfinal.AdminWinForms
{
    public partial class DashboardForm : Form
    {
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        private User? _currentUser;

        private TabControl _tabs = null!;
        private readonly List<Button> _navButtons = new();

        // Overview controls
        private Label _totalProductsLabel = null!;
        private Label _totalOrdersLabel = null!;
        private Label _totalRevenueLabel = null!;
        private Label _avgOrderValueLabel = null!;

        // Product tab controls
        private DataGridView _productsGrid = null!;
        private TextBox _productSearchInput = null!;
        private BindingList<ProductRow> _productBinding = new();
        private List<Product> _allProducts = new();

        // Order tab controls
        private DataGridView _ordersGrid = null!;
        private TextBox _orderSearchInput = null!;
        private ComboBox _orderFilterCombo = null!;
        private ComboBox _orderStatusUpdateCombo = null!;
        private TextBox _trackingInput = null!;
        private BindingList<OrderRow> _orderBinding = new();
        private List<Order> _allOrders = new();
        private Panel _orderDetailsContent = null!;
        private TableLayoutPanel _orderDetailsTable = null!;
        private Label _orderDetailsPlaceholder = null!;
        private Label _orderInvoiceLabel = null!;
        private Label _orderCustomerLabel = null!;
        private Label _orderContactLabel = null!;
        private Label _orderAddressLabel = null!;
        private ListView _orderItemsList = null!;
        private Label _orderTotalLabel = null!;
        private Label _orderTrackingLabel = null!;
        private Button _printDeliveryButton = null!;
        private Button _printReceiptButton = null!;
        private Button _orderUpdateButton = null!;
        private Label _ordersSummaryTotalLabel = null!;
        private Label _ordersSummaryPendingLabel = null!;
        private Label _ordersSummaryTransitLabel = null!;
        private Label _ordersSummaryDeliveredLabel = null!;
        private Order? _selectedOrder;

        // Analytics controls
        private ComboBox _analyticsRangeCombo = null!;
        private Label _analyticsRevenueLabel = null!;
        private Label _analyticsOrdersLabel = null!;
        private Label _analyticsProductsLabel = null!;
        private Label _analyticsAvgLabel = null!;
        private Panel _revenueChartPanel = null!;
        private Panel _ordersChartPanel = null!;
        private ListView _topProductsList = null!;
        private List<(string Label, decimal Value)> _revenuePoints = new();
        private List<(string Label, int Orders, int Products)> _orderPoints = new();

        public DashboardForm(IProductService productService, IOrderService orderService)
        {
            _productService = productService;
            _orderService = orderService;
            InitializeComponent();
        }

        public void SetLoggedInUser(User user)
        {
            _currentUser = user;
            Text = $"AWE Electronics - Admin Dashboard ({user.Name})";
        }

        private static Color Hex(string value) => ColorTranslator.FromHtml(value);

        private void BuildLayout()
        {
            Text = "AWE Electronics - Admin";
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1400, 950);
            Size = new Size(1400, 1000);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Hex("#EDEECE");
            Font = new Font("Segoe UI", 10f, FontStyle.Regular);

            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                RowStyles =
                {
                    new RowStyle(SizeType.AutoSize),
                    new RowStyle(SizeType.AutoSize),
                    new RowStyle(SizeType.Percent, 100)
                }
            };

            mainLayout.Controls.Add(CreateHeaderPanel(), 0, 0);
            mainLayout.Controls.Add(CreateNavPanel(), 0, 1);

            _tabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Appearance = TabAppearance.FlatButtons,
                ItemSize = new Size(0, 1),
                SizeMode = TabSizeMode.Fixed
            };
            _tabs.SelectedIndexChanged += (_, _) => UpdateNavState(_tabs.SelectedIndex);

            _tabs.TabPages.Add(CreateDashboardTab());
            _tabs.TabPages.Add(CreateProductsTab());
            _tabs.TabPages.Add(CreateOrdersTab());
            _tabs.TabPages.Add(CreateAnalyticsTab());

            mainLayout.Controls.Add(_tabs, 0, 2);
            Controls.Add(mainLayout);

            Shown += async (_, _) => await RefreshAllAsync();
            UpdateNavState(0);
        }

        private Panel CreateHeaderPanel()
        {
            var panel = new Panel
            {
                BackColor = Hex("#073634"),
                Dock = DockStyle.Top,
                Height = 90,
                Padding = new Padding(24)
            };

            var title = new Label
            {
                Text = "AWE Admin Dashboard",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 22f, FontStyle.Bold),
                Dock = DockStyle.Left,
                AutoSize = true
            };

            var subtitle = new Label
            {
                Text = "Product • Order • Analytics",
                ForeColor = Color.WhiteSmoke,
                Font = new Font("Segoe UI", 12f, FontStyle.Regular),
                Dock = DockStyle.Left,
                AutoSize = true,
                Margin = new Padding(20, 32, 0, 0)
            };

            var logoutButton = new Button
            {
                Text = "Logout",
                Dock = DockStyle.Right,
                Width = 130,
                Height = 40,
                BackColor = Hex("#EDEECE"),
                FlatStyle = FlatStyle.Flat
            };
            logoutButton.FlatAppearance.BorderSize = 2;
            logoutButton.FlatAppearance.BorderColor = Color.Black;
            logoutButton.Click += (_, _) => Close();

            panel.Controls.Add(logoutButton);
            panel.Controls.Add(subtitle);
            panel.Controls.Add(title);
            return panel;
        }

        private Panel CreateNavPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                Padding = new Padding(24, 12, 24, 0),
                BackColor = Color.White
            };

            var flow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                WrapContents = false
            };

            _navButtons.Clear();
            _navButtons.Add(CreateNavButton("Dashboard Overview", 0));
            _navButtons.Add(CreateNavButton("Product Management", 1));
            _navButtons.Add(CreateNavButton("Order Management", 2));
            _navButtons.Add(CreateNavButton("Analytics", 3));

            foreach (var btn in _navButtons)
            {
                flow.Controls.Add(btn);
            }

            panel.Controls.Add(flow);
            panel.Paint += (_, e) =>
            {
                using var pen = new Pen(Color.Black, 2);
                e.Graphics.DrawLine(pen, 0, panel.Height - 1, panel.Width, panel.Height - 1);
            };
            return panel;
        }

        private Button CreateNavButton(string text, int tabIndex)
        {
            var button = new Button
            {
                Text = text,
                Tag = tabIndex,
                AutoSize = true,
                Margin = new Padding(0, 0, 12, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Hex("#EDEECE"),
                ForeColor = Color.Black,
                Padding = new Padding(16, 8, 16, 8)
            };
            button.FlatAppearance.BorderSize = 2;
            button.FlatAppearance.BorderColor = Color.Black;
            button.Click += (_, _) => SwitchTab(tabIndex);
            return button;
        }

        private void SwitchTab(int index)
        {
            if (_tabs.SelectedIndex != index)
                _tabs.SelectedIndex = index;

            UpdateNavState(index);
        }

        private void UpdateNavState(int activeIndex)
        {
            for (int i = 0; i < _navButtons.Count; i++)
            {
                var btn = _navButtons[i];
                var isActive = i == activeIndex;
                btn.BackColor = isActive ? Hex("#073634") : Hex("#EDEECE");
                btn.ForeColor = isActive ? Color.White : Color.Black;
            }
        }

        private TabPage CreateDashboardTab()
        {
            var page = new TabPage("Overview")
            {
                BackColor = ColorTranslator.FromHtml("#EDEECE")
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 300,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(20, 10, 20, 10)
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 300));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            layout.Controls.Add(CreateMetricCard("Total Revenue", "$0.00", out _totalRevenueLabel), 0, 0);
            layout.Controls.Add(CreateMetricCard("Total Orders", "0", out _totalOrdersLabel), 1, 0);
            layout.Controls.Add(CreateMetricCard("Products Sold", "0", out _totalProductsLabel), 2, 0);
            layout.Controls.Add(CreateMetricCard("Avg Order Value", "$0.00", out _avgOrderValueLabel), 3, 0);

            page.Controls.Add(layout);
            return page;
        }

        private Control CreateMetricCard(string title, string initialValue, out Label valueLabel)
        {
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Margin = new Padding(6),
                Padding = new Padding(8, 8, 8, 8), // padding inside the card
                BackColor = Color.White
            };

            var titleLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                AutoSize = true,                           // let it choose its own height
                Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                Padding = new Padding(0, 2, 0, 2),         // some top/bottom padding
                TextAlign = ContentAlignment.MiddleLeft
            };

            valueLabel = new Label
            {
                Text = initialValue,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                Padding = new Padding(0, 6, 0, 0),
                TextAlign = ContentAlignment.TopLeft
            };

            panel.Controls.Add(valueLabel);
            panel.Controls.Add(titleLabel);

            return panel;
        }


        private TabPage CreateProductsTab()
        {
            var page = new TabPage("Products");
            page.BackColor = Hex("#EDEECE");

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1,
                RowStyles =
                {
                    new RowStyle(SizeType.AutoSize),
                    new RowStyle(SizeType.Percent, 100)
                }
            };

            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(10),
                BackColor = Color.White
            };
            toolbar.Paint += (_, e) =>
            {
                using var pen = new Pen(Color.Black, 2);
                e.Graphics.DrawRectangle(pen, 1, 1, toolbar.Width - 3, toolbar.Height - 3);
            };

            _productSearchInput = new TextBox { Width = 360, PlaceholderText = "Search by name or category" };
            _productSearchInput.TextChanged += (_, _) => ApplyProductFilter();

            var refreshButton = CreateSecondaryButton("Refresh");
            refreshButton.Click += async (_, _) => await LoadProductsAsync();

            var addButton = CreatePrimaryButton("Add Product");
            addButton.Click += async (_, _) => await AddProductAsync();

            var editButton = CreateSecondaryButton("Edit");
            editButton.Click += async (_, _) => await EditProductAsync();

            var deleteButton = CreateSecondaryButton("Delete");
            deleteButton.Click += async (_, _) => await DeleteProductAsync();

            toolbar.Controls.Add(new Label { Text = "Search:", AutoSize = true, Padding = new Padding(0, 6, 6, 0) });
            toolbar.Controls.Add(_productSearchInput);
            toolbar.Controls.Add(refreshButton);
            toolbar.Controls.Add(addButton);
            toolbar.Controls.Add(editButton);
            toolbar.Controls.Add(deleteButton);

            _productsGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };
            StyleGrid(_productsGrid);

            _productsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "Id", Width = 70 });
            _productsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Name", DataPropertyName = "Name", Width = 260 });
            _productsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Category", DataPropertyName = "Category", Width = 160 });
            _productsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Price", DataPropertyName = "Price", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            _productsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Stock", DataPropertyName = "Stock", Width = 100 });
            _productsGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 120 });

            var card = CreateCardContainer();
            card.Controls.Add(_productsGrid);

            layout.Controls.Add(toolbar, 0, 0);
            layout.Controls.Add(card, 0, 1);

            page.Controls.Add(layout);
            return page;
        }

        private TabPage CreateOrdersTab()
        {
            var page = new TabPage("Orders");
            page.BackColor = Hex("#EDEECE");

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                RowStyles =
                {
                    new RowStyle(SizeType.AutoSize),
                    new RowStyle(SizeType.Percent, 100),
                    new RowStyle(SizeType.AutoSize)
                }
            };

            var filterPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                Padding = new Padding(10),
                BackColor = Color.White
            };
            filterPanel.Paint += (_, e) =>
            {
                using var pen = new Pen(Color.Black, 2);
                e.Graphics.DrawRectangle(pen, 1, 1, filterPanel.Width - 3, filterPanel.Height - 3);
            };

            _orderSearchInput = new TextBox { Width = 320, PlaceholderText = "Search by order # or customer" };
            _orderSearchInput.TextChanged += (_, _) => ApplyOrderFilter();

            _orderFilterCombo = new ComboBox
            {
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _orderFilterCombo.Items.AddRange(new object[] { "All", "pending", "paid", "packaging", "shipped", "delivered", "cancelled" });
            _orderFilterCombo.SelectedIndex = 0;
            _orderFilterCombo.SelectedIndexChanged += (_, _) => ApplyOrderFilter();

            var refreshButton = CreateSecondaryButton("Refresh");
            refreshButton.Click += async (_, _) => await LoadOrdersAsync();

            filterPanel.Controls.Add(new Label { Text = "Search:", AutoSize = true, Padding = new Padding(0, 6, 6, 0) });
            filterPanel.Controls.Add(_orderSearchInput);
            filterPanel.Controls.Add(new Label { Text = "Status:", AutoSize = true, Padding = new Padding(16, 6, 6, 0) });
            filterPanel.Controls.Add(_orderFilterCombo);
            filterPanel.Controls.Add(refreshButton);

            _ordersGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };
            StyleGrid(_ordersGrid);
            _ordersGrid.SelectionChanged += (_, _) => ShowSelectedOrderDetails();

            _ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ID", DataPropertyName = "Id", Width = 70 });
            _ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Order #", DataPropertyName = "OrderNumber", Width = 180 });
            _ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Customer", DataPropertyName = "Customer", Width = 260 });
            _ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Total", DataPropertyName = "Total", Width = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } });
            _ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 130 });
            _ordersGrid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Updated", DataPropertyName = "UpdatedAt", Width = 200 });

            var orderCard = CreateCardContainer();
            orderCard.Controls.Add(_ordersGrid);

            var contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
            contentLayout.Controls.Add(orderCard, 0, 0);
            contentLayout.Controls.Add(CreateOrderDetailsCard(), 1, 0);

            layout.Controls.Add(filterPanel, 0, 0);
            layout.Controls.Add(contentLayout, 0, 1);
            layout.Controls.Add(CreateOrdersSummaryPanel(), 0, 2);

            page.Controls.Add(layout);
            return page;
        }

        private Control CreateOrderDetailsCard()
        {
            var card = CreateCardContainer();
            card.Padding = new Padding(28);
            card.MinimumSize = new Size(640, 0);
            card.Width = 660;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var title = new Label
            {
                Text = "Order Details",
                Dock = DockStyle.Top,
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Padding = new Padding(0, 0, 0, 8),
                Margin = new Padding(0, 0, 0, 4)
            };
            layout.Controls.Add(title, 0, 0);

            var host = new Panel { Dock = DockStyle.Fill };

            _orderDetailsPlaceholder = new Label
            {
                Text = "Select an order to view details",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.DimGray
            };

            _orderDetailsContent = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Visible = false,
                Padding = new Padding(0, 0, 0, 20) // Add bottom padding for scrolling
            };
            _orderDetailsContent.Resize += (_, _) => ResizeOrderDetailsLayout();

            _orderDetailsTable = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowOnly, // Prevent shrinking
                ColumnCount = 1,
                Padding = new Padding(0),
                Margin = new Padding(0),
                RowCount = 14 // Explicitly set row count to match added controls
            };
            _orderDetailsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            // Explicitly set all rows to AutoSize to ensure proper vertical stacking and full visibility
            for (int i = 0; i < 14; i++)
            {
                _orderDetailsTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            }

            // Add consistent vertical spacing between rows by setting minimum row height if needed, but rely on margins
            _orderDetailsTable.Controls.Add(CreateOrderDetailBlock("Invoice Number", out _orderInvoiceLabel), 0, 0);
            _orderDetailsTable.Controls.Add(CreateOrderDetailBlock("Customer", out _orderCustomerLabel), 0, 1);
            _orderDetailsTable.Controls.Add(CreateOrderDetailBlock("Contact", out _orderContactLabel), 0, 2);
            _orderDetailsTable.Controls.Add(CreateOrderDetailBlock("Shipping Address", out _orderAddressLabel, wrap: true), 0, 3);

            var itemsHeader = new Label
            {
                Text = "Items",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Padding = new Padding(0, 8, 0, 4),
                Margin = new Padding(0, 12, 0, 0), // Added top margin for spacing from previous block
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right // Stretch horizontally
            };
            _orderDetailsTable.Controls.Add(itemsHeader, 0, 4);

            _orderItemsList = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                Height = 200,
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0, 0, 0, 10),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right // Stretch horizontally
            };
            _orderItemsList.Columns.Add("Product", 260);
            _orderItemsList.Columns.Add("Qty", 80, HorizontalAlignment.Center);
            _orderItemsList.Columns.Add("Subtotal", 150);
            _orderDetailsTable.Controls.Add(_orderItemsList, 0, 5);

            _orderDetailsTable.Controls.Add(CreateOrderDetailBlock("Order Total", out _orderTotalLabel), 0, 6);
            _orderDetailsTable.Controls.Add(CreateOrderDetailBlock("Tracking Number", out _orderTrackingLabel), 0, 7);

            var statusLabel = new Label
            {
                Text = "Update Status",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 16, 0, 8), // Increased bottom margin to prevent overlap with combo
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };
            _orderDetailsTable.Controls.Add(statusLabel, 0, 8);

            _orderStatusUpdateCombo = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false,
                Margin = new Padding(0, 0, 0, 12), // Increased bottom margin for spacing from tracking input
                Padding = new Padding(4), // Internal padding for better text spacing
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right // Stretch horizontally
            };
            _orderStatusUpdateCombo.Items.AddRange(new object[] { "pending", "paid", "packaging", "shipped", "delivered", "cancelled" });
            _orderDetailsTable.Controls.Add(_orderStatusUpdateCombo, 0, 9);

            _trackingInput = new TextBox
            {
                PlaceholderText = "Tracking # (optional)",
                Enabled = false,
                Margin = new Padding(0, 0, 0, 16), // Increased bottom margin for spacing from update button
                Padding = new Padding(4), // Internal padding for better text spacing
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right // Stretch horizontally
            };
            _orderDetailsTable.Controls.Add(_trackingInput, 0, 10);

            _orderUpdateButton = CreatePrimaryButton("Update Order");
            _orderUpdateButton.Enabled = false;
            _orderUpdateButton.Margin = new Padding(0, 0, 0, 16);
            _orderUpdateButton.Click += async (_, _) => await UpdateOrderAsync();
            _orderDetailsTable.Controls.Add(_orderUpdateButton, 0, 11);

            var printLabel = new Label
            {
                Text = "Print Documents",
                Font = new Font(Font, FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 16, 0, 8), // Increased bottom margin to prevent overlap with buttons
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right
            };
            _orderDetailsTable.Controls.Add(printLabel, 0, 12);

            var printPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0, 0, 0, 20), // Bottom margin for overall spacing
                Padding = new Padding(0, 0, 0, 0),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right // Stretch horizontally
            };

            _printDeliveryButton = CreatePrimaryButton("Print Delivery Note");
            _printDeliveryButton.Enabled = false;
            _printDeliveryButton.Margin = new Padding(0, 0, 16, 0);
            _printDeliveryButton.Click += (_, _) => PrintOrderDocument(OrderDocumentType.DeliveryNote);

            _printReceiptButton = CreateSecondaryButton("Print Customer Receipt");
            _printReceiptButton.Enabled = false;
            _printReceiptButton.Margin = new Padding(0);
            _printReceiptButton.Click += (_, _) => PrintOrderDocument(OrderDocumentType.Receipt);

            printPanel.Controls.Add(_printDeliveryButton);
            printPanel.Controls.Add(_printReceiptButton);
            _orderDetailsTable.Controls.Add(printPanel, 0, 13);

            _orderDetailsContent.Controls.Add(_orderDetailsTable);
            host.Controls.Add(_orderDetailsContent);
            host.Controls.Add(_orderDetailsPlaceholder);
            _orderDetailsPlaceholder.BringToFront();

            layout.Controls.Add(host, 0, 1);
            card.Controls.Add(layout);

            // Initial layout force to compute preferred height
            _orderDetailsTable.PerformLayout();
            var initialWidth = _orderDetailsTable.Width > 0 ? _orderDetailsTable.Width : 660;
            var preferredHeight = _orderDetailsTable.GetPreferredSize(new Size(initialWidth, int.MaxValue)).Height;
            _orderDetailsTable.Height = preferredHeight;

            host.PerformLayout();

            return card;
        }

        private void ResizeOrderDetailsLayout()
        {
            if (_orderDetailsContent == null || _orderItemsList == null)
                return;

            _orderDetailsContent.SuspendLayout(); // Suspend to avoid flicker during resize

            var availableWidth = _orderDetailsContent.ClientSize.Width - 20;
            var targetWidth = Math.Max(availableWidth, 500);

            if (_orderDetailsTable != null)
            {
                _orderDetailsTable.Width = targetWidth;
                _orderDetailsTable.PerformLayout();

                // Recompute preferred height after width change (for wrapping)
                var currentWidth = targetWidth;
                var preferredHeight = _orderDetailsTable.GetPreferredSize(new Size(currentWidth, int.MaxValue)).Height;
                _orderDetailsTable.Height = preferredHeight;
            }

            _orderItemsList.Width = targetWidth;

            if (_orderStatusUpdateCombo != null)
                _orderStatusUpdateCombo.Width = targetWidth;

            if (_trackingInput != null)
                _trackingInput.Width = targetWidth;

            if (_orderDetailsTable != null)
            {
                foreach (Control control in _orderDetailsTable.Controls)
                {
                    if (control is FlowLayoutPanel panel && panel.Controls.Count >= 2) // Assuming FlowLayoutPanel from CreateOrderDetailBlock
                    {
                        panel.Width = targetWidth;
                        foreach (Control inner in panel.Controls)
                        {
                            if (inner is Label label && label.MaximumSize.Width > 0)
                            {
                                label.MaximumSize = new Size(targetWidth - 20, 0);
                            }
                        }
                    }
                }
            }

            _orderDetailsContent.ResumeLayout();
            _orderDetailsContent.PerformLayout();
        }

        private Control CreateOrderDetailBlock(string labelText, out Label valueLabel, bool wrap = false)
        {
            var panel = new FlowLayoutPanel
            {
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(0, 4, 0, 8), // Top/bottom padding for vertical spacing between blocks
                Margin = new Padding(0, 0, 0, 0),
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right // Stretch horizontally in TLP cell
            };

            var label = new Label
            {
                Text = labelText + ":",
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                Margin = new Padding(0, 0, 12, 0), // Right margin to separate label from value
                Padding = new Padding(0),
                Anchor = AnchorStyles.Left | AnchorStyles.Top // No right anchor to allow value to stretch
            };

            valueLabel = new Label
            {
                AutoSize = wrap ? false : true,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0),
                Padding = new Padding(4), // Internal padding for text breathing room
                MaximumSize = new Size(int.MaxValue, 0), // Allow multi-line expansion for wrapped text
                Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right // Stretch to fill remaining space
            };

            panel.Controls.Add(label);
            panel.Controls.Add(valueLabel);

            return panel;
        }
        private Control CreateOrdersSummaryPanel()
        {
            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Padding = new Padding(10, 10, 10, 0),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));

            layout.Controls.Add(CreateOrderSummaryCard("Total Orders", out _ordersSummaryTotalLabel), 0, 0);
            layout.Controls.Add(CreateOrderSummaryCard("Pending", out _ordersSummaryPendingLabel), 1, 0);
            layout.Controls.Add(CreateOrderSummaryCard("In Transit", out _ordersSummaryTransitLabel), 2, 0);
            layout.Controls.Add(CreateOrderSummaryCard("Delivered", out _ordersSummaryDeliveredLabel), 3, 0);

            return layout;
        }

        private Control CreateOrderSummaryCard(string title, out Label valueLabel)
        {
            var panel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(6),
                Padding = new Padding(8),
                BackColor = Color.White,
                Width = 260,
                Height = 130
            };

            var caption = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                AutoSize = true,
                Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                Padding = new Padding(0, 2, 0, 2)
            };

            valueLabel = new Label
            {
                Text = "0",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 20f, FontStyle.Bold),
                TextAlign = ContentAlignment.TopLeft,
                Padding = new Padding(0, 6, 0, 0)
            };

            panel.Controls.Add(valueLabel);
            panel.Controls.Add(caption);
            return panel;
        }

        private void ShowSelectedOrderDetails(Order? explicitOrder = null)
        {
            var order = explicitOrder;
            if (order == null && _ordersGrid?.CurrentRow?.DataBoundItem is OrderRow row)
            {
                order = _allOrders.FirstOrDefault(o => o.Id == row.Id);
            }

            if (order == null)
            {
                _selectedOrder = null;
                ToggleOrderDetailsState(false);
                return;
            }

            _selectedOrder = order;
            PopulateOrderDetails(order);
        }

        private void PopulateOrderDetails(Order order)
        {
            if (_orderDetailsContent == null)
                return;

            ToggleOrderDetailsState(true);

            _orderInvoiceLabel.Text = ResolveOrderNumber(order);
            _orderCustomerLabel.Text = string.IsNullOrWhiteSpace(order.ShippingFullName)
                ? order.User?.Name ?? "Unknown"
                : order.ShippingFullName;
            _orderContactLabel.Text = string.IsNullOrWhiteSpace(order.ShippingPhone) ? "N/A" : order.ShippingPhone;
            _orderAddressLabel.Text = $"{order.ShippingAddress}\n{order.ShippingCity}, {order.ShippingPostalCode}\n{order.ShippingCountry}";

            var items = order.OrderItems?.ToList() ?? new List<OrderItem>();
            _orderItemsList.BeginUpdate();
            _orderItemsList.Items.Clear();
            if (items.Any())
            {
                foreach (var item in items)
                {
                    var entry = new ListViewItem(item.ProductName ?? "Item");
                    entry.SubItems.Add(item.Quantity.ToString());
                    entry.SubItems.Add(item.Subtotal.ToString("C", CultureInfo.CurrentCulture));
                    _orderItemsList.Items.Add(entry);
                }
            }
            else
            {
                var placeholder = new ListViewItem("No line items recorded")
                {
                    ForeColor = Color.DimGray
                };
                placeholder.SubItems.Add(string.Empty);
                placeholder.SubItems.Add(string.Empty);
                _orderItemsList.Items.Add(placeholder);
            }
            _orderItemsList.EndUpdate();

            _orderTotalLabel.Text = order.Total.ToString("C", CultureInfo.CurrentCulture);
            _orderTrackingLabel.Text = string.IsNullOrWhiteSpace(order.TrackingNumber) ? "Not assigned" : order.TrackingNumber;

            if (_orderStatusUpdateCombo.Items.Contains(order.Status))
            {
                _orderStatusUpdateCombo.SelectedItem = order.Status;
            }
            else
            {
                _orderStatusUpdateCombo.SelectedIndex = -1;
            }

            _trackingInput.Text = order.TrackingNumber ?? string.Empty;
        }

        private void ToggleOrderDetailsState(bool hasSelection)
        {
            if (_orderDetailsContent == null || _orderDetailsPlaceholder == null)
                return;

            _orderDetailsContent.Visible = hasSelection;
            _orderDetailsPlaceholder.Visible = !hasSelection;

            if (hasSelection)
                ResizeOrderDetailsLayout();

            if (_orderStatusUpdateCombo != null)
                _orderStatusUpdateCombo.Enabled = hasSelection;
            if (_trackingInput != null)
                _trackingInput.Enabled = hasSelection;
            if (_orderUpdateButton != null)
                _orderUpdateButton.Enabled = hasSelection;
            if (_printDeliveryButton != null)
                _printDeliveryButton.Enabled = hasSelection;
            if (_printReceiptButton != null)
                _printReceiptButton.Enabled = hasSelection;
        }

        private bool SelectOrderRowById(int id)
        {
            if (_ordersGrid == null)
                return false;

            foreach (DataGridViewRow row in _ordersGrid.Rows)
            {
                if (row.DataBoundItem is OrderRow orderRow && orderRow.Id == id)
                {
                    row.Selected = true;
                    _ordersGrid.CurrentCell = row.Cells[0];
                    return true;
                }
            }

            return false;
        }

        private void UpdateOrdersSummaryCards()
        {
            if (_ordersSummaryTotalLabel == null)
                return;

            var orders = _allOrders ?? new List<Order>();
            _ordersSummaryTotalLabel.Text = orders.Count.ToString();
            _ordersSummaryPendingLabel.Text = orders.Count(o =>
                string.Equals(o.Status, "pending", StringComparison.OrdinalIgnoreCase)
                || string.Equals(o.Status, "paid", StringComparison.OrdinalIgnoreCase)).ToString();
            _ordersSummaryTransitLabel.Text = orders.Count(o =>
                string.Equals(o.Status, "packaging", StringComparison.OrdinalIgnoreCase)
                || string.Equals(o.Status, "shipped", StringComparison.OrdinalIgnoreCase)).ToString();
            _ordersSummaryDeliveredLabel.Text = orders.Count(o =>
                string.Equals(o.Status, "delivered", StringComparison.OrdinalIgnoreCase)).ToString();
        }

        private void PrintOrderDocument(OrderDocumentType documentType)
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Select an order to print.", "Orders", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var order = _selectedOrder;
                var document = new PrintDocument
                {
                    DocumentName = documentType == OrderDocumentType.DeliveryNote
                        ? $"DeliveryNote_{order.Id}"
                        : $"Receipt_{order.Id}"
                };
                document.PrintPage += (_, e) => RenderOrderDocument(e, order, documentType);

                using var preview = new PrintPreviewDialog
                {
                    Document = document,
                    Width = 1000,
                    Height = 800
                };
                preview.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unable to render the document: {ex.Message}", "Print", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RenderOrderDocument(PrintPageEventArgs e, Order order, OrderDocumentType documentType)
        {
            using var headerFont = new Font("Segoe UI", 16f, FontStyle.Bold);
            using var subHeaderFont = new Font("Segoe UI", 12f, FontStyle.Regular);
            using var bodyFont = new Font("Segoe UI", 10f, FontStyle.Regular);
            using var bodyBoldFont = new Font("Segoe UI", 10f, FontStyle.Bold);

            var g = e.Graphics ?? throw new InvalidOperationException("Unable to access printer graphics surface.");
            var bounds = e.MarginBounds;
            float y = bounds.Top;

            g.DrawString("AWE Electronics", headerFont, Brushes.Black, bounds.Left, y);
            y += headerFont.GetHeight(g) + 5;
            var title = documentType == OrderDocumentType.DeliveryNote ? "Delivery Note" : "Customer Receipt";
            g.DrawString(title, subHeaderFont, Brushes.Black, bounds.Left, y);
            y += subHeaderFont.GetHeight(g) + 10;

            g.DrawString($"Invoice: {ResolveOrderNumber(order)}", bodyFont, Brushes.Black, bounds.Left, y);
            y += bodyFont.GetHeight(g) + 2;
            g.DrawString($"Date: {order.CreatedAt.ToLocalTime():MMM dd, yyyy HH:mm}", bodyFont, Brushes.Black, bounds.Left, y);
            y += bodyFont.GetHeight(g) + 2;
            if (!string.IsNullOrWhiteSpace(order.TrackingNumber))
            {
                g.DrawString($"Tracking #: {order.TrackingNumber}", bodyFont, Brushes.Black, bounds.Left, y);
                y += bodyFont.GetHeight(g) + 6;
            }
            else
            {
                y += 6;
            }

            g.DrawString("Ship To:", bodyBoldFont, Brushes.Black, bounds.Left, y);
            y += bodyBoldFont.GetHeight(g) + 2;
            var address = $"{order.ShippingFullName}\n{order.ShippingAddress}\n{order.ShippingCity}, {order.ShippingPostalCode}\n{order.ShippingCountry}";
            var addressSize = g.MeasureString(address, bodyFont, bounds.Width);
            g.DrawString(address, bodyFont, Brushes.Black, new RectangleF(bounds.Left, y, bounds.Width, addressSize.Height));
            y += addressSize.Height + 10;

            g.DrawString("Items", bodyBoldFont, Brushes.Black, bounds.Left, y);
            y += bodyBoldFont.GetHeight(g) + 4;

            var colItem = bounds.Left;
            var colQty = bounds.Left + bounds.Width * 0.6f;
            var colPrice = bounds.Left + bounds.Width * 0.8f;

            g.DrawString("Product", bodyBoldFont, Brushes.Black, colItem, y);
            g.DrawString("Qty", bodyBoldFont, Brushes.Black, colQty, y);
            g.DrawString("Subtotal", bodyBoldFont, Brushes.Black, colPrice, y);
            y += bodyBoldFont.GetHeight(g) + 4;

            var items = order.OrderItems?.ToList() ?? new List<OrderItem>();
            if (items.Any())
            {
                foreach (var item in items)
                {
                    g.DrawString(item.ProductName ?? "Item", bodyFont, Brushes.Black, colItem, y);
                    g.DrawString(item.Quantity.ToString(), bodyFont, Brushes.Black, colQty, y);
                    g.DrawString(item.Subtotal.ToString("C", CultureInfo.CurrentCulture), bodyFont, Brushes.Black, colPrice, y);
                    y += bodyFont.GetHeight(g) + 2;
                }
            }
            else
            {
                g.DrawString("No line items recorded.", bodyFont, Brushes.Black, colItem, y);
                y += bodyFont.GetHeight(g) + 4;
            }

            y += 6;
            g.DrawLine(Pens.Black, bounds.Left, y, bounds.Right, y);
            y += 8;

            var totalText = $"Total: {order.Total.ToString("C", CultureInfo.CurrentCulture)}";
            var totalSize = g.MeasureString(totalText, bodyBoldFont);
            g.DrawString(totalText, bodyBoldFont, Brushes.Black, bounds.Right - totalSize.Width, y);
            y += totalSize.Height + 10;

            if (documentType == OrderDocumentType.Receipt)
            {
                g.DrawString("Thank you for shopping with AWE Electronics.", bodyFont, Brushes.Black, bounds.Left, y);
            }
            else
            {
                g.DrawString("Please include this delivery note inside the shipment.", bodyFont, Brushes.Black, bounds.Left, y);
            }

            e.HasMorePages = false;
        }

        private TabPage CreateAnalyticsTab()
        {
            var page = new TabPage("Analytics");
            page.BackColor = Hex("#EDEECE");   // or ColorTranslator.FromHtml("#EDEECE");

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 3,
                ColumnCount = 1,
                Padding = new Padding(30, 24, 30, 24),
                AutoSize = false
            };

            // 3 rows: summary (auto), charts (fill), footer (auto)
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // --------- SUMMARY (CARDS + TIMEFRAME) ---------

            var summaryPanel = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 1,
                Dock = DockStyle.Fill,
                AutoSize = false,
                Margin = new Padding(0, 0, 16, 12),
                Padding = new Padding(0, 0, 16, 0)
            };

            summaryPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            for (int i = 0; i < 3; i++)
                summaryPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            _analyticsRangeCombo = new ComboBox
            {
                Width = 260,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _analyticsRangeCombo.Items.AddRange(new object[]
            {
                "Last 7 Days",
                "Last 30 Days",
                "Quarter to Date",
                "Year to Date",
                "All Time"
            });
            _analyticsRangeCombo.SelectedIndex = 1;
            _analyticsRangeCombo.SelectedIndexChanged += (_, _) => UpdateAnalyticsUI();

            // 3 overview-style metric cards
            summaryPanel.Controls.Add(
                CreateMetricCard("Total Revenue", "$0.00", out _analyticsRevenueLabel), 0, 0);
            summaryPanel.Controls.Add(
                CreateMetricCard("Total Orders", "0", out _analyticsOrdersLabel), 1, 0);
            summaryPanel.Controls.Add(
                CreateMetricCard("Products Sold", "0", out _analyticsProductsLabel), 2, 0);

            var rangePanel = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 2,
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 10, 0, 0)
            };
            rangePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            rangePanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            var timeframeRow = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };
            timeframeRow.Controls.Add(new Label
            {
                Text = "Timeframe:",
                AutoSize = true,
                Padding = new Padding(0, 6, 6, 0)
            });
            timeframeRow.Controls.Add(_analyticsRangeCombo);

            var avgRow = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 12, 0, 0)
            };
            avgRow.Controls.Add(new Label
            {
                Text = "Avg Order Value:",
                AutoSize = true,
                Padding = new Padding(0, 6, 6, 0),
                Font = new Font("Segoe UI", 10f, FontStyle.Regular)
            });
            _analyticsAvgLabel = new Label
            {
                Text = "$0.00",
                AutoSize = true,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                Padding = new Padding(0, 4, 0, 0)
            };
            avgRow.Controls.Add(_analyticsAvgLabel);

            rangePanel.Controls.Add(timeframeRow, 0, 0);
            rangePanel.Controls.Add(avgRow, 0, 1);

            var summaryPanelHost = new Panel
            {
                Dock = DockStyle.Top,
                Height = 260,
                MinimumSize = new Size(0, 260),
                Padding = new Padding(0)
            };
            summaryPanelHost.Controls.Add(summaryPanel);

            var summaryContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 1,
                RowCount = 2,
                Margin = new Padding(0, 0, 0, 20),
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            summaryContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            summaryContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            summaryContainer.Controls.Add(summaryPanelHost, 0, 0);
            summaryContainer.Controls.Add(rangePanel, 0, 1);

            // --------- CHARTS + TOP PRODUCTS ---------

            var chartContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                Margin = new Padding(0),
                AutoSize = false
            };

            chartContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55f));
            chartContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45f));
            chartContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            chartContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

            _revenueChartPanel = CreateChartPanel(DrawRevenueChart);
            _ordersChartPanel = CreateChartPanel(DrawOrdersChart);

            var revenueCard = WrapChartWithCard("Revenue Trend", _revenueChartPanel);
            var ordersCard = WrapChartWithCard("Orders & Products", _ordersChartPanel);

            _topProductsList = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                BorderStyle = BorderStyle.None
            };
            _topProductsList.Columns.Add("Product", 240);
            _topProductsList.Columns.Add("Quantity", 100);
            _topProductsList.Columns.Add("Revenue", 140);

            var topProductsCard = CreateCardContainer();
            topProductsCard.Padding = new Padding(24);

            var topProductsHeader = new Label
            {
                Text = "Top Selling Products",
                Dock = DockStyle.Top,
                Font = new Font(Font, FontStyle.Bold),
                Height = 34,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 0, 0, 4)
            };

            topProductsCard.Controls.Add(_topProductsList);
            topProductsCard.Controls.Add(topProductsHeader);

            chartContainer.Controls.Add(revenueCard, 0, 0);
            chartContainer.Controls.Add(topProductsCard, 1, 0);
            chartContainer.Controls.Add(ordersCard, 0, 1);
            chartContainer.SetColumnSpan(ordersCard, 2);

            // --------- FOOTER ---------

            var footerLabel = new Label
            {
                Text = "Analytics refresh automatically with the selected timeframe.",
                AutoSize = true,
                ForeColor = Color.DimGray
            };

            // add to main layout
            layout.Controls.Add(summaryContainer, 0, 0);
            layout.Controls.Add(chartContainer, 0, 1);
            layout.Controls.Add(footerLabel, 0, 2);

            page.Controls.Add(layout);
            return page;
        }

        private async Task RefreshAllAsync()
        {
            await LoadProductsAsync();
            await LoadOrdersAsync();
            UpdateOverviewCards();
        }

        private async Task LoadProductsAsync()
        {
            var products = (await _productService.GetAllProductsAsync()).OrderBy(p => p.Name).ToList();
            _allProducts = products;
            ApplyProductFilter();
            UpdateOverviewCards();
        }

        private void ApplyProductFilter()
        {
            if (_allProducts == null)
                return;

            var query = _allProducts.AsEnumerable();
            var filter = _productSearchInput?.Text.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(p => p.Name.Contains(filter, StringComparison.OrdinalIgnoreCase)
                    || p.Category.Contains(filter, StringComparison.OrdinalIgnoreCase));
            }

            var rows = query.Select(p => new ProductRow
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Price = p.Price,
                Stock = p.StockQuantity,
                Status = p.InStock ? "In stock" : "Out of stock"
            }).ToList();

            _productBinding = new BindingList<ProductRow>(rows);
            _productsGrid.DataSource = _productBinding;
        }

        private async Task AddProductAsync()
        {
            using var editor = new ProductEditorForm();
            if (editor.ShowDialog(this) == DialogResult.OK && editor.Product != null)
            {
                await _productService.CreateProductAsync(editor.Product);
                await LoadProductsAsync();
            }
        }

        private async Task EditProductAsync()
        {
            var product = GetSelectedProduct();
            if (product == null)
            {
                MessageBox.Show("Select a product to edit.", "Edit Product", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var editor = new ProductEditorForm(product);
            if (editor.ShowDialog(this) == DialogResult.OK && editor.Product != null)
            {
                await _productService.UpdateProductAsync(editor.Product);
                await LoadProductsAsync();
            }
        }

        private async Task DeleteProductAsync()
        {
            var product = GetSelectedProduct();
            if (product == null)
            {
                MessageBox.Show("Select a product to delete.", "Delete Product", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var confirm = MessageBox.Show($"Delete {product.Name}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                await _productService.DeleteProductAsync(product.Id);
                await LoadProductsAsync();
            }
        }

        private Product? GetSelectedProduct()
        {
            if (_productsGrid.CurrentRow?.DataBoundItem is ProductRow row)
            {
                return _allProducts.FirstOrDefault(p => p.Id == row.Id);
            }

            return null;
        }

        private async Task LoadOrdersAsync()
        {
            var orders = (await _orderService.GetAllOrdersAsync()).OrderByDescending(o => o.CreatedAt).ToList();
            _allOrders = orders;
            ApplyOrderFilter();
            UpdateOrdersSummaryCards();
            UpdateOverviewCards();
            UpdateAnalyticsUI();
        }

        private void ApplyOrderFilter()
        {
            if (_allOrders == null)
                return;

            var query = _allOrders.AsEnumerable();
            var search = _orderSearchInput?.Text.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o =>
                {
                    var number = ResolveOrderNumber(o);
                    var customer = o.ShippingFullName ?? string.Empty;
                    return number.Contains(search, StringComparison.OrdinalIgnoreCase)
                        || customer.Contains(search, StringComparison.OrdinalIgnoreCase);
                });
            }

            var status = _orderFilterCombo?.SelectedItem?.ToString();
            if (!string.IsNullOrWhiteSpace(status) && status != "All")
            {
                query = query.Where(o => string.Equals(o.Status, status, StringComparison.OrdinalIgnoreCase));
            }

            var rows = query.Select(o => new OrderRow
            {
                Id = o.Id,
                OrderNumber = ResolveOrderNumber(o),
                Customer = string.IsNullOrWhiteSpace(o.ShippingFullName)
                    ? o.User?.Name ?? "Unknown"
                    : o.ShippingFullName,
                Total = o.Total,
                Status = o.Status,
                UpdatedAt = (o.UpdatedAt == default ? o.CreatedAt : o.UpdatedAt).ToLocalTime()
                    .ToString("g", CultureInfo.CurrentCulture)
            }).ToList();

            _orderBinding = new BindingList<OrderRow>(rows);
            _ordersGrid.DataSource = _orderBinding;

            var previousId = _selectedOrder?.Id;
            var hasSelection = false;
            if (previousId.HasValue)
            {
                hasSelection = SelectOrderRowById(previousId.Value);
            }

            if (!hasSelection && _ordersGrid.Rows.Count > 0)
            {
                var firstRow = _ordersGrid.Rows[0];
                firstRow.Selected = true;
                _ordersGrid.CurrentCell = firstRow.Cells[0];
                hasSelection = true;
            }

            if (!hasSelection)
            {
                ShowSelectedOrderDetails(null);
            }
        }

        private async Task UpdateOrderAsync()
        {
            if (_selectedOrder == null)
            {
                MessageBox.Show("Select an order to update.", "Orders", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var status = _orderStatusUpdateCombo.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(status))
            {
                MessageBox.Show("Select a new status.", "Orders", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var tracking = _trackingInput.Text.Trim();
            var selectedId = _selectedOrder.Id;
            await _orderService.UpdateOrderStatusAsync(selectedId, status, string.IsNullOrWhiteSpace(tracking) ? null : tracking);
            await LoadOrdersAsync();
            SelectOrderRowById(selectedId);
        }

        private void UpdateOverviewCards()
        {
            var orders = _allOrders ?? new List<Order>();
            _totalOrdersLabel.Text = orders.Count.ToString();

            var revenueOrders = FilterRevenueOrders(orders).ToList();
            var revenue = revenueOrders.Sum(o => o.Total);
            _totalRevenueLabel.Text = revenue.ToString("C", CultureInfo.CurrentCulture);

            var average = revenueOrders.Any() ? revenue / revenueOrders.Count : 0m;
            _avgOrderValueLabel.Text = average.ToString("C", CultureInfo.CurrentCulture);
            _totalProductsLabel.Text = CalculateProductsSold(revenueOrders).ToString();
        }

        private void UpdateAnalyticsUI()
        {
            if (_allOrders == null || _analyticsRangeCombo == null)
                return;

            var range = _analyticsRangeCombo.SelectedItem?.ToString() ?? "Last 30 Days";
            var now = DateTime.UtcNow;
            DateTime? start = range switch
            {
                "Last 7 Days" => now.AddDays(-7),
                "Last 30 Days" => now.AddDays(-30),
                "Quarter to Date" => new DateTime(now.Year, ((now.Month - 1) / 3) * 3 + 1, 1),
                "Year to Date" => new DateTime(now.Year, 1, 1),
                _ => null
            };

            var filtered = FilterRevenueOrders(_allOrders);
            if (start.HasValue)
                filtered = filtered.Where(o => o.CreatedAt >= start.Value);

            var list = filtered.ToList();
            var revenue = list.Sum(o => o.Total);
            var orderCount = list.Count;
            var productsSold = CalculateProductsSold(list);
            var avgOrder = orderCount > 0 ? revenue / orderCount : 0m;

            _analyticsRevenueLabel.Text = revenue.ToString("C", CultureInfo.CurrentCulture);
            _analyticsOrdersLabel.Text = orderCount.ToString();
            _analyticsProductsLabel.Text = productsSold.ToString();
            _analyticsAvgLabel.Text = avgOrder.ToString("C", CultureInfo.CurrentCulture);

            var grouped = list
                .OrderBy(o => o.CreatedAt)
                .GroupBy(o => o.CreatedAt.ToLocalTime().Date)
                .Select(g => new
                {
                    Date = g.Key.ToString("MMM dd"),
                    Revenue = g.Sum(o => o.Total),
                    Orders = g.Count(),
                    Products = g.Sum(o => o.OrderItems?.Sum(oi => oi.Quantity) ?? 0)
                })
                .ToList();

            _revenuePoints = grouped.Select(g => (g.Date, g.Revenue)).ToList();
            _orderPoints = grouped.Select(g => (g.Date, g.Orders, g.Products)).ToList();
            _revenueChartPanel?.Invalidate();
            _ordersChartPanel?.Invalidate();

            var topProducts = list
                .SelectMany(o => o.OrderItems ?? new List<OrderItem>())
                .GroupBy(i => i.Product?.Name ?? "Unknown")
                .Select(g => new
                {
                    Product = g.Key,
                    Quantity = g.Sum(i => i.Quantity),
                    Revenue = g.Sum(i => i.Subtotal)
                })
                .OrderByDescending(g => g.Quantity)
                .ThenByDescending(g => g.Revenue)
                .Take(5)
                .ToList();

            _topProductsList.Items.Clear();
            if (topProducts.Any())
            {
                foreach (var tp in topProducts)
                {
                    var item = new ListViewItem(tp.Product);
                    item.SubItems.Add(tp.Quantity.ToString());
                    item.SubItems.Add(tp.Revenue.ToString("C", CultureInfo.CurrentCulture));
                    _topProductsList.Items.Add(item);
                }
            }
            else
            {
                _topProductsList.Items.Add(new ListViewItem("No sales data") { ForeColor = Color.DimGray });
            }
        }

        private Panel CreateChartPanel(PaintEventHandler painter)
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
                MinimumSize = new Size(600, 220)
            };
            panel.Paint += painter;
            panel.Resize += (_, _) => panel.Invalidate();
            return panel;
        }

        private void DrawRevenueChart(object? sender, PaintEventArgs e)
        {
            var panel = (Panel)sender!;
            var g = e.Graphics;
            g.Clear(Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (!_revenuePoints.Any())
            {
                TextRenderer.DrawText(g, "No revenue data", panel.Font, panel.ClientRectangle, Color.Gray, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                return;
            }

            var rect = new Rectangle(40, 10, panel.Width - 60, panel.Height - 50);
            rect.Width = Math.Max(rect.Width, 10);
            rect.Height = Math.Max(rect.Height, 10);

            var max = (float)_revenuePoints.Max(p => p.Value);
            if (max <= 0) max = 1;
            var step = rect.Width / Math.Max(1, _revenuePoints.Count - 1f);

            using var borderPen = new Pen(Color.Gainsboro, 1);
            g.DrawRectangle(borderPen, rect);

            var points = _revenuePoints.Select((point, index) =>
            {
                var x = rect.Left + step * index;
                var y = rect.Bottom - ((float)point.Value / max) * rect.Height;
                return new PointF(x, y);
            }).ToArray();

            using var linePen = new Pen(Hex("#073634"), 3);
            if (points.Length == 1)
            {
                g.DrawEllipse(linePen, points[0].X - 3, points[0].Y - 3, 6, 6);
            }
            else
            {
                g.DrawLines(linePen, points);
            }

            for (int i = 0; i < points.Length; i++)
            {
                var label = _revenuePoints[i].Label;
                TextRenderer.DrawText(g, label, panel.Font, new Point((int)points[i].X - 20, rect.Bottom + 4), Color.DimGray);
            }
        }

        private void DrawOrdersChart(object? sender, PaintEventArgs e)
        {
            var panel = (Panel)sender!;
            var g = e.Graphics;
            g.Clear(Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (!_orderPoints.Any())
            {
                TextRenderer.DrawText(g, "No order data", panel.Font, panel.ClientRectangle, Color.Gray, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                return;
            }

            var rect = new Rectangle(40, 10, panel.Width - 60, panel.Height - 50);
            rect.Width = Math.Max(rect.Width, 10);
            rect.Height = Math.Max(rect.Height, 10);

            var max = (float)_orderPoints.Max(p => Math.Max(p.Orders, p.Products));
            if (max <= 0) max = 1;

            using var borderPen = new Pen(Color.Gainsboro, 1);
            g.DrawRectangle(borderPen, rect);

            var groupWidth = rect.Width / Math.Max(1, _orderPoints.Count);
            var barWidth = Math.Max(10, (int)(groupWidth / 2.5));

            for (int i = 0; i < _orderPoints.Count; i++)
            {
                var pt = _orderPoints[i];
                var baseX = rect.Left + i * groupWidth;

                var ordersHeight = (pt.Orders / max) * rect.Height;
                var productsHeight = (pt.Products / max) * rect.Height;

                var ordersRect = new RectangleF(baseX, rect.Bottom - ordersHeight, barWidth, ordersHeight);
                var productsRect = new RectangleF(baseX + barWidth + 4, rect.Bottom - productsHeight, barWidth, productsHeight);

                using var ordersBrush = new SolidBrush(Hex("#0A7E8C"));
                using var productsBrush = new SolidBrush(Hex("#8C6A0A"));
                g.FillRectangle(ordersBrush, ordersRect);
                g.FillRectangle(productsBrush, productsRect);

                TextRenderer.DrawText(g, pt.Label, panel.Font, new Point((int)baseX, rect.Bottom + 4), Color.DimGray);
            }

            DrawLegendItem(g, new Rectangle(rect.Right - 120, rect.Top + 10, 14, 14), Hex("#0A7E8C"), "Orders");
            DrawLegendItem(g, new Rectangle(rect.Right - 120, rect.Top + 30, 14, 14), Hex("#8C6A0A"), "Products");
        }

        private void DrawLegendItem(Graphics g, Rectangle rect, Color color, string text)
        {
            using var brush = new SolidBrush(color);
            g.FillRectangle(brush, rect);
            TextRenderer.DrawText(g, text, Font, new Point(rect.Right + 6, rect.Top - 2), Color.DimGray);
        }

        private Control WrapChartWithCard(string title, Control chartControl)
        {
            var card = CreateCardContainer();
            card.Padding = new Padding(24);
            card.MinimumSize = new Size(640, 240);
            card.Margin = new Padding(0, 0, 0, 16);

            var header = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Font = new Font(Font, FontStyle.Bold),
                Height = 34,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0, 0, 0, 4)
            };

            chartControl.Dock = DockStyle.Fill;
            chartControl.MinimumSize = new Size(600, 200);

            card.Controls.Add(chartControl);
            card.Controls.Add(header);
            return card;
        }

        private Panel CreateCardContainer()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(10)
            };
            panel.Paint += (_, e) =>
            {
                using var pen = new Pen(Color.Black, 2);
                e.Graphics.DrawRectangle(pen, 1, 1, panel.Width - 3, panel.Height - 3);
            };
            return panel;
        }

        private Button CreatePrimaryButton(string text)
        {
            var button = new Button
            {
                Text = text,
                AutoSize = true,
                Margin = new Padding(6, 0, 0, 0),
                Padding = new Padding(14, 6, 14, 6),
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
                Margin = new Padding(6, 0, 0, 0),
                Padding = new Padding(14, 6, 14, 6),
                BackColor = Hex("#EDEECE"),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            button.FlatAppearance.BorderSize = 2;
            button.FlatAppearance.BorderColor = Color.Black;
            return button;
        }

        private void StyleGrid(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Hex("#073634");
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10f, FontStyle.Bold);
            grid.DefaultCellStyle.SelectionBackColor = Hex("#e0d9b0");
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.BorderStyle = BorderStyle.None;
        }

        private static IEnumerable<Order> FilterRevenueOrders(IEnumerable<Order>? orders) =>
            orders?.Where(IsRevenueOrder) ?? Enumerable.Empty<Order>();

        private static bool IsRevenueOrder(Order? order) =>
            order != null && !string.Equals(order.Status, "cancelled", StringComparison.OrdinalIgnoreCase);

        private static int CalculateProductsSold(IEnumerable<Order>? orders) =>
            orders?.Sum(o => o.OrderItems?.Sum(oi => oi.Quantity) ?? 0) ?? 0;

        private static string ResolveOrderNumber(Order order) =>
            order.InvoiceNumber ?? order.OrderNumber ?? $"ORD-{order.Id:0000}";

        private enum OrderDocumentType
        {
            DeliveryNote,
            Receipt
        }

        private class ProductRow
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public string Status { get; set; } = string.Empty;
        }

        private class OrderRow
        {
            public int Id { get; set; }
            public string OrderNumber { get; set; } = string.Empty;
            public string Customer { get; set; } = string.Empty;
            public decimal Total { get; set; }
            public string Status { get; set; } = string.Empty;
            public string UpdatedAt { get; set; } = string.Empty;
        }

        private class ReportRow
        {
            public string Period { get; set; } = string.Empty;
            public int OrderCount { get; set; }
            public decimal Revenue { get; set; }
        }
    }
}
