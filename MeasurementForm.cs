#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace EasyDent2
{
    public class MeasurementForm : Form
    {
        private PictureBox pic;
        private Label lbl;
        private Bitmap bmp;
        private Point? p1, p2;
        private double _pixelsPerMm;

        // قمنا بإضافة معامل PixelsPerMm هنا ليتم تمريره من الفورم الرئيسي وحساب المقاس الفعلي بالمليمتر
        public MeasurementForm(Image image, double pixelsPerMm)
        {
            _pixelsPerMm = pixelsPerMm;
            Text = "أداة قياس طول القناة والجذر (انقر نقطتين)";
            Width = 800; Height = 600;
            StartPosition = FormStartPosition.CenterParent;

            pic = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.Black };
            Controls.Add(pic);

            lbl = new Label
            {
                Text = "انقر على نقطتين (بداية ونهاية الجذر أو القناة المراد قياسها).",
                Dock = DockStyle.Bottom,
                Height = 35,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.WhiteSmoke,
                ForeColor = Color.DarkSlateGray
            };
            Controls.Add(lbl);

            if (image != null) bmp = new Bitmap(image);
            pic.Image = bmp;
            pic.MouseClick += Pic_MouseClick;
        }

        private void Pic_MouseClick(object sender, MouseEventArgs e)
        {
            if (bmp == null) return;
            var rect = GetImageRect();
            if (!rect.Contains(e.Location)) return;

            var ix = (int)((e.X - rect.X) * bmp.Width / (double)rect.Width);
            var iy = (int)((e.Y - rect.Y) * bmp.Height / (double)rect.Height);

            if (p1 == null)
            {
                p1 = new Point(ix, iy);
                lbl.Text = "تم اختيار النقطة الأولى. انقر الآن على نقطة النهاية.";
            }
            else if (p2 == null)
            {
                p2 = new Point(ix, iy);
                var distPx = Distance(p1.Value, p2.Value);

                // حساب الطول بالمليمتر إذا كانت هناك معايرة جاهزة ومحملة
                if (_pixelsPerMm > 0)
                {
                    var distMm = distPx / _pixelsPerMm;
                    lbl.Text = $"المسافة المحسوبة: {distMm:F2} مليمتر (الطول الفعلي بالأشعة) | {distPx:F1} بكسل";
                }
                else
                {
                    lbl.Text = $"المسافة: {distPx:F1} بكسل (الرجاء عمل معايرة أولاً للحصول على المقاس بالمليمتر).";
                }
            }
            else
            {
                p1 = new Point(ix, iy);
                p2 = null;
                lbl.Text = "أعدت اختيار نقطة البداية. انقر نقطة النهاية.";
            }
            RefreshPreview();
        }

        private double Distance(Point a, Point b)
        {
            var dx = a.X - b.X;
            var dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
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
            if (bmp == null) return;

            var oldImage = pic.Image;
            var preview = new Bitmap(bmp);

            using (var g = Graphics.FromImage(preview))
            {
                if (p1 != null) g.FillEllipse(Brushes.Red, p1.Value.X - 5, p1.Value.Y - 5, 10, 10);
                if (p2 != null) g.FillEllipse(Brushes.Blue, p2.Value.X - 5, p2.Value.Y - 5, 10, 10);
                if (p1 != null && p2 != null)
                {
                    using var pen = new Pen(Color.LimeGreen, 2);
                    g.DrawLine(pen, p1.Value.X, p1.Value.Y, p2.Value.X, p2.Value.Y);
                }
            }
            pic.Image = preview;
            if (oldImage != null && oldImage != bmp) oldImage.Dispose(); // تنظيف فوري لمنع انهيار الرام
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (bmp != null) bmp.Dispose();
            if (pic.Image != null) pic.Image.Dispose();
        }
    }
}
