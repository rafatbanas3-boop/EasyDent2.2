#nullable disable
using System;
using System.Collections.Generic;

namespace EasyDent2
{
    public class CalibrationInfo
    {
        public DateTime Date { get; set; }
        public double PixelsPerMm { get; set; }
        public string Method { get; set; }
        public string Note { get; set; }
    }

    public class SensorProfile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public double DefaultPixelsPerMm { get; set; }
        public double? SensorWidthMm { get; set; }
        public double? SensorHeightMm { get; set; }
        public CalibrationInfo LastCalibration { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }
}
