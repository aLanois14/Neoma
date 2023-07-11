using System;
using System.Collections.Generic;
using System.Text;

namespace Neoma.RazorClassLib.Views.Emails.ValidateSelection
{
    public class ValidateSelectionEmailViewModel
    {
        public string ProjetName { get; set; }
        public string FondateurName { get; set; }
        public string EmailUrl { get; set; }
        public ValidateSelectionEmailViewModel(string projetName, string fondateurName, string emailUrl)
        {
            FondateurName = fondateurName;
            ProjetName = projetName;
            EmailUrl = emailUrl;
        }
    }
}
