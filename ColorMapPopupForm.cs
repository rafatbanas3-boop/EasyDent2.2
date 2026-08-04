#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace EasyDent2
{
    /// <summary>
    /// نموذج اختيار خريطة الألوان (Color Map Popup)
    /// للمعالجة المتقدمة للأشعة السينية
    /// </summary>
    public class ColorMapPopupForm : Form
    {
        private string selectedMap = "None";
        private Button btnApply;
        private Button btnCancel;
        private ListBox listBoxMaps;

        public string SelectedMap => selectedMap;

        public ColorMapPopupForm(string currentMap = "None")
        {
            selectedMap = currentMap;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 400);
            this.Text = "اختيار خريطة الألوان";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 244, 248);

            // قائمة خرائط الألوان
            var lblMaps = new Label
            {
                Text = "اختر خريطة الألوان المرغوبة:",
                Location = new Point(15, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 66)
            };
            this.Controls.Add(lblMaps);

            listBoxMaps = new ListBox
            {
                Location = new Point(15, 40),
                Size = new Size(370, 250),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // إضافة خرائط الألوان المتاحة
            string[] colorMaps = new[]
            {
                "None",
                "Autumn",
                "Bone",
                "Cool",
                "Hot",
                "HSV",
                "Jet",
                "Ocean",
                "Pink",
                "Rainbow",
                "Spring",
                "Summer",
                "Twilight",
                "Viridis",
                "Winter"
            };

            listBoxMaps.Items.AddRange(colorMaps);
            listBoxMaps.SelectedItem = selectedMap;
            listBoxMaps.DoubleClick += (s, e) => ApplySelection();

            this.Controls.Add(listBoxMaps);

            // زر التطبيق
            btnApply = new Button
            {
                Text = "تطبيق",
                Location = new Point(200, 305),
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Click += (s, e) => ApplySelection();
            this.Controls.Add(btnApply);

            // زر الإلغاء
            btnCancel = new Button
            {
                Text = "إلغاء",
                Location = new Point(295, 305),
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);
        }

        private void ApplySelection()
        {
            if (listBoxMaps.SelectedItem != null)
            {
                selectedMap = listBoxMaps.SelectedItem.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
