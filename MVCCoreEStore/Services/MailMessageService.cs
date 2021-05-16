using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System.Net;
using System.Threading.Tasks;

namespace MVCCoreEStore.Services
{
    public class MailMessageService : IMailMessageService
    {
        private readonly IConfiguration configuration;

        public MailMessageService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<bool> Send(string to, string subject, string body)
        {
            var mm = new MimeMessage();

            var from = new MailboxAddress(configuration.GetValue<string>("Email:FromName"), 
                configuration.GetValue<string>("Email:Account"));
            mm.From.Add(from);

            mm.To.Add(new MailboxAddress("", to));

            mm.Subject = subject;

            mm.Body = new TextPart(TextFormat.Html)
            {
                Text = body,
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(configuration.GetValue<string>("Email:Host"), 
                        configuration.GetValue<int>("Email:Port"), 
                        MailKit.Security.SecureSocketOptions.Auto).ConfigureAwait(false);
                    await client.AuthenticateAsync(new NetworkCredential(configuration.GetValue<string>("Email:UserName"), 
                        configuration.GetValue<string>("Email:Password")));
                    await client.SendAsync(mm).ConfigureAwait(false);
                    await client.DisconnectAsync(true).ConfigureAwait(false);
                }
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}
