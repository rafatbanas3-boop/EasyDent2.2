#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace EasyDent2
{
    /// <summary>
    /// تصميم محسّن للواجهة الرئيسية - نسخة احترافية متقدمة
    /// مستوحاة من تصميم DABI ATLANTE الاحترافي
    /// Powered by ROFA SOFT - Enhanced Design v3.1
    /// </summary>
    partial class Form1Enhanced
    {
        private System.ComponentModel.IContainer components = null;

        // ===== MAIN PANELS =====
        private Panel topHeaderPanel;
        private Panel leftToolbarPanel;
        private Panel mainContentPanel;
        private Panel rightControlPanel;
        private Panel bottomFooterPanel;
        private Panel leftImageListPanel;
        private Panel imageDisplayPanel;
        private Panel imageBorderPanel;
        private Panel panelHistogram;

        // ===== TOP SECTION =====
        private Label lblRofaLogo;
        private Label lblAppTitle;
        private TextBox txtPatientName;
        private TextBox txtPatientID;
        private ComboBox cmbToothNumber;
        private Label lblToothInfo;
        private Guna2Button btnSave;

        // ===== IMAGE DISPLAY WITH HISTOGRAM =====
        private PictureBox pictureBoxMain;
        private Label lblHistogramPlaceholder;

        // ===== TOOLBAR BUTTONS =====
        private Guna2Button[] toolbarButtons;
        private Label[] toolbarLabels;

        // ===== RIGHT CONTROL SECTION =====
        private GroupBox groupBoxAdjustments;
        private GroupBox groupBoxCalibration;
        private TrackBar trackBarBrightness;
        private TrackBar trackBarContrast;
        private Label lblBrightness;
        private Label lblContrast;
        private Label lblBrightnessValue;
        private Label lblContrastValue;
        private TextBox txtCalibrationValue;

        // ===== LEFT IMAGE LIST =====
        private ListBox listBoxImages;
        private Label lblImageHistory;

        // ===== FOOTER =====
        private Label lblStatus;
        private Label lblDateTime;
        private Panel footerLeftPanel;
        private Panel footerRightPanel;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            
            this.Size = new System.Drawing.Size(1700, 950);
            this.Text = "🦷 EasyDent 2 Professional v3.0 - نظام التقاط ومعالجة الأشعة الرقمية للأسنان";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 242, 245);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.RightToLeft = RightToLeft.Yes;
            this.AutoScaleMode = AutoScaleMode.Font;

            // ===== FOOTER (BOTTOM STATUS BAR) =====
            InitializeFooter();

            // ===== TOP HEADER PANEL =====
            InitializeTopHeader();

            // ===== LEFT TOOLBAR =====
            InitializeLeftToolbar();

            // ===== LEFT IMAGE LIST PANEL =====
            InitializeLeftImageList();

            // ===== MAIN CONTENT (Image Display + Histogram) =====
            InitializeMainContent();

            // ===== RIGHT CONTROL PANEL =====
            InitializeRightControls();
        }

        private void InitializeTopHeader()
        {
            topHeaderPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15),
                RightToLeft = RightToLeft.Yes
            };

            // ROFA Logo Section
            lblRofaLogo = new Label
            {
                Text = "🦷 ROFA SOFT",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                Location = new Point(15, 10),
                AutoSize = true,
                RightToLeft = RightToLeft.No
            };

            lblAppTitle = new Label
            {
                Text = "EasyDent 2 Professional v3.0",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 100, 100),
                Location = new Point(15, 38),
                AutoSize = true
            };

            // Patient Name
            Label lblPatientNameLabel = new Label
            {
                Text = "اسم المريض:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(500, 15),
                AutoSize = true
            };

            txtPatientName = new TextBox
            {
                Location = new Point(320, 15),
                Width = 170,
                Height = 28,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "أدخل اسم المريض"
            };

            // Patient ID
            Label lblPatientIDLabel = new Label
            {
                Text = "رقم المريض:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(500, 50),
                AutoSize = true
            };

            txtPatientID = new TextBox
            {
                Location = new Point(320, 50),
                Width = 170,
                Height = 28,
                Font = new Font("Segoe UI", 9),
                BorderStyle = BorderStyle.FixedSingle,
                Text = "أدخل الرقم"
            };

            // Tooth Number
            Label lblToothLabel = new Label
            {
                Text = "رقم السن:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(180, 15),
                AutoSize = true
            };

            cmbToothNumber = new ComboBox
            {
                Location = new Point(80, 15),
                Width = 90,
                Height = 28,
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            for (int i = 1; i <= 32; i++)
                cmbToothNumber.Items.Add(i.ToString());
            cmbToothNumber.SelectedIndex = 0;

            lblToothInfo = new Label
            {
                Text = "معلومات الأسنان",
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray,
                Location = new Point(80, 50),
                Width = 90,
                Height = 25
            };

            // Save Button
            btnSave = new Guna2Button
            {
                Text = "💾 حفظ",
                Size = new Size(90, 35),
                Location = new Point(1520, 25),
                FillColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BorderRadius = 5,
                Cursor = Cursors.Hand,
                RightToLeft = RightToLeft.No
            };

            topHeaderPanel.Controls.Add(lblRofaLogo);
            topHeaderPanel.Controls.Add(lblAppTitle);
            topHeaderPanel.Controls.Add(lblPatientNameLabel);
            topHeaderPanel.Controls.Add(txtPatientName);
            topHeaderPanel.Controls.Add(lblPatientIDLabel);
            topHeaderPanel.Controls.Add(txtPatientID);
            topHeaderPanel.Controls.Add(lblToothLabel);
            topHeaderPanel.Controls.Add(cmbToothNumber);
            topHeaderPanel.Controls.Add(lblToothInfo);
            topHeaderPanel.Controls.Add(btnSave);

            this.Controls.Add(topHeaderPanel);
        }

        private void InitializeLeftToolbar()
        {
            leftToolbarPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 130,
                BackColor = Color.FromArgb(240, 240, 243),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8),
                AutoScroll = true,
                RightToLeft = RightToLeft.Yes
            };

            // Toolbar items with icons and labels
            string[] toolNames = {
                "📂", "✨", "🔄", "⚪",
                "↶", "📥", "📷", "📏",
                "🖨️", "💾", "🔗", "⚙️"
            };

            string[] toolLabels = {
                "فتح", "تحسين", "عكس", "رمادي",
                "إعادة", "معايرة", "التقاط", "قياس",
                "طباعة", "حفظ", "توصيل", "إعدادات"
            };

            Color[] btnColors = {
                Color.FromArgb(23, 162, 184),
                Color.FromArgb(111, 66, 193),
                Color.FromArgb(200, 100, 100),
                Color.FromArgb(150, 150, 150),
                Color.FromArgb(255, 140, 0),
                Color.FromArgb(100, 150, 200),
                Color.FromArgb(100, 180, 180),
                Color.FromArgb(255, 193, 7),
                Color.FromArgb(150, 150, 150),
                Color.FromArgb(40, 167, 69),
                Color.FromArgb(220, 53, 69),
                Color.FromArgb(0, 123, 255)
            };

            toolbarButtons = new Guna2Button[toolNames.Length];
            toolbarLabels = new Label[toolNames.Length];

            int btnY = 5;
            int btnSize = 40;
            int btnSpacing = 62;

            for (int i = 0; i < toolNames.Length; i++)
            {
                int row = i / 2;
                int col = i % 2;
                int btnX = col * 56;
                int currentY = btnY + (row * btnSpacing);

                toolbarButtons[i] = new Guna2Button
                {
                    Text = toolNames[i],
                    Size = new Size(btnSize, btnSize),
                    Location = new Point(btnX, currentY),
                    FillColor = btnColors[i],
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 18, FontStyle.Bold),
                    BorderRadius = 8,
                    Cursor = Cursors.Hand,
                    ShadowDecoration = { Enabled = true }
                };

                toolbarLabels[i] = new Label
                {
                    Text = toolLabels[i],
                    Font = new Font("Segoe UI", 7, FontStyle.Bold),
                    Location = new Point(btnX, currentY + btnSize + 2),
                    Width = btnSize,
                    TextAlign = ContentAlignment.TopCenter,
                    AutoSize = false,
                    Height = 18
                };

                leftToolbarPanel.Controls.Add(toolbarButtons[i]);
                leftToolbarPanel.Controls.Add(toolbarLabels[i]);
            }

            this.Controls.Add(leftToolbarPanel);
        }

        private void InitializeLeftImageList()
        {
            leftImageListPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 140,
                BackColor = Color.FromArgb(250, 250, 252),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8),
                RightToLeft = RightToLeft.Yes
            };

            lblImageHistory = new Label
            {
                Text = "📸 سجل الصور",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                Location = new Point(8, 5),
                AutoSize = true
            };

            listBoxImages = new ListBox
            {
                Location = new Point(8, 30),
                Width = 124,
                Height = 650,
                Font = new Font("Segoe UI", 8),
                BorderStyle = BorderStyle.FixedSingle,
                RightToLeft = RightToLeft.Yes
            };

            leftImageListPanel.Controls.Add(lblImageHistory);
            leftImageListPanel.Controls.Add(listBoxImages);

            this.Controls.Add(leftImageListPanel);
        }

        private void InitializeMainContent()
        {
            mainContentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(20, 20, 25),
                Padding = new Padding(10)
            };

            // ===== HISTOGRAM PANEL =====
            panelHistogram = new Panel
            {
                Dock = DockStyle.Top,
                Height = 130,
                BackColor = Color.FromArgb(30, 30, 35),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 5)
            };

            lblHistogramPlaceholder = new Label
            {
                Dock = DockStyle.Fill,
                Text = "📊 رسم بياني - سيتم تحديثه عند تحميل صورة",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 200, 100),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(30, 30, 35)
            };

            panelHistogram.Controls.Add(lblHistogramPlaceholder);

            // ===== IMAGE BORDER PANEL =====
            imageBorderPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(0, 102, 204), // لون الإطار الأزرق
                Padding = new Padding(4) // سمك الإطار (4 بكسل)
            };

            // ===== IMAGE DISPLAY PANEL =====
            imageDisplayPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(15, 15, 20),
                Padding = new Padding(2)
            };

            pictureBoxMain = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(15, 15, 20),
                BorderStyle = BorderStyle.None
            };

            imageDisplayPanel.Controls.Add(pictureBoxMain);
            imageBorderPanel.Controls.Add(imageDisplayPanel);

            mainContentPanel.Controls.Add(imageBorderPanel);
            mainContentPanel.Controls.Add(panelHistogram);

            this.Controls.Add(mainContentPanel);
        }

        private void InitializeRightControls()
        {
            rightControlPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 220,
                BackColor = Color.FromArgb(240, 240, 243),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10),
                AutoScroll = true,
                RightToLeft = RightToLeft.Yes
            };

            // ===== CALIBRATION GROUP =====
            groupBoxCalibration = new GroupBox
            {
                Text = "⚙️ المعايرة والقياس",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 102, 204),
                Location = new Point(10, 10),
                Width = 200,
                Height = 100,
                Padding = new Padding(8)
            };

            Label lblPixelsPerMm = new Label
            {
                Text = "البكسل/مم:",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                Location = new Point(10, 25),
                AutoSize = true
            };

            txtCalibrationValue = new TextBox
            {
                Location = new Point(10, 45),
                Width = 180,
                Height = 28,
                Font = new Font("Segoe UI", 9),
                Text = "0.00",
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            groupBoxCalibration.Controls.Add(lblPixelsPerMm);
            groupBoxCalibration.Controls.Add(txtCalibrationValue);

            // ===== ADJUSTMENTS GROUP =====
            groupBoxAdjustments = new GroupBox
            {
                Text = "🎨 ضبط الصورة",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(102, 51, 153),
                Location = new Point(10, 115),
                Width = 200,
                Height = 280,
                Padding = new Padding(10)
            };

            // Brightness
            lblBrightness = new Label
            {
                Text = "السطوع",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(15, 25),
                AutoSize = true
            };

            lblBrightnessValue = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(170, 25),
                AutoSize = true,
                TextAlign = ContentAlignment.TopRight
            };

            trackBarBrightness = new TrackBar
            {
                Minimum = -100,
                Maximum = 100,
                Value = 0,
                Location = new Point(15, 50),
                Width = 170,
                Height = 40,
                TickStyle = TickStyle.TopLeft,
                TickFrequency = 10
            };

            // Contrast
            lblContrast = new Label
            {
                Text = "التباين",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(15, 100),
                AutoSize = true
            };

            lblContrastValue = new Label
            {
                Text = "0",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(170, 100),
                AutoSize = true,
                TextAlign = ContentAlignment.TopRight
            };

            trackBarContrast = new TrackBar
            {
                Minimum = -100,
                Maximum = 100,
                Value = 0,
                Location = new Point(15, 125),
                Width = 170,
                Height = 40,
                TickStyle = TickStyle.TopLeft,
                TickFrequency = 10
            };

            groupBoxAdjustments.Controls.Add(lblBrightness);
            groupBoxAdjustments.Controls.Add(lblBrightnessValue);
            groupBoxAdjustments.Controls.Add(trackBarBrightness);
            groupBoxAdjustments.Controls.Add(lblContrast);
            groupBoxAdjustments.Controls.Add(lblContrastValue);
            groupBoxAdjustments.Controls.Add(trackBarContrast);

            rightControlPanel.Controls.Add(groupBoxCalibration);
            rightControlPanel.Controls.Add(groupBoxAdjustments);

            this.Controls.Add(rightControlPanel);
        }

        private void InitializeFooter()
        {
            bottomFooterPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(45, 45, 48),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };

            footerLeftPanel = new Panel
            {
                Location = new Point(10, 10),
                Width = 800,
                Height = 40,
                BackColor = Color.Transparent
            };

            lblStatus = new Label
            {
                Text = "🟢 نظام الأشعة جاهز ومستقر...",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 255, 100),
                Location = new Point(0, 8),
                AutoSize = true
            };

            footerLeftPanel.Controls.Add(lblStatus);

            footerRightPanel = new Panel
            {
                Location = new Point(1400, 10),
                Width = 280,
                Height = 40,
                BackColor = Color.Transparent
            };

            lblDateTime = new Label
            {
                Text = "📅 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightGray,
                Location = new Point(0, 8),
                AutoSize = true
            };

            footerRightPanel.Controls.Add(lblDateTime);

            bottomFooterPanel.Controls.Add(footerLeftPanel);
            bottomFooterPanel.Controls.Add(footerRightPanel);

            this.Controls.Add(bottomFooterPanel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
