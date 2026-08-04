#nullable disable
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

#if USE_OPENCV
using OpenCvSharp;
#endif

namespace EasyDent2
{
    public static class OpenCvImageProcessing
    {
        /// <summary>
        /// تطبيق فلاتر تحسين التباين التكيفي وإزالة التشويش الرقمي لإظهار قنوات جذور الأسنان بوضوح فائق.
        /// </summary>
        public static Bitmap ApplyClaheAndDenoise(Bitmap input, double clipLimit = 2.0, int tileGridSize = 8, int denoiseH = 10)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

#if USE_OPENCV
            // تحويل الـ Bitmap إلى كائن Mat الخاص بـ OpenCV عبر الذاكرة لتجنب مشاكل الصيغ
            using var mat = BitmapToMat(input);
            using var gray = new Mat();

            // تحويل الصورة إلى تدرج رمادي نقي (Grayscale)
            Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);

            // تهيئة وتطبيق فلتر CLAHE المطور لمعالجة بهتان الأشعة وتوضيح الفروقات الدقيقة
            using var clahe = Cv2.CreateCLAHE(clipLimit, new OpenCvSharp.Size(tileGridSize, tileGridSize));
            using var claheMat = new Mat();
            clahe.Apply(gray, claheMat);

            // تنظيف نمش وتشويش السحب الإلكتروني للحساس بدون تضييع حواف وتفاصيل السن
            using var denoised = new Mat();
            Cv2.FastNlMeansDenoising(claheMat, denoised, h: denoiseH, templateWindowSize: 7, searchWindowSize: 21);

            return MatToBitmap(denoised);
#else
            // إذا لم تكن المكتبة مفعلة، نرجع نسخة من الصورة الأصلية كآلية حماية للبرنامج
            return new Bitmap(input);
#endif
        }

        /// <summary>
        /// تطبيق التلوين المجسم والمتباين لصور الأشعة لمساعدة الطبيب في تحليل كثافة عظام الفك وقنوات الأعصاب.
        /// </summary>
        public static Bitmap ApplyColorMap(Bitmap input, string mapName)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

#if USE_OPENCV
            int colormap = -1;
            switch (mapName?.ToLowerInvariant())
            {
                case "jet": colormap = (int)ColormapTypes.Jet; break;
                case "hot": colormap = (int)ColormapTypes.Hot; break;
                case "rainbow": colormap = (int)ColormapTypes.Rainbow; break;
                case "viridis": colormap = (int)ColormapTypes.Viridis; break;
                case "bone": colormap = (int)ColormapTypes.Bone; break;
                default: colormap = -1; break; // لا يطبق أي خريطة (None)
            }

            using var mat = BitmapToMat(input);
            using var gray = new Mat();
            Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);

            using var colored = new Mat();
            if (colormap >= 0)
            {
                Cv2.ApplyColorMap(gray, colored, (ColormapTypes)colormap);
                return MatToBitmap(colored);
            }
            else
            {
                return MatToBitmap(gray);
            }
#else
            return new Bitmap(input);
#endif
        }

#if USE_OPENCV
        private static Mat BitmapToMat(Bitmap bmp)
        {
            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            var bytes = ms.ToArray();
            return Cv2.ImDecode(bytes, ImreadModes.Color);
        }

        private static Bitmap MatToBitmap(Mat mat)
        {
            Cv2.ImEncode(".png", mat, out byte[] bytes);
            using var ms = new MemoryStream(bytes);
            return new Bitmap(ms);
        }
#endif
    }
}
