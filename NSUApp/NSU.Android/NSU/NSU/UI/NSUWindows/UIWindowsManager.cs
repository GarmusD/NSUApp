using Android.Content;
using NSU.Shared;
using NSU.Shared.NSUUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace NSU.Droid.UI.NSUWindows
{
    class UIWindowsManager : INSUWindowsManager
    {
        const string LogTag = "UIWindowsManager";
        Context context;
        List<UIWindowsGroup> groups;
        UIWindowsGroup current;
        float scale;

        public event EventHandler OnWindowsGroupChanged;
        public event EventHandler OnWindowChanged;

        public UIWindowsManager(Context cntx, float scale)
        {
            this.scale = scale;
            context = cntx;
            groups = new List<UIWindowsGroup>();
            current = null;
        }

        public INSUWindowsGroup this[int index]
        {
            get
            {
                return groups[index];
            }
        }

        public int Count
        {
            get
            {
                return groups.Count;
            }
        }

        public INSUWindowsGroup CurrentGroup
        {
            get
            {
                if (current == null)
                {
                    foreach (var item in groups)
                    {
                        if (item.IsDefault)
                        {
                            current = item;
                            break;
                        }
                    }
                    if (current == null && groups.Count > 0)
                        current = groups[0];
                }
                return current;
            }
        }

        public INSUWindowsGroup CreateGroup(string grpName)
        {
            var tmp = new UIWindowsGroup(context, grpName, this, scale);
            groups.Add(tmp);
            tmp.OnWindowChanged += OnWindowChangedHandler;            
            return tmp;
        }

        private void OnWindowChangedHandler(object sender, EventArgs e)
        {
            OnWindowChanged?.Invoke(sender, e);
        }

        public void DeleteGroup(string grpName)
        {
            throw new NotImplementedException();
        }

        public INSUWindowsGroup FindGroup(string grpName)
        {
            foreach(var item in groups)
            {
                if(item.Name.Equals(grpName))
                {
                    return item;
                }
            }
            return null;
        }

        public void ActivateWindowsGroup(string grpName)
        {
            //if (CurrentGroup != null && CurrentGroup.Name.Equals(grpName))
            //    return;
            foreach(var item in groups)
            {
                if(item.Name.Equals(grpName))
                {
                    current = item;
                    OnWindowsGroupChanged?.Invoke(item, new EventArgs());
                }
            }
        }

        public void Dispose()
        {
            NSULog.Debug(LogTag, "Dispose()");
            foreach(var item in groups)
            {
                item.Dispose();
            }
            groups.Clear();
            groups = null;
        }
    }
}
