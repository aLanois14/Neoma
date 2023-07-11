using System;
using System.Collections.Generic;
using System.Text;

namespace Neoma.RazorClassLib.Views.Emails.LeaveProject
{
    public class LeaveProjectEmailViewModel
    {
        public string ProjetName { get; set; }
        public string CandidatName { get; set; }
        public string Motif { get; set; }
        public string EmailUrl { get; set; }
        public LeaveProjectEmailViewModel(string projetName, string candidatName, string motif, string emailUrl)
        {
            ProjetName = projetName;
            CandidatName = candidatName;
            Motif = motif;
            EmailUrl = emailUrl;
        }
    }
}
