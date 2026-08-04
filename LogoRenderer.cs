#nullable disable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EasyDent2
{
    /// <summary>
    /// فئة رسم شعار ROFA SOFT - مرسوم بالكود بدون صور خارجية
    /// Powered by ROFA SOFT - Dental Imaging Solutions
    /// </summary>
    public static class LogoRenderer
    {
        /// <summary>
        /// رسم شعار ROFA المتكامل (حرف + سن)
        /// </summary>
        public static Bitmap RenderFullLogo(int width = 200, int height = 200)
        {
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                // الخلفية
                using (var brush = new SolidBrush(Color.FromArgb(240, 244, 248)))
                {
                    g.FillRectangle(brush, 0, 0, width, height);
                }

                // رسم السن (الجزء الأيسر)
                DrawToothIcon(g, 20, 30, 60, 120, Color.FromArgb(102, 51, 153));

                // رسم حرف R (الجزء الأيمن)
                DrawLetterR(g, 100, 40, 80, Color.FromArgb(0, 102, 204));

                // كتابة الاسم تحت الشعار
                using (var font = new Font("Segoe UI", 11, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.FromArgb(33, 33, 66)))
                {
                    var textSize = g.MeasureString("ROFA SOFT", font);
                    g.DrawString("ROFA SOFT", font, brush, (width - textSize.Width) / 2, height - 35);
                }

                // كتابة الشعار الجانبي
                using (var font = new Font("Segoe UI", 7, FontStyle.Italic))
                using (var brush = new SolidBrush(Color.FromArgb(102, 51, 153)))
                {
                    g.DrawString("Dental Imaging", font, brush, 15, height - 15);
                }
            }

            return bitmap;
        }

        /// <summary>
        /// رسم أيقونة السن فقط
        /// </summary>
        public static Bitmap RenderToothOnly(int width = 80, int height = 100)
        {
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                DrawToothIcon(g, 5, 5, width - 10, height - 10, Color.FromArgb(102, 51, 153));
            }
            return bitmap;
        }

        /// <summary>
        /// رسم أيقونة السن بألوان متعددة
        /// </summary>
        private static void DrawToothIcon(Graphics g, int x, int y, int width, int height, Color color)
        {
            // الجسم الرئيسي للسن
            using (var path = new GraphicsPath())
            {
                // شكل السن (الجزء العلوي مدبب)
                var points = new PointF[]
                {
                    new PointF(x + width / 2, y),                  // القمة المدببة
                    new PointF(x + width, y + height / 3),        // الزاوية اليمنى العلوية
                    new PointF(x + width, y + height * 2 / 3),    // الزاوية اليمنى
                    new PointF(x + width * 3 / 4, y + height),    // الزاوية اليمنى السفلى
                    new PointF(x + width / 4, y + height),        // الزاوية اليسرى السفلى
                    new PointF(x, y + height * 2 / 3),            // الزاوية اليسرى
                    new PointF(x, y + height / 3)                 // الزاوية اليسرى العلوية
                };

                path.AddPolygon(points);
                path.CloseFigure();

                using (var brush = new SolidBrush(color))
                {
                    g.FillPath(brush, path);
                }

                using (var pen = new Pen(Color.FromArgb(Math.Max(0, color.R - 50), Math.Max(0, color.G - 50), Math.Max(0, color.B - 50)), 2))
                {
                    g.DrawPath(pen, path);
                }
            }

            // رسم خط على السن (تأثير 3D)
            using (var pen = new Pen(Color.FromArgb(255, 255, 255, 100), 1.5f))
            {
                g.DrawLine(pen, x + width / 3, y + height / 4, x + width / 3, y + height * 3 / 4);
            }

            // رسم جذر السن
            using (var brush = new SolidBrush(Color.FromArgb(color.R - 30, color.G - 30, color.B - 30)))
            {
                int rootY = y + (int)(height * 0.8);
                g.FillRectangle(brush, x + width / 3, rootY, width / 3, height - rootY + 5);

                using (var pen = new Pen(Color.FromArgb(color.R - 50, color.G - 50, color.B - 50), 1))
                {
                    g.DrawRectangle(pen, x + width / 3, rootY, width / 3, height - rootY + 5);
                }
            }
        }

        /// <summary>
        /// رسم حرف R احترافي
        /// </summary>
        private static void DrawLetterR(Graphics g, int x, int y, int size, Color color)
        {
            using (var font = new Font("Arial", size * 0.6f, FontStyle.Bold))
            using (var brush = new SolidBrush(color))
            {
                g.DrawString("R", font, brush, x, y);
            }
        }

        /// <summary>
        /// رسم الشعار البسيط للشريط العلوي
        /// </summary>
        public static Bitmap RenderSimpleLogo(int size = 40)
        {
            var bitmap = new Bitmap(size, size);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                DrawToothIcon(g, 2, 2, size - 4, size - 4, Color.FromArgb(102, 51, 153));
            }
            return bitmap;
        }

        /// <summary>
        /// رسم شعار مع نص (للصفحات المختلفة)
        /// </summary>
        public static Bitmap RenderLogoWithText(string text, int width = 400, int height = 150)
        {
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.FromArgb(240, 244, 248));
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

                // رسم السن الكبير
                DrawToothIcon(g, 20, 20, 80, 110, Color.FromArgb(102, 51, 153));

                // رسم النص الرئيسي
                using (var font = new Font("Segoe UI", 16, FontStyle.Bold))
                using (var brush = new SolidBrush(Color.FromArgb(33, 33, 66)))
                {
                    g.DrawString("EasyDent 2 Professional", font, brush, 120, 30);
                }

                // رسم النص الثانوي
                using (var font = new Font("Segoe UI", 10))
                using (var brush = new SolidBrush(Color.FromArgb(102, 51, 153)))
                {
                    g.DrawString(text, font, brush, 120, 70);
                }

                // رسم النص السفلي
                using (var font = new Font("Segoe UI", 9, FontStyle.Italic))
                using (var brush = new SolidBrush(Color.FromArgb(108, 117, 125)))
                {
                    g.DrawString("Powered by ROFA SOFT", font, brush, 120, 110);
                }
            }

            return bitmap;
        }
    }
}
