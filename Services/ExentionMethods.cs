using System.Collections.Generic;

namespace DeskPad.Services
{
    public static class ExentionMethods
    {
        public static void Replace<T>(this List<T> list, T oldItem, T newItem)
        {
            var oldItemIndex = list.IndexOf(oldItem);
            list[oldItemIndex] = newItem;
        }
    }
}
