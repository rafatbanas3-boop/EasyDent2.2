#nullable disable
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using EasyDent2.Properties; // استدعاء كلاس الإعدادات المطور

namespace EasyDent2
{
    public class SettingsForm : Form
    {
        private Label lblPath;
        private Button btnChoose;
        private Button btnClear;
        private CheckBox chkCopyToSensors;

        public SettingsForm()
        {
            InitializeComponents();
            LoadCurrentSetting();
        }

        private void InitializeComponents()
        {
            Text = "إعدادات نظام الأشعة المعايرة";
            Width = 600; Height = 180;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false; MinimizeBox = false;

            lblPath = new Label { Left = 12, Top = 16, Width = 560, Height = 35, Text = "ملف المعايرة النشط: (لا يوجد)" };

            btnChoose = new Button { Text = "اختر ملف المعايرة...", Left = 12, Top = 60, Width = 180, FlatStyle = FlatStyle.Flat, BackColor = Color.Gainsboro };
            btnChoose.Click += BtnChoose_Click;

            btnClear = new Button { Text = "مسح الإعداد الحالي", Left = 200, Top = 60, Width = 140, FlatStyle = FlatStyle.Flat, BackColor = Color.Gainsboro };
            btnClear.Click += BtnClear_Click;

            chkCopyToSensors = new CheckBox { Text = "انسخ الملف إلى مجلد التطبيق الداخلي (Sensors)", Left = 12, Top = 110, Width = 360 };
            chkCopyToSensors.Checked = true;

            Controls.Add(lblPath);
            Controls.Add(btnChoose);
            Controls.Add(btnClear);
            Controls.Add(chkCopyToSensors);
        }

        private void LoadCurrentSetting()
        {
            var path = AppSettings.Instance.CalibrationFilePath;
            lblPath.Text = string.IsNullOrWhiteSpace(path) ? "ملف المعايرة النشط: (لا يوجد)" : "ملف المعايرة النشط: " + path;
        }

        private void BtnChoose_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog();
            ofd.Filter = "ملفات المعايرة والصور|*.cor;*.json;*.csv;*.txt;*.png;*.jpg;*.bmp|كل الملفات|*.*";
            if (ofd.ShowDialog(this) != DialogResult.OK) return;

            string selected = ofd.FileName;
            try
            {
                if (chkCopyToSensors.Checked)
                {
                    var sensorsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sensors");
                    Directory.CreateDirectory(sensorsFolder);
                    var dest = Path.Combine(sensorsFolder, Path.GetFileName(selected));
                    File.Copy(selected, dest, true);
                    AppSettings.Instance.CalibrationFilePath = dest;
                }
                else
                {
                    AppSettings.Instance.CalibrationFilePath = selected;
                }

                AppSettings.Instance.Save();
                LoadCurrentSetting();
                MessageBox.Show("تم حفظ وإعداد ملف المعايرة للبرنامج بنجاح.", "نجح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ أثناء حفظ ملف المعايرة: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            AppSettings.Instance.CalibrationFilePath = string.Empty;
            AppSettings.Instance.Save();
            LoadCurrentSetting();
            MessageBox.Show("تم مسح ملف المعايرة المحفوظ من ذاكرة النظام.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
