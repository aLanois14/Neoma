using System;
using System.Collections.Generic;
using System.Text;

namespace Neoma.RazorClassLib.Views.Emails.RefuseCandidature
{
    public class RefuseCandidatureEmailViewModel
    {
        public string ProjetName { get; set; }
        public string Motif { get; set; }
        public string EmailUrl { get; set; }
        public RefuseCandidatureEmailViewModel(string projetName, string motif, string emailUrl)
        {
            ProjetName = projetName;
            Motif = motif;
            EmailUrl = emailUrl;
        }
    }
}
