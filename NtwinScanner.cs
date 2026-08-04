#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NTwain;
using NTwain.Data;

namespace EasyDent2
{
    public class NtwinScanner : IScannerSession
    {
        public event EventHandler<Image> ImageAcquired;
        public event EventHandler<string> InfoMessage;

        private TwainSession _twainSession;
        private bool _isOpen;
        private string _selectedSourceName;

        public NtwinScanner()
        {
            SetupTwainSession();
        }

        private void SetupTwainSession()
        {
            try
            {
                // إنشاء الجلسة الصحيح والمطابق تماماً لمتطلبات إصدار 3.7.6 لديك
                _twainSession = new TwainSession(DataGroups.Image);

                _twainSession.TransferReady += (s, e) => InfoMessage?.Invoke(this, "تم التقاط إشارة الأشعة، جاري نقل داتا الصورة...");
                _twainSession.DataTransferred += TwainSession_DataTransferred;
                _twainSession.StateChanged += (s, e) => InfoMessage?.Invoke(this, $"حالة TWAIN الحالية: {_twainSession.State}");
            }
            catch (Exception ex)
            {
                _twainSession = null;
                InfoMessage?.Invoke(this, "فشل تهيئة موديول TWAIN: " + ex.Message);
            }
        }

        public bool Open()
        {
            if (_isOpen) return true;
            if (_twainSession == null) return false;

            try
            {
                // فتح مدير الأجهزة DSM
                _twainSession.Open();
                _isOpen = _twainSession.IsDsmOpen;

                if (_isOpen)
                {
                    InfoMessage?.Invoke(this, "تم الاتصال بمدير أجهزة الأشعة TWAIN بنجاح.");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                InfoMessage?.Invoke(this, "خطأ أثناء محاولة فتح موديول الأشعة: " + ex.Message);
                return false;
            }
        }

        public IEnumerable<string> GetSourceNames()
        {
            if (!_isOpen && !Open()) return Enumerable.Empty<string>();
            try
            {
                return _twainSession.GetSources().Select(s => s.Name).ToList();
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        public bool SelectSource(string name)
        {
            if (!_isOpen && !Open()) return false;
            try
            {
                var source = _twainSession.GetSources().FirstOrDefault(s => s.Name == name);
                if (source != null)
                {
                    _selectedSourceName = name;
                    InfoMessage?.Invoke(this, $"تم اختيار السينسور بنجاح: {name}");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                InfoMessage?.Invoke(this, "خطأ أثناء اختيار جهاز الأشعة: " + ex.Message);
                return false;
            }
        }

        public bool Acquire()
        {
            if (!_isOpen && !Open()) return false;
            try
            {
                var source = _twainSession.GetSources().FirstOrDefault(s => s.Name == _selectedSourceName) ?? _twainSession.GetSources().FirstOrDefault();
                if (source == null)
                {
                    InfoMessage?.Invoke(this, "لم يتم العثور على أي مستشعر متصل.");
                    return false;
                }

                // فتح مصدر السحب مباشرة أو التحقق الذكي عبر الجلسة منعاً لأي خطأ Compile
                if (!_twainSession.IsSourceOpen)
                {
                    source.Open();
                }

                source.Enable(SourceEnableMode.ShowUI, false, IntPtr.Zero);
                return true;
            }
            catch (Exception ex)
            {
                InfoMessage?.Invoke(this, "خطأ برمي أثناء طلب التقاط الأشعة: " + ex.Message);
                return false;
            }
        }

        private void TwainSession_DataTransferred(object sender, DataTransferredEventArgs e)
        {
            try
            {
                if (e.NativeData != IntPtr.Zero)
                {
                    using var stream = e.GetNativeImageStream();
                    if (stream != null)
                    {
                        var img = Image.FromStream(stream);
                        ImageAcquired?.Invoke(this, new Bitmap(img));
                        img.Dispose();
                        InfoMessage?.Invoke(this, "تم استلام ومعالجة صورة الأشعة بنجاح.");
                    }
                }
            }
            catch (Exception ex)
            {
                InfoMessage?.Invoke(this, "خطأ أثناء تحويل بيانات الحساس: " + ex.Message);
            }
        }

        public void Close()
        {
            try
            {
                if (_twainSession != null)
                {
                    if (_twainSession.CurrentSource != null) _twainSession.CurrentSource.Close();
                    _twainSession.Close();
                }
                _isOpen = false;
            }
            catch { }
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }
    }
}
