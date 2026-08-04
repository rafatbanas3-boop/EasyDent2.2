#nullable disable
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyDent2
{
    public class CountdownForm : Form
    {
        private Label lbl;
        private Button btnCancel;
        private int seconds;
        private CancellationTokenSource cts;

        public CountdownForm(int seconds = 60)
        {
            this.seconds = seconds;
            Width = 380; Height = 150;
            StartPosition = FormStartPosition.CenterParent;
            Text = "مؤقت التقاط الأشعة";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            BackColor = Color.WhiteSmoke;

            lbl = new Label
            {
                Text = $"سيبدأ الالتقاط بعد {seconds} ثانية...",
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.DarkSlateGray
            };

            btnCancel = new Button
            {
                Text = "إلغاء الأمر",
                Dock = DockStyle.Bottom,
                Height = 40,
                DialogResult = DialogResult.Cancel,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Controls.Add(lbl);
            Controls.Add(btnCancel);
            Shown += CountdownForm_Shown;
        }

        private async void CountdownForm_Shown(object sender, EventArgs e)
        {
            cts = new CancellationTokenSource();
            try
            {
                for (int i = seconds; i >= 0; i--)
                {
                    lbl.Text = $"سيبدأ الالتقاط التلقائي بعد {i} ثانية...";

                    // تغيير لون النص تدريجياً للتحذير مع اقتراب الصفر
                    if (i <= 5) lbl.ForeColor = Color.Red;

                    await Task.Delay(1000, cts.Token);
                }
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (OperationCanceledException)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}
