#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EasyDent2
{
    public class ReflectionScanner : IScannerSession
    {
        public event EventHandler<Image> ImageAcquired;
        public event EventHandler<string> InfoMessage;

        private bool _open;

        public bool Open()
        {
            _open = true;
            InfoMessage?.Invoke(this, "تم تشغيل وتفعيل الحساس المحاكي بنجاح (Simulation Mode).");
            return true;
        }

        public IEnumerable<string> GetSourceNames()
        {
            yield return "الحساس الافتراضي للمحاكاة (Simulated Sensor)";
        }

        public bool SelectSource(string name)
        {
            InfoMessage?.Invoke(this, $"تم اختيار جهاز السحب: {name}");
            return true;
        }

        public bool Acquire()
        {
            if (!_open)
            {
                InfoMessage?.Invoke(this, "خطأ: الحساس ليس في وضع الاستعداد أو الإقلاع.");
                return false;
            }

            using var ofd = new OpenFileDialog();
            ofd.Filter = "ملفات صور الأشعة|*.png;*.jpg;*.jpeg;*.bmp|كل الملفات|*.*";
            ofd.Title = "محاكاة التقاط صورة من الحساس - اختر ملف أشعة";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // قراءة وتمرير الصورة عبر حدث التقاط الصورة لربطها بالواجهة الرئيسية
                    using var img = Image.FromFile(ofd.FileName);
                    ImageAcquired?.Invoke(this, new Bitmap(img));
                    InfoMessage?.Invoke(this, $"نجاح الالتقاط: تم استلام صورة الأشعة من المسار المحدد.");
                    return true;
                }
                catch (Exception ex)
                {
                    InfoMessage?.Invoke(this, "خطأ أثناء محاكاة السحب: " + ex.Message);
                    return false;
                }
            }

            InfoMessage?.Invoke(this, "تم إلغاء عملية التقاط الأشعة بواسطة المستخدم.");
            return false;
        }

        public void Close()
        {
            _open = false;
            InfoMessage?.Invoke(this, "تم إغلاق الحساس الافتراضي المحاكي.");
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }
    }
}
