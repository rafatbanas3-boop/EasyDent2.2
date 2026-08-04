#nullable disable
using System;
using System.Drawing;

namespace EasyDent2
{
    public static class ColorMaps
    {
        public static Color[] GetColorMap(string name)
        {
            Color[] colors = new Color[256];
            switch (name?.ToLowerInvariant())
            {
                case "bone":
                    // تدرج رمادي نقي ومحسن مخصص للأشعة السينية العظمية
                    for (int i = 0; i < 256; i++) colors[i] = Color.FromArgb(i, i, i);
                    break;
                case "jet":
                    for (int i = 0; i < 256; i++) colors[i] = InterpolateJet(i);
                    break;
                case "hot":
                    for (int i = 0; i < 256; i++) colors[i] = Color.FromArgb(Math.Min(255, i * 3), Math.Max(0, Math.Min(255, (i - 85) * 3)), Math.Max(0, Math.Min(255, (i - 170) * 3)));
                    break;
                case "rainbow":
                    for (int i = 0; i < 256; i++) colors[i] = ColorFromHsv(i * 240.0 / 255.0, 1.0, 1.0);
                    break;
                case "viridis":
                    for (int i = 0; i < 256; i++) colors[i] = InterpolateViridis(i);
                    break;
                default:
                    // الوضع الافتراضي التدرج الرمادي القياسي
                    for (int i = 0; i < 256; i++) colors[i] = Color.FromArgb(i, i, i);
                    break;
            }
            return colors;
        }

        private static Color InterpolateJet(int v)
        {
            double r = Math.Clamp(Math.Min(4 * (v / 255.0) - 1.5, -4 * (v / 255.0) + 4.5), 0.0, 1.0);
            double g = Math.Clamp(Math.Min(4 * (v / 255.0) - 0.5, -4 * (v / 255.0) + 3.5), 0.0, 1.0);
            double b = Math.Clamp(Math.Min(4 * (v / 255.0) + 0.5, -4 * (v / 255.0) + 2.5), 0.0, 1.0);
            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }

        private static Color InterpolateViridis(int v)
        {
            double f = v / 255.0;
            int r = (int)((0.267 + 0.7 * f) * 255);
            int g = (int)((0.004 + 0.8 * f) * 255);
            int b = (int)((0.329 + 0.4 * f) * 255);
            return Color.FromArgb(Math.Clamp(r, 0, 255), Math.Clamp(g, 0, 255), Math.Clamp(b, 0, 255));
        }

        private static Color ColorFromHsv(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);
            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0) return Color.FromArgb(255, v, t, p);
            else if (hi == 1) return Color.FromArgb(255, q, v, p);
            else if (hi == 2) return Color.FromArgb(255, p, v, t);
            else if (hi == 3) return Color.FromArgb(255, p, q, v);
            else if (hi == 4) return Color.FromArgb(255, t, p, v);
            else return Color.FromArgb(255, v, p, q);
        }
    }
}
