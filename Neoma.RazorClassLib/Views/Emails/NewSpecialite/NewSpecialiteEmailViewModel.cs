using System;
using System.Collections.Generic;
using System.Text;

namespace Neoma.RazorClassLib.Views.Emails.NewSpecialite
{
    public class NewSpecialiteEmailViewModel
    {
        public string NewSpecialiteEmailUrl { get; set; }
        public List<string> Specialites { get; set; }

        public NewSpecialiteEmailViewModel(string newSpecialiteEmailUrl, List<string> specialites)
        {
            NewSpecialiteEmailUrl = newSpecialiteEmailUrl;
            Specialites = specialites;
        }
    }
}
