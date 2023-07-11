using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neoma.Extensions
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue)
        {
            if (selectedValue == -1)
            {
                return from item in items
                       select new SelectListItem
                       {
                           Text = item.GetPropertyValue("Name"),
                           Value = item.GetPropertyValue("Id")
                       };
            }
            else
            {
                return from item in items
                       select new SelectListItem
                       {
                           Text = item.GetPropertyValue("Name"),
                           Value = item.GetPropertyValue("Id"),
                           Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                       };
            }
        }
    }
}
