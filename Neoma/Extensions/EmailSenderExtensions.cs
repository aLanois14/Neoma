using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Neoma.Services
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirmez votre email", link);
        }

        public static Task SendEmailResetPasswordAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Changement de mot de passe",link);
        }

        public static Task SendNewEmail(this IEmailSender emailSender, string email, string title, string message)
        {
            return emailSender.SendEmailAsync(email, title, message);
        }

        public static Task SendEmailSuperUser(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Compte Super Utilisateur",
                $"Un compte super utilisateur a été créé pour vous. Veuillez cliquer sur ce <a href='{HtmlEncoder.Default.Encode(link)}'>lien</a> pour créer votre mot de passe.");
        }
    }
}
