#nullable disable
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using FormsTimer = System.Windows.Forms.Timer;

namespace EasyDent2
{
    /// <summary>
    /// شاشة البداية (Splash Screen) لنظام EasyDent 2 Professional
    /// تظهر عند تشغيل البرنامج
    /// </summary>
    public class SplashForm : Form
    {
        private PictureBox logoPictureBox;
        private Label lblTitle;
        private Label lblVersion;
        private Label lblPoweredBy;
        private Label lblLoading;
        private ProgressBar progressBar;
        private FormsTimer loadingTimer;

        public SplashForm()
        {
            InitializeComponent();
            SetupUI();
            CenterToScreen();
        }

        private void InitializeComponent()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(600, 500);
            this.BackColor = Color.FromArgb(240, 244, 248);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.ShowInTaskbar = false;
        }

        private void SetupUI()
        {
            // الشعار
            logoPictureBox = new PictureBox
            {
                Location = new Point(150, 50),
                Size = new Size(300, 200),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = LogoRenderer.RenderFullLogo(300, 200)
            };
            this.Controls.Add(logoPictureBox);

            // عنوان البرنامج
            lblTitle = new Label
            {
                Text = "🦷 EasyDent 2 Professional v3.0",
                Location = new Point(30, 270),
                Size = new Size(540, 40),
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(33, 33, 66)
            };
            this.Controls.Add(lblTitle);

            // رقم الإصدار
            lblVersion = new Label
            {
                Text = "Dental Imaging Solutions",
                Location = new Point(30, 310),
                Size = new Size(540, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(102, 51, 153)
            };
            this.Controls.Add(lblVersion);

            // جهات الإنتاج
            lblPoweredBy = new Label
            {
                Text = "Powered by ROFA SOFT",
                Location = new Point(30, 340),
                Size = new Size(540, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(0, 102, 204)
            };
            this.Controls.Add(lblPoweredBy);

            // شريط التحميل
            progressBar = new ProgressBar
            {
                Location = new Point(50, 390),
                Size = new Size(500, 20),
                Style = ProgressBarStyle.Continuous,
                BackColor = Color.FromArgb(240, 244, 248)
            };
            this.Controls.Add(progressBar);

            // نص التحميل
            lblLoading = new Label
            {
                Text = "جاري التحميل...",
                Location = new Point(30, 420),
                Size = new Size(540, 25),
                Font = new Font("Segoe UI", 10),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(108, 117, 125)
            };
            this.Controls.Add(lblLoading);

            // مؤقت التحميل
            loadingTimer = new FormsTimer();
            loadingTimer.Interval = 30;
            loadingTimer.Tick += LoadingTimer_Tick;
        }

        private int progressValue = 0;

        private void LoadingTimer_Tick(object sender, EventArgs e)
        {
            progressValue += 2;
            progressBar.Value = Math.Min(progressValue, 100);

            // تحديث رسالة التحميل
            int dots = (progressValue / 10) % 4;
            lblLoading.Text = "جاري التحميل" + new string('.', dots);

            if (progressValue >= 100)
            {
                loadingTimer.Stop();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        public void StartLoading()
        {
            progressValue = 0;
            progressBar.Value = 0;
            loadingTimer.Start();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                loadingTimer?.Dispose();
                logoPictureBox?.Image?.Dispose();
                logoPictureBox?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}