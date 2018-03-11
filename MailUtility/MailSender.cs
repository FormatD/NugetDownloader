using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace MailUtility
{

    public class EmptyLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return new Scope();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Console.WriteLine($"{logLevel} {formatter(state, exception)}");
        }

        public class Scope : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }


    public class MailSender
    {
        private readonly ILogger _logger;
        private readonly Config _config;



        public MailSender(Config config, ILogger logger)
        {
            _config = config;
            _logger = logger;
        }

        public void SendEmail(string file, string subject = null)
        {
            var queue = new ConcurrentQueue<string>();
            if (!File.Exists(file))
            {
                _logger?.LogError("file \"{0}\" was not existed.", file);
                return;
            }

            subject = subject ?? Path.GetFileNameWithoutExtension(file);
            var fileList = SplitFile(file);

            foreach (var fileFragement in fileList)
            {
                queue.Enqueue(fileFragement);

                SmtpClient client = new SmtpClient(_config.SmtpServerAddress, _config.SmtpServerPort)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential(_config.UserName, _config.UserPass),
                    Timeout = 120000,
                };

                client.SendCompleted += Client_SendCompleted;

                var message = CreateMessage(fileFragement, subject);
                try
                {
                    _logger?.LogInformation($"begin send file {fileFragement}.");
                    // client.Send(message);
                    client.SendAsync(message, new SendArg { File = fileFragement, RetryTime = 0, Client = client, Message = message, QueuedFiles = queue });
                }
                catch (Exception ex)
                {
                    _logger?.LogError($"Exception caught in SendEmail(): {ex}");
                }
            }

            while (queue.Any())
            {
                Thread.Sleep(100);
            }
        }

        private void Client_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            var sendArg = e.UserState as SendArg;
            if (sendArg == null)
                return;

            if (e.Error != null)
            {
                _logger?.LogWarning($"Error when send file {sendArg.File} ,the {sendArg.RetryTime + 1} times.");
                //if (sendArg.RetryTime < 3)
                sendArg.Client.SendAsync(sendArg.Message, sendArg.Retry());
            }
            else
            {
                _logger?.LogInformation($"Sucess when send file {sendArg.File} ,the {sendArg.RetryTime + 1} times.");
                string fileName;
                while (!sendArg.QueuedFiles.TryDequeue(out fileName))
                {
                }
            }
        }

        private IEnumerable<string> SplitFile(string file)
        {
            var fileList = new List<string>();
            var fileInfo = new FileInfo(file);
            if (!fileInfo.Exists)
                return fileList;

            if (fileInfo.Length <= _config.MaxSize)
            {
                fileList.Add(file);
                return fileList;
            }

            var offset = 0;
            var index = 1;
            using (var fs = File.OpenRead(file))
            {
                byte[] fileBytes = new byte[_config.MaxSize];

                var readByte = fs.Read(fileBytes, offset, _config.MaxSize);

                while (readByte > 0)
                {
                    var targetBytes = readByte == _config.MaxSize ? fileBytes : fileBytes.Take(readByte).ToArray();
                    var fileName = $"{file}.{index:D3}";
                    File.WriteAllBytes(fileName, targetBytes);

                    fileList.Add(fileName);

                    offset += readByte;
                    index++;
                    readByte = fs.Read(fileBytes, 0, _config.MaxSize);
                }
            }

            return fileList;
        }

        private MailMessage CreateMessage(string file, string subject = null)
        {
            var message = new MailMessage()
            {
                From = new MailAddress(_config.SendFrom),
                Sender = new MailAddress(_config.SendFrom),
                Subject = subject ?? $"{Path.GetFileNameWithoutExtension(file)}.",
                Body = $"file of {file}."
            };

            foreach (var mailAddress in _config.SendTo.Split(';'))
            {
                message.To.Add(new MailAddress(mailAddress, mailAddress));
            }

            var data = new Attachment(file, MediaTypeNames.Application.Octet)
            {
                Name = Path.GetFileName(file)
            };
            var disposition = data.ContentDisposition;
            disposition.CreationDate = File.GetCreationTime(file);
            disposition.ModificationDate = File.GetLastWriteTime(file);
            disposition.ReadDate = File.GetLastAccessTime(file);
            message.Attachments.Add(data);
            return message;
        }
    }
}