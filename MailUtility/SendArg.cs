using System.Collections.Concurrent;
using System.Net.Mail;

namespace MailUtility
{
    public class SendArg
    {
        public string File { get; set; }

        public int RetryTime { get; set; }

        public SmtpClient Client { get; set; }

        public MailMessage Message { get; set; }

        public ConcurrentQueue<string> QueuedFiles { get; set; }

        public SendArg Retry()
        {
            return new SendArg { File = File, RetryTime = RetryTime + 1, Client = Client, Message = Message, QueuedFiles = QueuedFiles };
        }
    }
}