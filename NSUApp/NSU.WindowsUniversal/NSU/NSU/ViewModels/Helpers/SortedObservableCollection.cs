using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSU.NSU_UWP.ViewModels.Helpers
{
    public class SortedObservableCollection<T> : ObservableCollection<T>
    {
        private string memberName;
        private bool hasFixedItem = false;

        public SortedObservableCollection(string sortMemberName)
        {
            if (string.IsNullOrEmpty(sortMemberName))
                throw new ArgumentNullException();
            memberName = sortMemberName;
            var pi = typeof(T).GetType().GetProperty(memberName);
            if(pi == null)
            {
                throw new ArgumentNullException($"Property {sortMemberName} does not exist.");
            }
            else
            {
                if(pi.PropertyType != typeof(string))
                {
                    throw new Exception("Unsuported property type.");
                }
            }
        }

        public void SetFixedItem(T item)
        {
            if(item != null)
            {
                Insert(0, item);
                hasFixedItem = true;
            }
        }

        public new void Add(T item)
        {
            int pos = GetNewItemIndex(item);
            if (pos != -1)
                InsertItem(pos, item);
        }

        public T FindItemOrDefault(object comparison)
        {
            if(comparison != null)
            {
                string s;
                if (comparison.GetType() == typeof(string))
                    s = comparison as string;
                else
                    s = comparison.ToString();
                
                foreach(T item in this)
                {
                    string sx = item.GetType().GetProperty(memberName)?.GetValue(item) as string;
                    if (s.CompareTo(sx) == 0)
                        return item;
                }
            }
            return hasFixedItem ? this[0] : default(T);
        }

        private int GetNewItemIndex(T item)
        {
            string newTxt = item.GetType().GetProperty(memberName).GetValue(item)?.ToString();
            int i, start = hasFixedItem ? 1 : 0 ;
            for(i=start; i < Count; i++)
            {
                T x = this[i];
                if (x != null)
                {
                    string Txt = this[i].GetType().GetProperty(memberName).GetValue(this[i]).ToString();
                    if (Txt.CompareTo(newTxt) >= 0)
                    {
                        return i;
                    }
                }
            }
            return i;
        }
    }
}
