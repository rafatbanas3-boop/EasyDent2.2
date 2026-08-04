#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace EasyDent2
{
    /// <summary>
    /// مدير الثيمات - يدير تطبيق الثيمات المختلفة على الواجهة
    /// Powered by ROFA SOFT
    /// </summary>
    public static class ThemeManager
    {
        public enum ThemeType
        {
            MedicalBlue,
            DarkXRay,
            RofaPremium
        }

        private static ThemeType currentTheme = ThemeType.MedicalBlue;

        public static ThemeType CurrentTheme
        {
            get => currentTheme;
            set => currentTheme = value;
        }

        /// <summary>
        /// ألوان الثيم الطبي الأزرق
        /// </summary>
        public static class MedicalBlue
        {
            public static Color Primary = Color.FromArgb(0, 102, 204);      // أزرق طبي
            public static Color Secondary = Color.FromArgb(51, 153, 255);   // أزرق فاتح
            public static Color Background = Color.FromArgb(240, 244, 248); // خلفية رمادي فاتح
            public static Color Surface = Color.FromArgb(255, 255, 255);    // سطح أبيض
            public static Color Success = Color.FromArgb(40, 167, 69);      // أخضر
            public static Color Danger = Color.FromArgb(220, 53, 69);       // أحمر
            public static Color Warning = Color.FromArgb(255, 193, 7);      // أصفر
            public static Color Info = Color.FromArgb(23, 162, 184);        // أزرق فاتح
            public static Color Dark = Color.FromArgb(52, 58, 64);          // رمادي غامق
        }

        /// <summary>
        /// ألوان ثيم الأشعة السوداء
        /// </summary>
        public static class DarkXRay
        {
            public static Color Primary = Color.FromArgb(25, 25, 25);       // أسود عميق
            public static Color Secondary = Color.FromArgb(50, 50, 50);     // رمادي غامق
            public static Color Background = Color.FromArgb(15, 15, 15);    // أسود جداً
            public static Color Surface = Color.FromArgb(35, 35, 35);       // سطح رمادي غامق
            public static Color Success = Color.FromArgb(76, 175, 80);      // أخضر مضيء
            public static Color Danger = Color.FromArgb(244, 67, 54);       // أحمر مضيء
            public static Color Warning = Color.FromArgb(255, 152, 0);      // برتقالي مضيء
            public static Color Info = Color.FromArgb(33, 150, 243);        // أزرق مضيء
            public static Color Dark = Color.FromArgb(200, 200, 200);       // رمادي فاتح
        }

        /// <summary>
        /// ألوان ثيم ROFA البريميوم
        /// </summary>
        public static class RofaPremium
        {
            public static Color Primary = Color.FromArgb(102, 51, 153);     // بنفسجي ROFA
            public static Color Secondary = Color.FromArgb(204, 153, 255);  // بنفسجي فاتح
            public static Color Background = Color.FromArgb(250, 248, 252); // خلفية بنفسجية فاتحة جداً
            public static Color Surface = Color.FromArgb(255, 255, 255);    // سطح أبيض
            public static Color Success = Color.FromArgb(76, 175, 80);      // أخضر
            public static Color Danger = Color.FromArgb(244, 67, 54);       // أحمر
            public static Color Warning = Color.FromArgb(255, 193, 7);      // أصفر
            public static Color Info = Color.FromArgb(0, 188, 212);         // سماوي
            public static Color Dark = Color.FromArgb(33, 33, 66);          // رمادي غامق مع بنفسجي
        }

        /// <summary>
        /// تطبيق الثيم على النموذج وجميع التحكمات
        /// </summary>
        public static void ApplyTheme(Form form)
        {
            if (form == null) return;

            var colors = GetCurrentColors();

            form.BackColor = colors.Background;
            form.ForeColor = colors.Dark;

            ApplyThemeRecursive(form.Controls, colors);
            SaveThemePreference();
        }

        private static void ApplyThemeRecursive(Control.ControlCollection controls, ThemeColors colors)
        {
            foreach (Control control in controls)
            {
                ApplyThemeToControl(control, colors);
                if (control.HasChildren)
                    ApplyThemeRecursive(control.Controls, colors);
            }
        }

        private static void ApplyThemeToControl(Control control, ThemeColors colors)
        {
            control.BackColor = GetBackColorForControl(control, colors);
            control.ForeColor = colors.Dark;

            if (control is Button btn)
            {
                btn.BackColor = colors.Primary;
                btn.ForeColor = Color.White;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
            }
            else if (control is TextBox || control is ComboBox)
            {
                control.BackColor = colors.Surface;
            }
            else if (control is GroupBox)
            {
                control.BackColor = colors.Background;
            }
            else if (control is Panel)
            {
                control.BackColor = colors.Background;
            }
            else if (control is StatusStrip || control is MenuStrip)
            {
                control.BackColor = colors.Surface;
            }
        }

        private static Color GetBackColorForControl(Control control, ThemeColors colors)
        {
            return control switch
            {
                Button => colors.Primary,
                TextBox => colors.Surface,
                ComboBox => colors.Surface,
                GroupBox => colors.Background,
                Panel => colors.Background,
                StatusStrip => colors.Surface,
                MenuStrip => colors.Surface,
                _ => colors.Background
            };
        }

        public static ThemeColors GetCurrentColors()
        {
            return currentTheme switch
            {
                ThemeType.MedicalBlue => GetMedicalBlueColors(),
                ThemeType.DarkXRay => GetDarkXRayColors(),
                ThemeType.RofaPremium => GetRofaPremiumColors(),
                _ => GetMedicalBlueColors()
            };
        }

        private static ThemeColors GetMedicalBlueColors()
        {
            return new ThemeColors
            {
                Primary = MedicalBlue.Primary,
                Secondary = MedicalBlue.Secondary,
                Background = MedicalBlue.Background,
                Surface = MedicalBlue.Surface,
                Success = MedicalBlue.Success,
                Danger = MedicalBlue.Danger,
                Warning = MedicalBlue.Warning,
                Info = MedicalBlue.Info,
                Dark = MedicalBlue.Dark
            };
        }

        private static ThemeColors GetDarkXRayColors()
        {
            return new ThemeColors
            {
                Primary = DarkXRay.Primary,
                Secondary = DarkXRay.Secondary,
                Background = DarkXRay.Background,
                Surface = DarkXRay.Surface,
                Success = DarkXRay.Success,
                Danger = DarkXRay.Danger,
                Warning = DarkXRay.Warning,
                Info = DarkXRay.Info,
                Dark = DarkXRay.Dark
            };
        }

        private static ThemeColors GetRofaPremiumColors()
        {
            return new ThemeColors
            {
                Primary = RofaPremium.Primary,
                Secondary = RofaPremium.Secondary,
                Background = RofaPremium.Background,
                Surface = RofaPremium.Surface,
                Success = RofaPremium.Success,
                Danger = RofaPremium.Danger,
                Warning = RofaPremium.Warning,
                Info = RofaPremium.Info,
                Dark = RofaPremium.Dark
            };
        }

        private static void SaveThemePreference()
        {
            try
            {
                Properties.Settings.Default.CurrentTheme = currentTheme.ToString();
                Properties.Settings.Default.Save();
            }
            catch { /* تجاهل الأخطاء */ }
        }

        public static void LoadThemePreference()
        {
            try
            {
                string savedTheme = Properties.Settings.Default.CurrentTheme;
                if (!string.IsNullOrEmpty(savedTheme) && Enum.TryParse(savedTheme, out ThemeType theme))
                {
                    currentTheme = theme;
                }
            }
            catch { /* تجاهل الأخطاء */ }
        }
    }

    /// <summary>
    /// فئة لتخزين ألوان الثيم
    /// </summary>
    public class ThemeColors
    {
        public Color Primary { get; set; }
        public Color Secondary { get; set; }
        public Color Background { get; set; }
        public Color Surface { get; set; }
        public Color Success { get; set; }
        public Color Danger { get; set; }
        public Color Warning { get; set; }
        public Color Info { get; set; }
        public Color Dark { get; set; }
    }
}
