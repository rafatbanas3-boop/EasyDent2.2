#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace EasyDent2
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private Panel topActionPanel;
        private Panel leftToolPanel;
        private Panel mainViewPanel;

        private GroupBox groupPatient;
        private GroupBox groupCapture;

        private TextBox txtPatientName;
        private ComboBox cmbToothNumber;
        private Guna2Button btnSavePatient;
        private Guna2Button btnTwainConnect;
        private Guna2Button btnTwain;
        private Guna2Button btnSettings;

        // Left toolbar icons (small buttons in grid)
        private Guna2Button btnOpen;
        private Guna2Button btnTestOpenCv;
        private Guna2Button btnInvert;
        private Guna2Button btnGrayscale;
        private Guna2Button btnReset;
        private Guna2Button btnImportCalibration;
        private Guna2Button btnAcquireSimulated;
        private Guna2Button btnMeasure;
        private Guna2Button btnPrint;
        private Guna2Button btnZoomIn;
        private Guna2Button btnZoomOut;

        private ListBox listBoxImages;
        private Panel panelImage;
        private PictureBox pictureBox1;
        private TrackBar trackBarContrast;
        private TrackBar trackBarGain;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabelContact;

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            this.Size = new System.Drawing.Size(1400, 800);
            this.Text = "EasyDent 2.0 - نظام التقاط ومعالجة الأشعة الرقمية للأسنان";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // ===== STATUS STRIP (Footer) =====
            statusStrip1 = new StatusStrip { BackColor = Color.FromArgb(45, 45, 48) };
            toolStripStatusLabel1 = new ToolStripStatusLabel { Text = "نظام الأشعة جاهز ومستقر...", Font = new Font("Segoe UI", 10), ForeColor = Color.White };
            toolStripStatusLabelContact = new ToolStripStatusLabel { Text = "☎ 01099116300 | rafatabozida3@gmail.com", Font = new Font("Segoe UI", 9), ForeColor = Color.LightGray };
            statusStrip1.Items.Add(toolStripStatusLabel1);
            statusStrip1.Items.Add(new ToolStripSeparator());
            statusStrip1.Items.Add(toolStripStatusLabelContact);
            statusStrip1.Dock = DockStyle.Bottom;
            this.Controls.Add(statusStrip1);

            // ===== TOP ACTION PANEL =====
            topActionPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            this.Controls.Add(topActionPanel);

            // Patient info group
            groupPatient = new GroupBox
            {
                Text = "بيانات المريض والسن",
                Location = new Point(10, 5),
                Size = new Size(500, 65),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50)
            };

            Label lblName = new Label { Text = "اسم المريض:", Location = new Point(10, 22), AutoSize = true, Font = new Font("Segoe UI", 9) };
            txtPatientName = new TextBox { Location = new Point(90, 20), Width = 180, Font = new Font("Segoe UI", 9), BorderStyle = BorderStyle.FixedSingle };

            Label lblTooth = new Label { Text = "السن:", Location = new Point(290, 22), AutoSize = true, Font = new Font("Segoe UI", 9) };
            cmbToothNumber = new ComboBox { Location = new Point(320, 20), Width = 60, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 9) };
            cmbToothNumber.Items.AddRange(new object[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31", "32" });
            cmbToothNumber.SelectedIndex = 0;

            btnSavePatient = MakeIconButton("💾", new Size(70, 35), Color.FromArgb(40, 167, 69), Color.White);
            btnSavePatient.Location = new Point(400, 18);

            groupPatient.Controls.Add(lblName);
            groupPatient.Controls.Add(txtPatientName);
            groupPatient.Controls.Add(lblTooth);
            groupPatient.Controls.Add(cmbToothNumber);
            groupPatient.Controls.Add(btnSavePatient);
            topActionPanel.Controls.Add(groupPatient);

            // Capture group (TWAIN controls on the right)
            groupCapture = new GroupBox
            {
                Text = "التحكم بالمستشعر",
                Location = new Point(530, 5),
                Size = new Size(380, 65),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50)
            };

            btnTwainConnect = MakeIconButton("🔗", new Size(90, 35), Color.FromArgb(220, 53, 69), Color.White);
            btnTwainConnect.Location = new Point(10, 20);
            btnTwainConnect.Text = "توصيل 🔗";

            btnTwain = MakeIconButton("⚙️", new Size(90, 35), Color.FromArgb(108, 117, 125), Color.White);
            btnTwain.Location = new Point(110, 20);
            btnTwain.Text = "إعدادات";

            btnSettings = MakeIconButton("🔧", new Size(90, 35), Color.FromArgb(0, 123, 255), Color.White);
            btnSettings.Location = new Point(210, 20);
            btnSettings.Text = "نظام";

            groupCapture.Controls.Add(btnTwainConnect);
            groupCapture.Controls.Add(btnTwain);
            groupCapture.Controls.Add(btnSettings);
            topActionPanel.Controls.Add(groupCapture);

            // ===== LEFT TOOLBAR PANEL (Vertical icon buttons grid) =====
            leftToolPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 90,
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5),
                AutoScroll = true
            };

            int btnY = 5;
            int btnSize = 40;
            int btnSpacing = 50;

            // Row 1: مفتوح، تحسين
            btnOpen = MakeIconButton("📂", new Size(btnSize, btnSize), Color.FromArgb(23, 162, 184), Color.White);
            btnOpen.Location = new Point(5, btnY);
            leftToolPanel.Controls.Add(btnOpen);

            btnTestOpenCv = MakeIconButton("✨", new Size(btnSize, btnSize), Color.FromArgb(111, 66, 193), Color.White);
            btnTestOpenCv.Location = new Point(50, btnY);
            leftToolPanel.Controls.Add(btnTestOpenCv);

            btnY += btnSpacing;

            // Row 2: عكس الألوان، تدرج رمادي
            btnInvert = MakeIconButton("🔄", new Size(btnSize, btnSize), Color.FromArgb(200, 200, 200), Color.Black);
            btnInvert.Location = new Point(5, btnY);
            leftToolPanel.Controls.Add(btnInvert);

            btnGrayscale = MakeIconButton("⚪", new Size(btnSize, btnSize), Color.FromArgb(200, 200, 200), Color.Black);
            btnGrayscale.Location = new Point(50, btnY);
            leftToolPanel.Controls.Add(btnGrayscale);

            btnY += btnSpacing;

            // Row 3: إعادة ضبط
            btnReset = MakeIconButton("↶", new Size(btnSize, btnSize), Color.FromArgb(255, 140, 0), Color.White);
            btnReset.Location = new Point(5, btnY);
            leftToolPanel.Controls.Add(btnReset);

            btnY += btnSpacing;

            // Row 4: تحميل معايرة، التقاط
            btnImportCalibration = MakeIconButton("📥", new Size(btnSize, btnSize), Color.FromArgb(100, 150, 200), Color.White);
            btnImportCalibration.Location = new Point(5, btnY);
            leftToolPanel.Controls.Add(btnImportCalibration);

            btnAcquireSimulated = MakeIconButton("📷", new Size(btnSize, btnSize), Color.FromArgb(100, 180, 180), Color.White);
            btnAcquireSimulated.Location = new Point(50, btnY);
            leftToolPanel.Controls.Add(btnAcquireSimulated);

            btnY += btnSpacing;

            // Row 5: قياس، طباعة
            btnMeasure = MakeIconButton("📏", new Size(btnSize, btnSize), Color.FromArgb(255, 193, 7), Color.Black);
            btnMeasure.Location = new Point(5, btnY);
            leftToolPanel.Controls.Add(btnMeasure);

            btnPrint = MakeIconButton("🖨️", new Size(btnSize, btnSize), Color.FromArgb(150, 150, 150), Color.Black);
            btnPrint.Location = new Point(50, btnY);
            leftToolPanel.Controls.Add(btnPrint);

            this.Controls.Add(leftToolPanel);

            // ===== MAIN VIEW PANEL (Image + Controls) =====
            mainViewPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
            this.Controls.Add(mainViewPanel);

            // Image display panel (center - large)
            panelImage = new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Black,
                Padding = new Padding(5)
            };

            pictureBox1 = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(25, 25, 25)
            };
            panelImage.Controls.Add(pictureBox1);
            mainViewPanel.Controls.Add(panelImage);

            // ===== RIGHT CONTROL PANEL (Zoom + Trackbars) =====
            Panel rightControlPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 100,
                BackColor = Color.FromArgb(245, 245, 245),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8)
            };

            // Zoom buttons (stacked)
            btnZoomIn = MakeIconButton("🔍+", new Size(80, 32), Color.FromArgb(100, 150, 200), Color.White);
            btnZoomIn.Location = new Point(8, 8);
            btnZoomIn.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            btnZoomOut = MakeIconButton("🔍−", new Size(80, 32), Color.FromArgb(100, 150, 200), Color.White);
            btnZoomOut.Location = new Point(8, 48);
            btnZoomOut.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Trackbars (vertical)
            Label lblGain = new Label { Text = "السطوع", Location = new Point(8, 90), AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) };
            trackBarGain = new TrackBar { Minimum = 0, Maximum = 100, Value = 50, Location = new Point(8, 110), Size = new Size(80, 40), TickStyle = TickStyle.None, Orientation = Orientation.Vertical };

            Label lblContrast = new Label { Text = "التباين", Location = new Point(8, 160), AutoSize = true, Font = new Font("Segoe UI", 8, FontStyle.Bold) };
            trackBarContrast = new TrackBar { Minimum = 0, Maximum = 100, Value = 50, Location = new Point(8, 180), Size = new Size(80, 40), TickStyle = TickStyle.None, Orientation = Orientation.Vertical };

            rightControlPanel.Controls.Add(btnZoomIn);
            rightControlPanel.Controls.Add(btnZoomOut);
            rightControlPanel.Controls.Add(lblGain);
            rightControlPanel.Controls.Add(trackBarGain);
            rightControlPanel.Controls.Add(lblContrast);
            rightControlPanel.Controls.Add(trackBarContrast);
            mainViewPanel.Controls.Add(rightControlPanel);

            // ===== LEFT SIDE IMAGE LIST =====
            Panel listPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 120,
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5)
            };

            Label lblHistory = new Label { Text = "الصور", Location = new Point(5, 5), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            listBoxImages = new ListBox { Location = new Point(5, 25), Size = new Size(110, 600), Font = new Font("Segoe UI", 9), BorderStyle = BorderStyle.FixedSingle };

            listPanel.Controls.Add(lblHistory);
            listPanel.Controls.Add(listBoxImages);
            mainViewPanel.Controls.Add(listPanel);

            // ===== EVENT HANDLERS =====
            this.btnOpen.Click += new System.EventHandler(this.BtnOpen_Click);
            this.btnTestOpenCv.Click += new System.EventHandler(this.BtnTestOpenCv_Click);
            this.btnImportCalibration.Click += new System.EventHandler(this.BtnImportCalibration_Click);
            this.btnAcquireSimulated.Click += new System.EventHandler(this.BtnAcquireSimulated_Click);
            this.btnInvert.Click += new System.EventHandler(this.BtnInvert_Click);
        }

        // Helper: Create small icon button
        private Guna2Button MakeIconButton(string text, Size size, Color fillColor, Color textColor)
        {
            Guna2Button btn = new Guna2Button
            {
                Text = text,
                Size = size,
                FillColor = fillColor,
                ForeColor = textColor,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                BorderRadius = 6,
                ShadowDecoration = { Enabled = false }
            };
            return btn;
        }
    }
}