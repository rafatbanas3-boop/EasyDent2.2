using System.Drawing;

namespace EasyDent2
{
    internal class ChartArea
    {
        private string v;

        public ChartArea(string v)
        {
            this.v = v;
        }

        public Color BackColor { get; internal set; }
        public object AxisX { get; internal set; }
    }
}