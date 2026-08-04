#nullable disable
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace EasyDent2
{
    /// <summary>
    /// صفحة حول البرنامج - About Form
    /// تعرض معلومات البرنامج والشركة المنتجة
    /// </summary>
    public class AboutForm : Form
    {
        private PictureBox logoPictureBox;
        private Label lblTitle;
        private Label lblVersion;
        private Label lblDescription;
        private Label lblContact;
        private Label lblCopyright;
        private LinkLabel linkContact;
        private Button btnClose;
        private Panel panelTop;
        private Panel panelContent;

        public AboutForm()
        {
            InitializeComponent();
            SetupUI();
            SetupTheme();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Icon = null;
            this.Text = "حول البرنامج";
            this.BackColor = Color.FromArgb(240, 244, 248);
        }

        private void SetupUI()
        {
            // الجزء العلوي (الشعار والعنوان)
            panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 200,
                BackColor = Color.FromArgb(102, 51, 153)
            };
            this.Controls.Add(panelTop);

            // الشعار
            logoPictureBox = new PictureBox
            {
                Location = new Point(150, 20),
                Size = new Size(200, 150),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = LogoRenderer.RenderFullLogo(200, 150),
                BackColor = Color.Transparent
            };
            panelTop.Controls.Add(logoPictureBox);

            // الجزء الرئيسي (المحتوى)
            panelContent = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(240, 244, 248),
                AutoScroll = true
            };
            this.Controls.Add(panelContent);

            // العنوان الرئيسي
            lblTitle = new Label
            {
                Text = "🦷 EasyDent 2 Professional",
                Location = new Point(20, 10),
                Size = new Size(460, 35),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.FromArgb(33, 33, 66),
                AutoSize = false
            };
            panelContent.Controls.Add(lblTitle);

            // رقم الإصدار
            lblVersion = new Label
            {
                Text = "الإصدار: 3.0.0.0",
                Location = new Point(20, 50),
                Size = new Size(460, 25),
                Font = new Font("Segoe UI", 11),
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.FromArgb(102, 51, 153),
                AutoSize = false
            };
            panelContent.Controls.Add(lblVersion);

            // الوصف
            lblDescription = new Label
            {
                Text = "نظام التقاط ومعالجة الأشعة الرقمية للأسنان\r\n\r\n" +
                       "برنامج احترافي متخصص في التقاط صور الأشعة الرقمية\r\n" +
                       "ومعالجتها باستخدام تقنيات متقدمة.\r\n\r\n" +
                       "يدعم:\r\n" +
                       "• أجهزة الأشعة الرقمية (TWAIN)\r\n" +
                       "• معالجة الصور بـ OpenCV\r\n" +
                       "• قياس الأبعاد والمسافات\r\n" +
                       "• المعايرة الدقيقة والقياس\r\n" +
                       "• الطباعة والحفظ",
                Location = new Point(20, 85),
                Size = new Size(460, 150),
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.TopRight,
                ForeColor = Color.FromArgb(52, 58, 64),
                AutoSize = false
            };
            panelContent.Controls.Add(lblDescription);

            // معلومات الاتصال
            lblContact = new Label
            {
                Text = "معلومات الاتصال:",
                Location = new Point(20, 240),
                Size = new Size(460, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.TopRight,
                ForeColor = Color.FromArgb(33, 33, 66),
                AutoSize = false
            };
            panelContent.Controls.Add(lblContact);

            // رابط الاتصال
            linkContact = new LinkLabel
            {
                Text = "📞 01099116300",
                Location = new Point(20, 265),
                Size = new Size(460, 25),
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.TopRight,
                LinkColor = Color.FromArgb(0, 102, 204),
                AutoSize = false
            };
            linkContact.LinkClicked += LinkContact_LinkClicked;
            panelContent.Controls.Add(linkContact);

            // حقوق الملكية
            lblCopyright = new Label
            {
                Text = "© 2024-2026 ROFA SOFT\r\nجميع الحقوق محفوظة",
                Location = new Point(20, 300),
                Size = new Size(460, 50),
                Font = new Font("Segoe UI", 9),
                TextAlign = ContentAlignment.TopCenter,
                ForeColor = Color.FromArgb(108, 117, 125),
                AutoSize = false
            };
            panelContent.Controls.Add(lblCopyright);

            // زر الإغلاق
            btnClose = new Button
            {
                Text = "إغلاق",
                Location = new Point(200, 360),
                Size = new Size(100, 35),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(102, 51, 153),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            panelContent.Controls.Add(btnClose);
        }

        private void SetupTheme()
        {
            // تطبيق الثيم الحالي
            var colors = ThemeManager.GetCurrentColors();
            this.BackColor = colors.Background;

            panelTop.BackColor = colors.Primary;
            lblTitle.ForeColor = colors.Dark;
            lblVersion.ForeColor = colors.Secondary;
            lblDescription.ForeColor = colors.Dark;
            lblContact.ForeColor = colors.Dark;
            lblCopyright.ForeColor = colors.Dark;

            btnClose.BackColor = colors.Primary;
            btnClose.ForeColor = Color.White;
        }

        private void LinkContact_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // فتح تطبيق الهاتف أو رسالة بريد
            try
            {
                Process.Start("tel:01099116300");
            }
            catch
            {
                MessageBox.Show("الرقم: 01099116300", "معلومات الاتصال", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                logoPictureBox?.Image?.Dispose();
                logoPictureBox?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
