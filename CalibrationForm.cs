#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;
using EasyDent2.Properties; // استدعاء كلاس الإعدادات المطور

namespace EasyDent2
{
    public class CalibrationForm : Form
    {
        private PictureBox pic;
        private Button btnSave;
        private Label lblInfo;
        private Bitmap loaded;
        private Point? p1, p2;
        public double PixelsPerMm { get; private set; }

        public CalibrationForm() { Initialize(); }
        public CalibrationForm(string imagePath) : this() { LoadImageFile(imagePath); }

        private void Initialize()
        {
            Text = "معايرة أبعاد الصورة الحقيقية";
            Width = 800; Height = 600;
            StartPosition = FormStartPosition.CenterParent;

            pic = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.Black };
            Controls.Add(pic);

            var panel = new Panel { Dock = DockStyle.Bottom, Height = 44, BackColor = Color.WhiteSmoke };
            btnSave = new Button { Text = "حفظ المعايرة", Dock = DockStyle.Right, Width = 120, FlatStyle = FlatStyle.Flat, BackColor = Color.SteelBlue, ForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            btnSave.Click += BtnSave_Click;

            lblInfo = new Label { Text = "انقر على نقطتين لمعرفة الطول، ثم اضغط حفظ.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            panel.Controls.Add(lblInfo);
            panel.Controls.Add(btnSave);
            Controls.Add(panel);

            pic.MouseClick += Pic_MouseClick;
        }

        public void LoadImageFile(string path)
        {
            try
            {
                // تحرير وتنظيف الصور القديمة فوراً من الذاكرة العشوائية
                if (loaded != null) loaded.Dispose();
                if (pic.Image != null) pic.Image.Dispose();

                loaded = (Bitmap)Image.FromFile(path);
                pic.Image = new Bitmap(loaded);
                p1 = p2 = null;
                lblInfo.Text = "انقر على نقطتين لمعرفة الطول، ثم اضغط حفظ.";

                // حفظ المسار في الإعدادات المشتركة بشكل آمن
                AppSettings.Instance.CalibrationFilePath = path;
                AppSettings.Instance.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ بتحميل الصورة: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Pic_MouseClick(object sender, MouseEventArgs e)
        {
            if (loaded == null) return;
            var imgRect = GetImageRect();
            if (!imgRect.Contains(e.Location)) return;

            var ix = (int)((e.X - imgRect.X) * loaded.Width / (double)imgRect.Width);
            var iy = (int)((e.Y - imgRect.Y) * loaded.Height / (double)imgRect.Height);

            if (p1 == null) { p1 = new Point(ix, iy); lblInfo.Text = "النقطة الأولى مختارة. انقر النقطة الثانية."; }
            else if (p2 == null) { p2 = new Point(ix, iy); lblInfo.Text = "نقطتان مختارتان. اضغط حفظ لإدخال المسافة الحقيقية (مم)."; }
            else { p1 = new Point(ix, iy); p2 = null; lblInfo.Text = "أعدت اختيار النقطة الأولى. انقر الثانية."; }
            RefreshPreview();
        }

        private Rectangle GetImageRect()
        {
            if (pic.Image == null) return Rectangle.Empty;
            var img = pic.Image;
            var pbw = pic.ClientSize.Width;
            var pbh = pic.ClientSize.Height;
            double ratio = Math.Min(pbw / (double)img.Width, pbh / (double)img.Height);
            int w = (int)(img.Width * ratio);
            int h = (int)(img.Height * ratio);
            int x = (pbw - w) / 2;
            int y = (pbh - h) / 2;
            return new Rectangle(x, y, w, h);
        }

        private void RefreshPreview()
        {
            if (loaded == null) return;

            var oldImage = pic.Image; // الاحتفاظ بالصورة السابقة لتدميرها
            var preview = new Bitmap(loaded);

            using (var g = Graphics.FromImage(preview))
            {
                if (p1 != null) g.FillEllipse(Brushes.Red, p1.Value.X - 4, p1.Value.Y - 4, 8, 8);
                if (p2 != null) g.FillEllipse(Brushes.Blue, p2.Value.X - 4, p2.Value.Y - 4, 8, 8);
                if (p1 != null && p2 != null)
                {
                    using var pen = new Pen(Color.Yellow, 2);
                    g.DrawLine(pen, p1.Value.X, p1.Value.Y, p2.Value.X, p2.Value.Y);
                }
            }

            pic.Image = preview;

            // تدمير الصورة السابقة فوراً لتوفير مساحة الـ RAM
            if (oldImage != null && oldImage != loaded) oldImage.Dispose();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (p1 == null || p2 == null)
            {
                MessageBox.Show("انقر نقطتين على الصورة أولاً لإجراء المعايرة.", "معايرة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using var dlg = new InputDistanceForm();
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                double mm = dlg.DistanceMm;
                var dx = p2.Value.X - p1.Value.X;
                var dy = p2.Value.Y - p1.Value.Y;
                var distPx = Math.Sqrt(dx * dx + dy * dy);
                if (mm <= 0)
                {
                    MessageBox.Show("القيمة المدخلة غير صحيحة.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                PixelsPerMm = distPx / mm;
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (loaded != null) loaded.Dispose();
            if (pic.Image != null) pic.Image.Dispose();
        }
    }

    public class InputDistanceForm : Form
    {
        private NumericUpDown nud;
        private Button ok, cancel;
        public double DistanceMm => (double)nud.Value;

        public InputDistanceForm()
        {
            Text = "أدخل المسافة الحقيقية بالأشعة (مم)";
            Width = 360; Height = 130;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;

            nud = new NumericUpDown { DecimalPlaces = 2, Minimum = 0.01M, Maximum = 10000, Value = 10, Dock = DockStyle.Top, Font = new Font("Segoe UI", 11) };
            ok = new Button { Text = "موافق", DialogResult = DialogResult.OK, Dock = DockStyle.Right, Width = 100, FlatStyle = FlatStyle.Flat, BackColor = Color.SteelBlue, ForeColor = Color.White };
            cancel = new Button { Text = "إلغاء", DialogResult = DialogResult.Cancel, Dock = DockStyle.Left, Width = 100, FlatStyle = FlatStyle.Flat, BackColor = Color.LightGray };

            var panel = new Panel { Dock = DockStyle.Bottom, Height = 40 };
            panel.Controls.Add(ok); panel.Controls.Add(cancel);
            Controls.Add(nud); Controls.Add(panel);
        }
    }
}
