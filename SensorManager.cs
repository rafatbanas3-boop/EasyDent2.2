#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EasyDent2
{
    public class SensorManager
    {
        public List<SensorProfile> Profiles { get; } = new List<SensorProfile>();

        private static string ProfilesPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sensors", "profiles.json");

        public SensorManager()
        {
            LoadProfiles();
            EnsureCorFilesLoaded();
        }

        public void LoadProfiles()
        {
            try
            {
                var folder = Path.GetDirectoryName(ProfilesPath);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                if (!File.Exists(ProfilesPath)) { SaveProfiles(); return; }

                var txt = File.ReadAllText(ProfilesPath);
                var arr = JsonSerializer.Deserialize<List<SensorProfile>>(txt);
                Profiles.Clear();
                if (arr != null) Profiles.AddRange(arr);
            }
            catch { }
        }

        public void SaveProfiles()
        {
            try
            {
                var folder = Path.GetDirectoryName(ProfilesPath);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                var txt = JsonSerializer.Serialize(Profiles, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(ProfilesPath, txt);
            }
            catch { }
        }

        public void AddOrUpdateProfile(SensorProfile p)
        {
            var existing = Profiles.FirstOrDefault(x => x.Id == p.Id);
            if (existing != null) Profiles.Remove(existing);
            Profiles.Add(p);
        }

        public bool ImportCalibrationFile(string filepath, out SensorProfile createdProfile, IWin32Window owner = null)
        {
            createdProfile = null;
            if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath)) return false;
            var ext = Path.GetExtension(filepath).ToLowerInvariant();

            try
            {
                if (ext == ".cor")
                {
                    var sensorsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sensors");
                    Directory.CreateDirectory(sensorsFolder);
                    var destFile = Path.Combine(sensorsFolder, Path.GetFileName(filepath));
                    File.Copy(filepath, destFile, true);

                    var baseName = Path.GetFileNameWithoutExtension(filepath);
                    string id = baseName;
                    try
                    {
                        var bytes = File.ReadAllBytes(filepath);
                        var text = System.Text.Encoding.ASCII.GetString(bytes);
                        var m = Regex.Match(text, @"\d{5,}");
                        if (m.Success) id = m.Value;
                    }
                    catch { }

                    var profile = new SensorProfile
                    {
                        Id = id,
                        Name = baseName + " (.cor imported)",
                        DefaultPixelsPerMm = 22.5, // قيمة افتراضية نموذجية لملفات التصحيح
                        LastCalibration = new CalibrationInfo { Date = DateTime.UtcNow, PixelsPerMm = 22.5, Method = "import-cor", Note = $"file={Path.GetFileName(destFile)}" },
                        Images = new List<string> { Path.GetFileName(destFile) }
                    };

                    AddOrUpdateProfile(profile);
                    SaveProfiles();
                    createdProfile = profile;
                    return true;
                }
                else if (ext == ".json")
                {
                    var txt = File.ReadAllText(filepath);
                    try
                    {
                        var sp = JsonSerializer.Deserialize<SensorProfile>(txt);
                        if (sp != null)
                        {
                            if (string.IsNullOrEmpty(sp.Id)) sp.Id = Path.GetFileNameWithoutExtension(filepath);
                            AddOrUpdateProfile(sp);
                            SaveProfiles();
                            createdProfile = sp;
                            return true;
                        }
                    }
                    catch { }

                    try
                    {
                        using var doc = JsonDocument.Parse(txt);
                        if (doc.RootElement.TryGetProperty("pixelsPerMm", out var p))
                        {
                            double ppm = p.GetDouble();
                            var profile = new SensorProfile
                            {
                                Id = Path.GetFileNameWithoutExtension(filepath),
                                Name = "معايرة مستوردة JSON",
                                DefaultPixelsPerMm = ppm,
                                LastCalibration = new CalibrationInfo { Date = DateTime.UtcNow, PixelsPerMm = ppm, Method = "import-json", Note = filepath }
                            };
                            AddOrUpdateProfile(profile);
                            createdProfile = profile;
                            SaveProfiles();
                            return true;
                        }
                    }
                    catch { }
                    return false;
                }
                else if (ext == ".csv" || ext == ".txt")
                {
                    var lines = File.ReadAllLines(filepath).Where(l => !string.IsNullOrWhiteSpace(l)).ToArray();
                    foreach (var ln in lines)
                    {
                        var parts = ln.Split(new[] { '=', ',', ';' }, 2);
                        if (parts.Length >= 2)
                        {
                            var key = parts[0].Trim().ToLowerInvariant();
                            var val = parts[1].Trim();
                            if (key.Contains("pixels") || key.Contains("ppm") || key.Contains("scale"))
                            {
                                if (double.TryParse(val, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double ppm))
                                {
                                    var profile = new SensorProfile
                                    {
                                        Id = Path.GetFileNameWithoutExtension(filepath),
                                        Name = "معايرة مستوردة نصية",
                                        DefaultPixelsPerMm = ppm,
                                        LastCalibration = new CalibrationInfo { Date = DateTime.UtcNow, PixelsPerMm = ppm, Method = "import-csv", Note = filepath }
                                    };
                                    AddOrUpdateProfile(profile);
                                    SaveProfiles();
                                    createdProfile = profile;
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                }
                else if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp")
                {
                    using var cf = new CalibrationForm(filepath);
                    var dr = (owner != null) ? cf.ShowDialog(owner) : cf.ShowDialog();
                    if (dr == DialogResult.OK && cf.PixelsPerMm > 0)
                    {
                        var ppm = cf.PixelsPerMm;
                        var profile = new SensorProfile
                        {
                            Id = Path.GetFileNameWithoutExtension(filepath),
                            Name = "معايرة يدوية من صورة",
                            DefaultPixelsPerMm = ppm,
                            LastCalibration = new CalibrationInfo { Date = DateTime.UtcNow, PixelsPerMm = ppm, Method = "import-image", Note = filepath }
                        };
                        AddOrUpdateProfile(profile);
                        SaveProfiles();
                        createdProfile = profile;
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public SensorProfile TryAutoApplyCalibrationFromImage(System.Drawing.Image img, string hint, IWin32Window owner)
        {
            if (Profiles.Count > 0) return Profiles.First();

            var defaultProfile = new SensorProfile
            {
                Id = "Default_Dental_Sensor",
                Name = "بروفايل الحساس التلقائي",
                DefaultPixelsPerMm = 20.0 // مقياس بكسل تقريبي افتراضي لعيادات الأسنان
            };
            AddOrUpdateProfile(defaultProfile);
            SaveProfiles();
            return defaultProfile;
        }

        private void EnsureCorFilesLoaded()
        {
            try
            {
                var sensorsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sensors");
                if (Directory.Exists(sensorsFolder))
                {
                    var files = Directory.GetFiles(sensorsFolder, "*.cor");
                    foreach (var file in files)
                    {
                        var id = Path.GetFileNameWithoutExtension(file);
                        if (!Profiles.Any(x => x.Id == id))
                        {
                            Profiles.Add(new SensorProfile { Id = id, Name = id + " (ملف تصحيح متاح)" });
                        }
                    }
                }
            }
            catch { }
        }
    }
}
