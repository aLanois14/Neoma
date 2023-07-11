using System;
using System.Collections.Generic;
using System.Text;

namespace Neoma.RazorClassLib.Views.Emails.Postuler
{
    public class PostulerEmailViewModel
    {
        public string ProjetName { get; set; }
        public string CandidatName { get; set; }
        public string EmailUrl { get; set; }
        public PostulerEmailViewModel(string projetName, string candidatName, string emailUrl)
        {
            ProjetName = projetName;
            CandidatName = candidatName;
            EmailUrl = emailUrl;
        }
    }
}
