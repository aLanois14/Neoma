using System;
using System.Collections.Generic;
using System.Text;

namespace Neoma.RazorClassLib.Views.Emails.ResetPassword
{
    public class ResetPasswordViewModel
    {
        public ResetPasswordViewModel(string resetEmailUrl)
        {
            ResetEmailUrl = resetEmailUrl;
        }

        public string ResetEmailUrl { get; set; }
    }
}
