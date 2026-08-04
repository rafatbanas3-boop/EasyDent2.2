#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace EasyDent2
{
    /// <summary>
    /// تحسينات متقدمة على مدير الثيمات
    /// Enhanced Theme Manager v3.1 - مع دعم تأثيرات بصرية متقدمة
    /// Powered by ROFA SOFT
    /// </summary>
    public static class ThemeManagerEnhanced
    {
        public enum ThemeType
        {
            MedicalBlue,
            DarkXRay,
            RofaPremium,
            ModernDental
        }

        private static ThemeType currentTheme = ThemeType.DarkXRay; // الثيم الافتراضي الأسود

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
            public static Color Primary = Color.FromArgb(0, 102, 204);
            public static Color Secondary = Color.FromArgb(51, 153, 255);
            public static Color Background = Color.FromArgb(240, 244, 248);
            public static Color Surface = Color.FromArgb(255, 255, 255);
            public static Color Success = Color.FromArgb(40, 167, 69);
            public static Color Danger = Color.FromArgb(220, 53, 69);
            public static Color Warning = Color.FromArgb(255, 193, 7);
            public static Color Info = Color.FromArgb(23, 162, 184);
            public static Color Dark = Color.FromArgb(52, 58, 64);
        }

        /// <summary>
        /// ألوان ثيم الأشعة السوداء (الافتراضي)
        /// </summary>
        public static class DarkXRay
        {
            public static Color Primary = Color.FromArgb(25, 25, 25);
            public static Color Secondary = Color.FromArgb(50, 50, 50);
            public static Color Background = Color.FromArgb(15, 15, 18);
            public static Color Surface = Color.FromArgb(35, 35, 40);
            public static Color Success = Color.FromArgb(76, 175, 80);
            public static Color Danger = Color.FromArgb(244, 67, 54);
            public static Color Warning = Color.FromArgb(255, 152, 0);
            public static Color Info = Color.FromArgb(33, 150, 243);
            public static Color Dark = Color.FromArgb(200, 200, 200);
        }

        /// <summary>
        /// ألوان ثيم ROFA البريميوم
        /// </summary>
        public static class RofaPremium
        {
            public static Color Primary = Color.FromArgb(102, 51, 153);
            public static Color Secondary = Color.FromArgb(204, 153, 255);
            public static Color Background = Color.FromArgb(250, 248, 252);
            public static Color Surface = Color.FromArgb(255, 255, 255);
            public static Color Success = Color.FromArgb(76, 175, 80);
            public static Color Danger = Color.FromArgb(244, 67, 54);
            public static Color Warning = Color.FromArgb(255, 193, 7);
            public static Color Info = Color.FromArgb(0, 188, 212);
            public static Color Dark = Color.FromArgb(33, 33, 66);
        }

        /// <summary>
        /// ثيم حديث متقدم
        /// </summary>
        public static class ModernDental
        {
            public static Color Primary = Color.FromArgb(63, 81, 181);
            public static Color Secondary = Color.FromArgb(103, 58, 183);
            public static Color Background = Color.FromArgb(240, 240, 245);
            public static Color Surface = Color.FromArgb(255, 255, 255);
            public static Color Success = Color.FromArgb(76, 175, 80);
            public static Color Danger = Color.FromArgb(244, 67, 54);
            public static Color Warning = Color.FromArgb(255, 193, 7);
            public static Color Info = Color.FromArgb(3, 155, 229);
            public static Color Dark = Color.FromArgb(66, 66, 66);
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
            else if (control is Guna2Button gBtn)
            {
                gBtn.FillColor = colors.Primary;
                gBtn.ForeColor = Color.White;
            }
            else if (control is TextBox || control is ComboBox)
            {
                control.BackColor = colors.Surface;
                control.ForeColor = colors.Dark;
            }
            else if (control is GroupBox)
            {
                control.BackColor = colors.Background;
                control.ForeColor = colors.Dark;
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
                Guna2Button => colors.Primary,
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
                ThemeType.ModernDental => GetModernDentalColors(),
                _ => GetDarkXRayColors()
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

        private static ThemeColors GetModernDentalColors()
        {
            return new ThemeColors
            {
                Primary = ModernDental.Primary,
                Secondary = ModernDental.Secondary,
                Background = ModernDental.Background,
                Surface = ModernDental.Surface,
                Success = ModernDental.Success,
                Danger = ModernDental.Danger,
                Warning = ModernDental.Warning,
                Info = ModernDental.Info,
                Dark = ModernDental.Dark
            };
        }

        private static void SaveThemePreference()
        {
            try
            {
                Properties.Settings.Default.CurrentTheme = currentTheme.ToString();
                Properties.Settings.Default.Save();
            }
            catch { }
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
            catch { }
        }

        /// <summary>
        /// تبديل الثيم الحالي
        /// </summary>
        public static void CycleTheme()
        {
            currentTheme = (ThemeType)(((int)currentTheme + 1) % 4);
            SaveThemePreference();
        }

        /// <summary>
        /// تعيين ثيم محدد
        /// </summary>
        public static void SetTheme(ThemeType theme)
        {
            currentTheme = theme;
            SaveThemePreference();
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
