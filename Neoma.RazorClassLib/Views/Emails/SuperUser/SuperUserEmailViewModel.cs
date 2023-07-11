using System;
using System.Collections.Generic;
using System.Text;

namespace Neoma.RazorClassLib.Views.Emails.SuperUser
{
    public class SuperUserEmailViewModel
    {
        public string SuperUserEmailUrl { get; set; }

        public SuperUserEmailViewModel(string superUserEmailUrl)
        {
            SuperUserEmailUrl = superUserEmailUrl;
        }
    }
}
