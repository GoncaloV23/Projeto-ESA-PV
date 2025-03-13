using EasyFitHub.Models.Account;
using System.Net.Mail;
using System.Net;
using System.Net.Http;
using System.Security.Policy;

namespace EasyFitHub.Utils
{
    /// <summary>
    /// AUTHOR: Rui Barroso
    /// Destinado a efetuar emaisl.
    /// </summary>
    public class EmailSender
    {
        private const string CompanyEmail = "easyfithub@outlook.pt";
        private const string EmailPassword = "projesa2024";

        private static SmtpClient _smtpClient = new SmtpClient("smtp-mail.outlook.com")
        {
            Port = 587,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(CompanyEmail, EmailPassword),
            EnableSsl = true,
        };

        public static async void SendEmail(string emailTarget, string subject, string content)
        {
            try
            {
                var mailMessage = new MailMessage
                (
                    from: CompanyEmail,
                    to: emailTarget,
                    subject,
                    content
                );

                await _smtpClient.SendMailAsync(mailMessage);
            }
            catch (SmtpException ex)
            {
                // Handle email sending exception
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        public static void SendPasswordRecoveryEmail(Account userAccount, string code)
        {
            SendEmail(
                userAccount.Email,
                "Password Recovery",
                $"Use this Code to change your password: {code}"
            );
        }

        public static void SendReceipt(Account userAccount, string content)
        {
            SendEmail(
                userAccount.Email,
                "Receipt",
                content
            );
        }
    }
}
