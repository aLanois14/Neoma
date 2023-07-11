using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.Net.Mime;

namespace Neoma.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public EmailSender(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {

            SmtpClient client = new SmtpClient(_configuration["Email:Host"])
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_configuration["Email:User"], _configuration["Email:Password"]),
                EnableSsl = true,
                Port = 587
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:User"], _configuration["Email:UserName"])
            };
            string webRootPath = _hostingEnvironment.WebRootPath;
            string pathImage = Path.Combine(webRootPath, "images");
            LinkedResource LinkedImage = new LinkedResource(pathImage + @"\logo.png");
            LinkedImage.ContentId = "Logo";
            LinkedImage.ContentType.MediaType = "image/png";
            ContentType mimeType = new ContentType("text/html");
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message, mimeType);
            htmlView.LinkedResources.Add(LinkedImage);
            mailMessage.AlternateViews.Add(htmlView);
            mailMessage.To.Add(email);
            //mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = subject;

            try
            {
                client.Send(mailMessage);
            }
            catch(Exception ex)
            {
                string messageerreur = ex.Message;
            }
            return Task.CompletedTask;
        }

        public void SendEmailWithAttachement(string from, string to, string subject, string body, FileContentResult attachment)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient(_configuration["Email:Host"]);


            mail.From = new MailAddress(from);
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;
            Attachment PieceJointe = new Attachment(new MemoryStream(attachment.FileContents), attachment.FileDownloadName, attachment.ContentType);
            mail.Attachments.Add(PieceJointe);
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential(_configuration["Email:User"], _configuration["Email:Password"]);
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
        }
    }
}
