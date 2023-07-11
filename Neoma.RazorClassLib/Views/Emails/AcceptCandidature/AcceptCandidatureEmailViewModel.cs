using System;
using System.Collections.Generic;
using System.Text;

namespace Neoma.RazorClassLib.Views.Emails.AcceptCandidature
{
    public class AcceptCandidatureEmailViewModel
    {
        public string ProjetName { get; set; }
        public string EmailUrl { get; set; }
        public AcceptCandidatureEmailViewModel(string projetName, string emailUrl)
        {
            ProjetName = projetName;
            EmailUrl = emailUrl;
        }
    }
}
