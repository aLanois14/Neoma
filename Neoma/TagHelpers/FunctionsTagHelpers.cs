using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Neoma.Models;

namespace Neoma.TagHelpers
{
    public class FunctionsTagHelpers : TagHelper
    {
        public static string formatRoleSpec(int role, List<Specialite> specialite)
        {
            List<Specialite> list = new List<Specialite>();
            string text = "";
            foreach (var spec in specialite)
            {
                if (spec.RoleId == role)
                {
                    list.Add(spec);
                }
            }

            if (list.Count > 0)
            {
                Specialite last = list.Last();
                foreach (var special in list)
                {
                    text += !special.Equals(last) ? special.Name + " - " : special.Name;
                }
            }
            return text;
        }
    }
}
