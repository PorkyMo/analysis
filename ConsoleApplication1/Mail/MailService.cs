using System;
using MailKit.Net.Smtp;
using MimeKit;

namespace DataAnalyst.Mail
{
    public class MailService
    {
        private static void SetConfig(SmtpClient client)
        {
            client.Connect("smtp.gmail.com", 465, MailKit.Security.SecureSocketOptions.SslOnConnect);
            client.Authenticate("gingin.gui@gmail.com", "Porky20151228");
        }

        private static void SendMessage(string stockCode, decimal puchasePrice, decimal currentPrice, string subject)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("StockAlert", "gingin.gui@gmail.com"));
            mailMessage.To.Add(new MailboxAddress("Jian", "ljianl@yahoo.com.au"));
            mailMessage.Subject = subject;

            mailMessage.Body = new TextPart("plain")
            {
                Text = $@"StockCode: {stockCode} {Environment.NewLine} 
                         Purchase: {puchasePrice} {Environment.NewLine}
                         Current: {currentPrice}"
            };

            using (var smtpClient = new SmtpClient())
            {
                SetConfig(smtpClient);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }
        }

        public static void SendStopLossAlert(string stockCode, decimal puchasePrice, decimal currentPrice)
        {
            SendMessage(stockCode, puchasePrice, currentPrice, $"Stop Loss {stockCode}");
        }

        public static void SendStopProfitAlert(string stockCode, decimal puchasePrice, decimal currentPrice)
        {
            SendMessage(stockCode, puchasePrice, currentPrice, $"Stop Profit {stockCode}");
        }

        public static void SendProfitDownToLossAlert(string stockCode, decimal puchasePrice, decimal currentPrice)
        {
            SendMessage(stockCode, puchasePrice, currentPrice, $"Profit Down To Loss {stockCode}");
        }
    }
}
