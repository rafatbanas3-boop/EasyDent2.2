#nullable disable
using System;
using System.IO;
using System.Windows.Forms;

namespace EasyDent2
{
    /// <summary>
    /// فئة إدارة الإعدادات - AppSettings
    /// تخزين واسترجاع إعدادات البرنامج
    /// </summary>
    public class AppSettings
    {
        private static AppSettings _instance;
        private string _calibrationFilePath = "";
        private string _lastPatientName = "";
        private int _defaultZoom = 100;

        public static AppSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AppSettings();
                return _instance;
            }
        }

        public string CalibrationFilePath
        {
            get => _calibrationFilePath;
            set => _calibrationFilePath = value;
        }

        public string LastPatientName
        {
            get => _lastPatientName;
            set => _lastPatientName = value;
        }

        public int DefaultZoom
        {
            get => _defaultZoom;
            set => _defaultZoom = Math.Max(50, Math.Min(200, value));
        }

        /// <summary>
        /// حفظ الإعدادات في الملف
        /// </summary>
        public void Save()
        {
            try
            {
                Properties.Settings.Default.CalibrationFilePath = _calibrationFilePath;
                Properties.Settings.Default.LastPatientName = _lastPatientName;
                Properties.Settings.Default.DefaultZoom = _defaultZoom;
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حفظ الإعدادات: {ex.Message}");
            }
        }

        /// <summary>
        /// تحميل الإعدادات من الملف
        /// </summary>
        public void Load()
        {
            try
            {
                _calibrationFilePath = Properties.Settings.Default.CalibrationFilePath ?? "";
                _lastPatientName = Properties.Settings.Default.LastPatientName ?? "";
                _defaultZoom = Properties.Settings.Default.DefaultZoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الإعدادات: {ex.Message}");
            }
        }

        /// <summary>
        /// إعادة تعيين الإعدادات إلى القيم الافتراضية
        /// </summary>
        public void Reset()
        {
            _calibrationFilePath = "";
            _lastPatientName = "";
            _defaultZoom = 100;
            Save();
        }
    }
}
