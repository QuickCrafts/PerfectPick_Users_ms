using _PerfectPickUsers_MS.Models.Email;
using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;

namespace _PerfectPickUsers_MS.Functions
{
    public class EmailSender
    {
        private readonly string _emailUsername;
        private readonly string _emailPassword;
        private readonly string _emailHost;
        private readonly int _emailPort;

        public EmailSender()
        {
            try
            {
                _emailUsername = Environment.GetEnvironmentVariable("emailUsername") ?? throw new Exception("Email username not found");
                _emailPassword = Environment.GetEnvironmentVariable("emailPassword") ?? throw new Exception("Email password not found");
                _emailHost = Environment.GetEnvironmentVariable("emailHost") ?? throw new Exception("Email host not found");
                _emailPort = int.Parse(Environment.GetEnvironmentVariable("emailPort") ?? throw new Exception("Email port not found"));
            }
            catch (Exception e)
            {
                throw new Exception("Error: " + e.Message);
            }
        }
        public void SendEmail(EmailDTO email)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(MailboxAddress.Parse(_emailUsername));
            emailMessage.To.Add(MailboxAddress.Parse(email.To));
            emailMessage.Subject = email.Subject;
            emailMessage.Body = new TextPart(TextFormat.Html) { Text = email.Body };

            using (var client = new SmtpClient())
            {
                client.Connect(_emailHost, _emailPort, SecureSocketOptions.StartTls);
                client.Authenticate(_emailUsername, _emailPassword);
                client.Send(emailMessage);
                client.Disconnect(true);
            }

        }
    }
}
