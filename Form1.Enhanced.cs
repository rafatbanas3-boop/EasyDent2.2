#nullable disable
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using EasyDent2.Properties;

namespace EasyDent2
{
    /// <summary>
    /// نموذج محسّن للواجهة الرئيسية - Enhanced Version v3.1
    /// مستوحاة من DABI ATLANTE Professional Design
    /// Powered by ROFA SOFT
    /// </summary>
    public partial class Form1Enhanced : Form
    {
        private readonly SensorManager sensorManager;
        private IScannerSession scanner;
        private SensorProfile activeProfile;
        private Bitmap originalImage;
        private string activeColorMap = "None";
        private double currentBrightness = 0;
        private double currentContrast = 0;

        public Form1Enhanced()
        {
            InitializeComponent();
            
            // تحميل الثيم المحفوظ
            ThemeManager.LoadThemePreference();
            
            sensorManager = new SensorManager();
            scanner = new ReflectionScanner();
            RegisterScannerEvents();

            // ربط أحداث الأزرار
            BindToolbarEvents();
            BindControlEvents();

            // تطبيق الثيم
            ApplyThemeToForm();

            // بدء تحديث الوقت
            StartDateTimeUpdater();
        }

        #region === INITIALIZATION & EVENTS ===

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

        private void BindToolbarEvents()
        {
            toolbarButtons[0].Click += (s, e) => BtnOpen_Click();           // 📂 فتح
            toolbarButtons[1].Click += (s, e) => BtnTestOpenCv_Click();     // ✨ تحسين
            toolbarButtons[2].Click += (s, e) => BtnInvert_Click();          // 🔄 عكس
            toolbarButtons[3].Click += (s, e) => BtnGrayscale_Click();       // ⚪ رمادي
            toolbarButtons[4].Click += (s, e) => BtnReset_Click();           // ↶ إعادة
            toolbarButtons[5].Click += (s, e) => BtnImportCalibration_Click(); // 📥 معايرة
            toolbarButtons[6].Click += (s, e) => BtnAcquireSimulated_Click(); // 📷 التقاط
            toolbarButtons[7].Click += (s, e) => BtnMeasure_Click();         // 📏 قياس
            toolbarButtons[8].Click += (s, e) => BtnPrint_Click();           // 🖨️ طباعة
            toolbarButtons[9].Click += (s, e) => BtnSave_Click();            // 💾 حفظ
            toolbarButtons[10].Click += (s, e) => BtnTwainConnect_Click();   // 🔗 توصيل
            toolbarButtons[11].Click += (s, e) => BtnSettings_Click();       // ⚙️ إعدادات
        }

        private void BindControlEvents()
        {
            btnSave.Click += (s, e) => BtnSave_Click();
            trackBarBrightness.ValueChanged += TrackBarBrightness_ValueChanged;
            trackBarContrast.ValueChanged += TrackBarContrast_ValueChanged;
            listBoxImages.SelectedIndexChanged += ListBoxImages_SelectedIndexChanged;
        }

        private void ApplyThemeToForm()
        {
            ThemeManager.ApplyTheme(this);
            
            var colors = ThemeManager.GetCurrentColors();
            
            // تحديث ألوان الأزرار
            toolbarButtons[0].FillColor = colors.Info;
            toolbarButtons[1].FillColor = Color.FromArgb(111, 66, 193);
            toolbarButtons[9].FillColor = colors.Success;
            toolbarButtons[10].FillColor = colors.Danger;
            toolbarButtons[11].FillColor = Color.FromArgb(0, 123, 255);

            UpdateStatusMessage("🟢 نظام الأشعة جاهز ومستقر... | الثيم: " + ThemeManager.CurrentTheme.ToString());
        }

        private void StartDateTimeUpdater()
        {
            Timer dateTimeTimer = new Timer();
            dateTimeTimer.Interval = 1000;
            dateTimeTimer.Tick += (s, e) => 
            {
                lblDateTime.Text = "📅 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };
            dateTimeTimer.Start();
        }

        #endregion

        #region === SCANNER EVENTS ===

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
            var prev = pictureBoxMain.Image;
            pictureBoxMain.Image = new Bitmap(e);
            originalImage = new Bitmap(e);
            prev?.Dispose();

            // إعادة تعيين القيم
            currentBrightness = 0;
            currentContrast = 0;
            trackBarBrightness.Value = 0;
            trackBarContrast.Value = 0;

            // محاولة تطبيق المعايرة
            var savedPath = AppSettings.Instance.CalibrationFilePath;
            if (!string.IsNullOrWhiteSpace(savedPath) && File.Exists(savedPath))
            {
                if (sensorManager.ImportCalibrationFile(savedPath, out var prof, this))
                {
                    activeProfile = prof;
                    txtCalibrationValue.Text = prof.DefaultPixelsPerMm.ToString("F2");
                    UpdateStatusMessage($"✅ تم تطبيق المعايرة: {prof.DefaultPixelsPerMm:F2} px/mm");
                    return;
                }
            }

            var matched = sensorManager.TryAutoApplyCalibrationFromImage(e, null, this);
            if (matched != null)
            {
                activeProfile = matched;
                txtCalibrationValue.Text = matched.DefaultPixelsPerMm.ToString("F2");
                UpdateStatusMessage($"⚡ معايرة تلقائية: {matched.DefaultPixelsPerMm:F2} px/mm");
            }

            // تحديث الرسم البياني
            UpdateHistogram(originalImage);
        }

        private void Scanner_InfoMessage(object sender, string e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => UpdateStatusMessage(e)));
            }
            else
            {
                UpdateStatusMessage(e);
            }
        }

        #endregion

        #region === TOOLBAR BUTTONS ===

        private void BtnOpen_Click()
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "ملفات صور الأشعة|*.png;*.jpg;*.jpeg;*.bmp|كل الملفات|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var img = Image.FromFile(ofd.FileName);
                    var prev = pictureBoxMain.Image;
                    pictureBoxMain.Image = new Bitmap(img);
                    originalImage = new Bitmap(img);
                    prev?.Dispose();

                    string fileName = Path.GetFileName(ofd.FileName);
                    if (!listBoxImages.Items.Contains(fileName))
                        listBoxImages.Items.Add(fileName);

                    UpdateHistogram(originalImage);
                    UpdateStatusMessage("✅ تم تحميل صورة الأشعة بنجاح: " + fileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ خطأ أثناء فتح الصورة: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnTestOpenCv_Click()
        {
            if (pictureBoxMain.Image is Bitmap bmp)
            {
                try
                {
                    using var pop = new ColorMapPopupForm(activeColorMap);
                    if (pop.ShowDialog(this) == DialogResult.OK)
                    {
                        activeColorMap = pop.SelectedMap;
                        var processed = OpenCvImageProcessing.ApplyClaheAndDenoise(originalImage ?? bmp, 2.0, 8, 10);
                        var colored = OpenCvImageProcessing.ApplyColorMap(processed, activeColorMap);

                        var prev = pictureBoxMain.Image;
                        pictureBoxMain.Image = colored;
                        prev?.Dispose();
                        processed.Dispose();

                        UpdateHistogram((Bitmap)pictureBoxMain.Image);
                        UpdateStatusMessage($"✨ تم تطبيق خريطة الألوان: {activeColorMap}");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ خطأ: " + ex.Message);
                }
            }
        }

        private void BtnInvert_Click()
        {
            if (pictureBoxMain.Image is Bitmap bmp)
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

                    var prev = pictureBoxMain.Image;
                    pictureBoxMain.Image = inverted;
                    prev?.Dispose();
                    UpdateHistogram(inverted);
                    UpdateStatusMessage("🔄 تم عكس ألوان الصورة بنجاح.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ خطأ: " + ex.Message);
                }
            }
        }

        private void BtnGrayscale_Click()
        {
            if (pictureBoxMain.Image is Bitmap bmp)
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

                    var prev = pictureBoxMain.Image;
                    pictureBoxMain.Image = gray;
                    prev?.Dispose();
                    UpdateHistogram(gray);
                    UpdateStatusMessage("⚪ تم تحويل الصورة إلى تدرج رمادي بنجاح.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ خطأ: " + ex.Message);
                }
            }
        }

        private void BtnReset_Click()
        {
            if (originalImage != null)
            {
                var prev = pictureBoxMain.Image;
                pictureBoxMain.Image = new Bitmap(originalImage);
                prev?.Dispose();
                currentBrightness = 0;
                currentContrast = 0;
                trackBarBrightness.Value = 0;
                trackBarContrast.Value = 0;
                UpdateHistogram(originalImage);
                UpdateStatusMessage("↶ تم إعادة ضبط الصورة إلى حالتها الأصلية.");
            }
        }

        private void BtnImportCalibration_Click()
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "ملفات المعايرة|*.json;*.csv;*.txt;*.cor;*.png;*.jpg|كل الملفات|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (sensorManager.ImportCalibrationFile(ofd.FileName, out var profile, this))
                {
                    activeProfile = profile;
                    txtCalibrationValue.Text = profile.DefaultPixelsPerMm.ToString("F2");
                    MessageBox.Show("✅ تم استيراد ملف المعايرة بنجاح.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateStatusMessage($"📥 تم تحميل معايرة: {profile.Name}");
                }
            }
        }

        private void BtnAcquireSimulated_Click()
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
                UpdateStatusMessage("📷 تم التقاط صورة جديدة...");
            }
        }

        private void BtnMeasure_Click()
        {
            if (pictureBoxMain.Image != null)
            {
                double ppm = (activeProfile != null) ? activeProfile.DefaultPixelsPerMm : 0.0;
                using var mf = new MeasurementForm(pictureBoxMain.Image, ppm);
                mf.ShowDialog(this);
            }
        }

        private void BtnSave_Click()
        {
            if (pictureBoxMain.Image == null)
            {
                MessageBox.Show("⚠️ الرجاء تحميل صورة أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPatientName.Text))
            {
                MessageBox.Show("⚠️ الرجاء إدخال اسم المريض", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var saveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Save");
                if (!Directory.Exists(saveFolder)) 
                    Directory.CreateDirectory(saveFolder);

                string safeName = string.Join("_", txtPatientName.Text.Split(Path.GetInvalidFileNameChars()));
                string patientFolder = Path.Combine(saveFolder, safeName);
                if (!Directory.Exists(patientFolder))
                    Directory.CreateDirectory(patientFolder);

                string toothNum = cmbToothNumber.SelectedItem?.ToString() ?? "0";
                string fileName = $"{safeName}_Tooth_{toothNum}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string fullPath = Path.Combine(patientFolder, fileName);

                pictureBoxMain.Image.Save(fullPath, ImageFormat.Png);

                if (!listBoxImages.Items.Contains(fileName))
                    listBoxImages.Items.Add(fileName);

                MessageBox.Show($"✅ تم حفظ الصورة بنجاح!\n{fileName}", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UpdateStatusMessage($"💾 تم حفظ الملف: {fileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ أثناء الحفظ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPrint_Click()
        {
            if (pictureBoxMain.Image != null)
            {
                try
                {
                    using var printDialog = new PrintDialog();
                    if (printDialog.ShowDialog() == DialogResult.OK)
                    {
                        using var printDoc = new System.Drawing.Printing.PrintDocument();
                        printDoc.PrintPage += (s, pe) =>
                        {
                            pe.Graphics.DrawImage(pictureBoxMain.Image, pe.MarginBounds);
                        };
                        printDoc.Print();
                        UpdateStatusMessage("🖨️ تم إرسال الصورة للطباعة بنجاح.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ خطأ أثناء الطباعة: {ex.Message}");
                }
            }
        }

        private void BtnTwainConnect_Click()
        {
            UpdateStatusMessage("🔍 جاري البحث عن أجهزة الأشعة الرقمية...");
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

                        toolbarButtons[10].FillColor = Color.FromArgb(40, 167, 69);
                        MessageBox.Show($"✅ تم توصيل السينسور بنجاح!\n{sources.First()}", "نجاح الاتصال", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpdateStatusMessage($"🔗 السينسور متصل وجاهز: {sources.First()}");
                    }
                    else
                    {
                        throw new Exception("لم يتم العثور على أي تعريفات نشطة.");
                    }
                }
                else
                {
                    throw new Exception("تعذر الاتصال بمدير TWAIN.");
                }
            }
            catch (Exception ex)
            {
                scanner = new ReflectionScanner();
                RegisterScannerEvents();
                scanner.Open();

                toolbarButtons[10].FillColor = Color.FromArgb(220, 53, 69);
                MessageBox.Show($"⚠️ الحساس غير متصل. تم تفعيل وضع المحاكاة.\n{ex.Message}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                UpdateStatusMessage("📴 وضع محاكاة: السينسور غير متصل");
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BtnSettings_Click()
        {
            using var sf = new SettingsForm();
            sf.ShowDialog(this);
        }

        #endregion

        #region === CONTROL EVENTS ===

        private void TrackBarBrightness_ValueChanged(object sender, EventArgs e)
        {
            currentBrightness = trackBarBrightness.Value;
            lblBrightnessValue.Text = trackBarBrightness.Value.ToString();
            ApplyImageAdjustments();
        }

        private void TrackBarContrast_ValueChanged(object sender, EventArgs e)
        {
            currentContrast = trackBarContrast.Value;
            lblContrastValue.Text = trackBarContrast.Value.ToString();
            ApplyImageAdjustments();
        }

        private void ListBoxImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxImages.SelectedIndex >= 0)
            {
                string selectedFile = listBoxImages.SelectedItem.ToString();
                UpdateStatusMessage($"📌 تم اختيار الصورة: {selectedFile}");
            }
        }

        private void ApplyImageAdjustments()
        {
            if (originalImage == null) return;

            try
            {
                Bitmap adjusted = new Bitmap(originalImage.Width, originalImage.Height);

                for (int y = 0; y < originalImage.Height; y++)
                {
                    for (int x = 0; x < originalImage.Width; x++)
                    {
                        Color pixel = originalImage.GetPixel(x, y);

                        // تطبيق السطوع
                        int r = Math.Min(255, Math.Max(0, (int)(pixel.R + currentBrightness * 2.55)));
                        int g = Math.Min(255, Math.Max(0, (int)(pixel.G + currentBrightness * 2.55)));
                        int b = Math.Min(255, Math.Max(0, (int)(pixel.B + currentBrightness * 2.55)));

                        // تطبيق التباين
                        float contrast = 1 + (float)currentContrast / 100;
                        r = Math.Min(255, Math.Max(0, (int)((r - 128) * contrast + 128)));
                        g = Math.Min(255, Math.Max(0, (int)((g - 128) * contrast + 128)));
                        b = Math.Min(255, Math.Max(0, (int)((b - 128) * contrast + 128)));

                        adjusted.SetPixel(x, y, Color.FromArgb(r, g, b));
                    }
                }

                var prev = pictureBoxMain.Image;
                if (prev != originalImage) prev?.Dispose();
                pictureBoxMain.Image = adjusted;

                UpdateHistogram(adjusted);
                UpdateStatusMessage($"🎨 تم تطبيق التعديلات: السطوع {currentBrightness}, التباين {currentContrast}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في تطبيق التعديلات: {ex.Message}");
            }
        }

        #endregion

        #region === UTILITY METHODS ===

        private void UpdateStatusMessage(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => lblStatus.Text = message));
            }
            else
            {
                lblStatus.Text = message;
            }
        }

        private void UpdateHistogram(Bitmap image)
        {
            try
            {
                if (image == null) return;

                // حساب توزيع الكثافة
                int[] histogram = new int[256];
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        Color pixel = image.GetPixel(x, y);
                        int gray = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                        histogram[gray]++;
                    }
                }

                // تحديث الرسم البياني
                histogramChart.Series[0].Points.Clear();
                for (int i = 0; i < 256; i++)
                {
                    histogramChart.Series[0].Points.AddXY(i, histogram[i]);
                }
            }
            catch
            {
                // تجاهل الأخطاء
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            UnregisterScannerEvents();
            scanner?.Dispose();
            originalImage?.Dispose();
            pictureBoxMain.Image?.Dispose();
            base.OnFormClosing(e);
        }

        #endregion
    }
}
