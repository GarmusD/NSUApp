using System;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Runtime;
using NSU.Shared.NSUUI;
using NSU.Droid.UI.Elements;
using System.Collections.Generic;
using NSU.NSUSystem;
using NSU.Shared;

namespace NSU.Droid
{
    public class SideWindow : LinearLayout
    {
        readonly string logTag = "SideWindow";
        private float scale = 1.0f;
        private bool disposed;

        public SideWindow(IntPtr a, JniHandleOwnership b)
            : base(a, b)
        {
        }

        public SideWindow(Context context)
            : base(context)
        {
        }

        public SideWindow(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public SideWindow(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            NSULog.Debug(logTag, "Dispose() is called");
            if (!disposed)
            {
                disposed = true;
            }
            RemoveAllViews();
        }

        void SetMargins(ImageView i)
        {
            i.SetPadding(0, 4, 0, 0);
            return;
        }
        public void AttachNSUSideWindow(INSUSideWindow wnd)
        {
            RemoveAllViews();
            if (wnd == null) return;
            scale = 1.0f;
            for (int i = 0; i < wnd.Count; i++)
            {
                var item = wnd[i];
                switch (item.UIClass)
                {
                    case NSUUISideElementClass.Switch:
                        var swt = item as UISideSwitch;                        
                        AddView(swt);
                        SetMargins(swt);
                        break;
                    case NSUUISideElementClass.WindowSwitch:
                        var wc = item as UISideWindowSwitch;                        
                        AddView(wc);
                        SetMargins(wc);
                        break;
                    case NSUUISideElementClass.GroupSwitch:
                        var gs = item as UISideGroupSwitch;                        
                        AddView(gs);
                        SetMargins(gs);
                        break;                    
                }
            }
        }

    }
}

