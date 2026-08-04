#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;

namespace EasyDent2
{
    public interface IScannerSession : IDisposable
    {
        // الحدث الذي يشتعل تلقائياً فور نجاح سحب الصورة أو التقاطها من الأشعة
        event EventHandler<Image> ImageAcquired;

        // الحدث المسؤول عن إرسال تقارير الحالة ورسائل اختبار التوصيل للواجهة الرئيسية
        event EventHandler<string> InfoMessage;

        // دالة لتهيئة وبدء تشغيل نظام السحب
        bool Open();

        // دالة لجلب أسماء كافة تعريفات السينسور المتصلة بالجهاز حالياً (TWAIN Drivers)
        IEnumerable<string> GetSourceNames();

        // دالة لاختيار سينسور معين من القائمة للاتصال به
        bool SelectSource(string name);

        // دالة لبدء عملية الالتقاط الفعلي (Acquire) من الجهاز المختار
        bool Acquire();

        // دالة لإغلاق الاتصال بالحساس وتحرير كائناته
        void Close();
    }
}
