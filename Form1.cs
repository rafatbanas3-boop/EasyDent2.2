#nullable disable
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EasyDent2.Properties;

namespace EasyDent2
{
    public partial class Form1 : Form
    {
        private readonly SensorManager sensorManager;
        private IScannerSession scanner;
        private SensorProfile activeProfile;
        private Bitmap originalImage;
        private string activeColorMap = "None";

        public Form1()
        {
            InitializeComponent();
            
            // تحميل الثيم المحفوظ
            ThemeManager.LoadThemePreference();
            
            sensorManager = new SensorManager();

            scanner = new ReflectionScanner();
            RegisterScannerEvents();

            // ربط أحداث الأزرار التفاعلية بالخلفية البرمجية بشكل مباشر
            this.btnSavePatient.Click += BtnSavePatient_Click;
            this.btnTwainConnect.Click += BtnTwainConnect_Click;
            this.btnTwain.Click += BtnTwain_Click;
            this.btnZoomIn.Click += BtnZoomIn_Click;
            this.btnZoomOut.Click += BtnZoomOut_Click;
            this.btnMeasure.Click += BtnMeasure_Click;
            this.btnSettings.Click += (s, e) => { using var sf = new SettingsForm(); sf.ShowDialog(this); };
            this.btnPrint.Click += BtnPrint_Click;
            
            // تطبيق الثيم
            ApplyThemeToForm();
        }

        /// <summary>
        /// تطبيق الثيم على جميع عناصر الواجهة
        /// </summary>
        private void ApplyThemeToForm()
        {
            ThemeManager.ApplyTheme(this);
            
            // تحديث ألوان الأزرار بناءً على وظائفها
            var colors = ThemeManager.GetCurrentColors();
            
            btnSavePatient.BackColor = colors.Success;
            btnTwainConnect.BackColor = colors.Danger;
            btnTwain.BackColor = colors.Secondary;
            btnSettings.BackColor = colors.Info;
            btnOpen.BackColor = colors.Info;
            btnTestOpenCv.Click += (s, e) => btnTestOpenCv.BackColor = Color.FromArgb(111, 66, 193);
            btnMeasure.BackColor = colors.Warning;
            btnPrint.BackColor = colors.Secondary;
            btnZoomIn.BackColor = colors.Secondary;
            btnZoomOut.BackColor = colors.Secondary;
            
            // تحديث شريط الحالة
            UpdateStatusBar();
        }

        /// <summary>
        /// تحديث شريط الحالة مع معلومات الثيم والبرنامج
        /// </summary>
        private void UpdateStatusBar()
        {
            var themeInfo = ThemeManager.CurrentTheme.ToString();
            toolStripStatusLabel1.Text = $"🦷 EasyDent 2 Professional v3.0 | الثيم: {themeInfo} | ROFA SOFT";
        }

        private void RegisterScannerEvents()
        {
            if (scanner == null) return;
            scanner.InfoMessage += Scanner_InfoMessage;
            scanner.ImageAcquired += Scanner_ImageAcquired;
        }

        private void UnregisterScannerEvents()
        {
            if (scanner == null) return;
            scanner.InfoMessage -= Scanner_InfoMessage;
            scanner.ImageAcquired -= Scanner_ImageAcquired;
        }

        private void Scanner_ImageAcquired(object sender, Image e)
        {
            if (e == null) return;
            
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateImageDisplay(e)));
            }
            else
            {
                UpdateImageDisplay(e);
            }
        }

        private void UpdateImageDisplay(Image e)
        {
            var prev = pictureBox1.Image;
            pictureBox1.Image = new Bitmap(e);
            originalImage = new Bitmap(e);
            prev?.Dispose();

            var savedPath = AppSettings.Instance.CalibrationFilePath;
            if (!string.IsNullOrWhiteSpace(savedPath) && File.Exists(savedPath))
            {
                if (sensorManager.ImportCalibrationFile(savedPath, out var prof, this))
                {
                    activeProfile = prof;
                    toolStripStatusLabel1.Text = $"وضع المعايرة النشط: {prof.Name} ({prof.DefaultPixelsPerMm:F2} px/mm)";
                    return;
                }
            }

            var matched = sensorManager.TryAutoApplyCalibrationFromImage(e, null, this);
            if (matched != null)
            {
                activeProfile = matched;
                toolStripStatusLabel1.Text = $"تم تطبيق بروفايل المعايرة التلقائي: {matched.DefaultPixelsPerMm:F2} px/mm";
            }
        }

        private void Scanner_InfoMessage(object sender, string e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => toolStripStatusLabel1.Text = e));
            }
            else
            {
                toolStripStatusLabel1.Text = e;
            }
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "ملفات صور الأشعة|*.png;*.jpg;*.jpeg;*.bmp|كل الملفات|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var img = Image.FromFile(ofd.FileName);
                    var prev = pictureBox1.Image;
                    pictureBox1.Image = new Bitmap(img);
                    originalImage = new Bitmap(img);
                    prev?.Dispose();

                    listBoxImages.Items.Add(Path.GetFileName(ofd.FileName));
                    toolStripStatusLabel1.Text = "تم تحميل وعرض ملف صورة الأشعة بنجاح.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ أثناء فتح الصورة: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnTestOpenCv_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image is Bitmap bmp)
            {
                try
                {
                    using var pop = new ColorMapPopupForm(activeColorMap);
                    if (pop.ShowDialog(this) == DialogResult.OK)
                    {
                        activeColorMap = pop.SelectedMap;
                        var processed = OpenCvImageProcessing.ApplyClaheAndDenoise(originalImage ?? bmp, 2.0, 8, 10);
                        var colored = OpenCvImageProcessing.ApplyColorMap(processed, activeColorMap);

                        var prev = pictureBox1.Image;
                        pictureBox1.Image = colored;
                        prev?.Dispose();
                        processed.Dispose();

                        toolStripStatusLabel1.Text = $"تم معالجة الأشعة بفلتر الألوان: {activeColorMap}";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ أثناء معالجة فلاتر الأشعة: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnImportCalibration_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "ملفات المعايرة|*.json;*.csv;*.txt;*.cor;*.png;*.jpg|كل الملفات|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (sensorManager.ImportCalibrationFile(ofd.FileName, out var profile, this))
                {
                    activeProfile = profile;
                    MessageBox.Show("تم استيراد ملف المعايرة بنجاح.");
                }
            }
        }

        private void BtnTwainConnect_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "جاري البحث واختبار توصيل أجهزة الأشعة الرقمية (TWAIN)...";
            Cursor = Cursors.WaitCursor;

            try
            {
                UnregisterScannerEvents();
                scanner?.Dispose();

                var realScanner = new NtwinScanner();
                if (realScanner.Open())
                {
                    var sources = realScanner.GetSourceNames().ToList();
                    if (sources.Count > 0)
                    {
                        realScanner.SelectSource(sources.First());
                        scanner = realScanner;
                        RegisterScannerEvents();

                        btnTwainConnect.Text = "السينسور متصل وجاهز ✔";
                        btnTwainConnect.BackColor = Color.FromArgb(40, 167, 69);
                        MessageBox.Show($"تم العثور واختبار السينسور بنجاح الموديل: {sources.First()}", "نجاح الاتصال", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        throw new Exception("لم يتم العثور على أي تعريفات نشطة لسينسور الأسنان بالكمبيوتر.");
                    }
                }
                else
                {
                    throw new Exception("تعذر الاتصال بمدير موديول TWAIN.");
                }
            }
            catch (Exception ex)
            {
                scanner = new ReflectionScanner();
                RegisterScannerEvents();
                scanner.Open();

                btnTwainConnect.Text = "السينسور غير متصل ❌";
                btnTwainConnect.BackColor = Color.FromArgb(220, 53, 69);
                MessageBox.Show($"الحساس غير متصل أو يحتاج تعريف. تم تفعيل موديول المحاكاة التلقائي الآمن.\nتفاصيل: {ex.Message}", "تحذير", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BtnTwain_Click(object sender, EventArgs e)
        {
            if (scanner != null)
            {
                if (!scanner.Open()) btnTwainConnect.PerformClick();
                scanner.Acquire();
            }
        }

        private void BtnAcquireSimulated_Click(object sender, EventArgs e)
        {
            using var cd = new CountdownForm(5);
            if (cd.ShowDialog(this) == DialogResult.OK)
            {
                if (scanner == null)
                {
                    scanner = new ReflectionScanner();
                    RegisterScannerEvents();
                }
                scanner.Open();
                scanner.Acquire();
            }
        }

        private void BtnMeasure_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                double ppm = (activeProfile != null) ? activeProfile.DefaultPixelsPerMm : 0.0;
                using var mf = new MeasurementForm(pictureBox1.Image, ppm);
                mf.ShowDialog(this);
            }
        }

        private void BtnSavePatient_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null || string.IsNullOrWhiteSpace(txtPatientName.Text)) return;

            try
            {
                var saveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Save");
                if (!Directory.Exists(saveFolder)) Directory.CreateDirectory(saveFolder);

                string safeName = string.Join("_", txtPatientName.Text.Split(Path.GetInvalidFileNameChars()));
                string fileName = $"{safeName}_Tooth_{cmbToothNumber.SelectedItem}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string fullPath = Path.Combine(saveFolder, fileName);

                pictureBox1.Image.Save(fullPath, ImageFormat.Png);
                listBoxImages.Items.Add(fileName);
                toolStripStatusLabel1.Text = $"تم حفظ الملف: {fileName}";
                MessageBox.Show("تم حفظ صورة الأشعة بنجاح!", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء الحفظ: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnInvert_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image is Bitmap bmp)
            {
                try
                {
                    var inverted = new Bitmap(bmp.Width, bmp.Height);
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            var pixel = bmp.GetPixel(x, y);
                            inverted.SetPixel(x, y, Color.FromArgb(255 - pixel.R, 255 - pixel.G, 255 - pixel.B));
                        }
                    }

                    var prev = pictureBox1.Image;
                    pictureBox1.Image = inverted;
                    prev?.Dispose();
                    toolStripStatusLabel1.Text = "تم عكس ألوان الصورة بنجاح.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ: " + ex.Message);
                }
            }
        }

        private void BtnGrayscale_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image is Bitmap bmp)
            {
                try
                {
                    var gray = new Bitmap(bmp.Width, bmp.Height);
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        for (int x = 0; x < bmp.Width; x++)
                        {
                            var pixel = bmp.GetPixel(x, y);
                            int grayValue = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                            gray.SetPixel(x, y, Color.FromArgb(grayValue, grayValue, grayValue));
                        }
                    }

                    var prev = pictureBox1.Image;
                    pictureBox1.Image = gray;
                    prev?.Dispose();
                    toolStripStatusLabel1.Text = "تم تحويل الصورة إلى تدرج رمادي بنجاح.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ: " + ex.Message);
                }
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (originalImage != null)
            {
                var prev = pictureBox1.Image;
                pictureBox1.Image = new Bitmap(originalImage);
                prev?.Dispose();
                toolStripStatusLabel1.Text = "تم إعادة ضبط الصورة إلى حالتها الأصلية.";
            }
        }

        private void BtnZoomIn_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null && pictureBox1.SizeMode == PictureBoxSizeMode.Zoom)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
                toolStripStatusLabel1.Text = "تكبير الصورة مفعل.";
            }
        }

        private void BtnZoomOut_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                toolStripStatusLabel1.Text = "تم تصغير الصورة للملاءمة.";
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                try
                {
                    using var printDialog = new PrintDialog();
                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        using var printDoc = new System.Drawing.Printing.PrintDocument();
                        printDoc.PrintPage += (s, pe) =>
                        {
                            pe.Graphics.DrawImage(pictureBox1.Image, pe.MarginBounds);
                        };
                        printDoc.Print();
                        toolStripStatusLabel1.Text = "تم إرسال الصورة للطباعة بنجاح.";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("خطأ أثناء الطباعة: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterScannerEvents();
            scanner?.Dispose();
            originalImage?.Dispose();
            pictureBox1.Image?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
