#nullable disable
using System;
using System.IO;
using System.Windows.Forms;

namespace EasyDent2
{
    /// <summary>
    /// نقطة الانطلاق الرئيسية للتطبيق
    /// Updated to support Enhanced UI v3.1 - DABI ATLANTE Style
    /// Powered by ROFA SOFT
    /// </summary>
    static class Program
    {
        /// <summary>
        /// نقطة الانطلاق الرئيسية للتطبيق بأكمله.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // تفعيل التوافق المتقدم مع الشاشات عالية الدقة لمنع تشوه حجم الخطوط والأزرار
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // تهيئة وتأمين وجود مجلد حفظ الحساسات والبروفايلات قبل بدء تشغيل الواجهة
                var sensorFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sensors");
                if (!Directory.Exists(sensorFolder))
                {
                    Directory.CreateDirectory(sensorFolder);
                }

                // إنشاء مجلد حفظ الصور
                var saveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Save");
                if (!Directory.Exists(saveFolder))
                {
                    Directory.CreateDirectory(saveFolder);
                }
            }
            catch
            {
                // تجاوز أخطاء صلاحيات ويندوز إن وجدت لضمان عدم تعطل الإقلاع
            }

            // عرض شاشة البداية (Splash Screen)
            using (var splashForm = new SplashForm())
            {
                splashForm.StartLoading();
                splashForm.ShowDialog();
            }

            // تحميل الثيم المحفوظ
            ThemeManager.LoadThemePreference();

            try
            {
                // تشغيل النافذة الرئيسية المطورة بالتصميم المحسّن
                // استخدام Form1Enhanced للحصول على التصميم الجديد DABI ATLANTE
                Application.Run(new Form1Enhanced());

                // يمكن التبديل إلى النسخة القديمة بتعليق السطر أعلاه واستخدام:
                // Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                // معالجة الأخطاء الحرجة
                MessageBox.Show(
                    $"❌ حدث خطأ أثناء تشغيل التطبيق:\n\n" +
                    $"الرسالة: {ex.Message}\n\n" +
                    $"النوع: {ex.GetType().Name}\n\n" +
                    $"الكود:\n{ex.StackTrace}",
                    "⚠️ خطأ في البدء",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}
