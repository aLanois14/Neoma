using System;
using System.Collections.Generic;
using System.Text;

namespace Neoma.RazorClassLib.Views.Emails.AddSelection
{
    public class AddSelectionEmailViewModel
    {
        public string SelectEmailUrl { get; set; }
        public AddSelectionEmailViewModel(string selectEmailUrl)
        {
            SelectEmailUrl = selectEmailUrl;
        }
    }
}
