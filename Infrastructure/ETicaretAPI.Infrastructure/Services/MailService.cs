using ETicaretAPI.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services
{
    public class MailService : IMailService
    {
        readonly IConfiguration _config;

        public MailService(IConfiguration config)
        {
            _config = config;
        }


        public async Task SendMailAsync(string[] tos, string subject, string body, bool isBodyHtml = true)
        {
            MailMessage mail = new();
            mail.IsBodyHtml = isBodyHtml;
            mail.Subject = subject;
            mail.Body = body;
            foreach (var to in tos)
            {
                mail.To.Add(to);
            }
            mail.From = new(_config["Mail:Username"], "Oğulcan Uçar", System.Text.Encoding.UTF8);

            SmtpClient smtp = new();
            smtp.Credentials = new NetworkCredential(_config["Mail:Username"], _config["Mail:Password"]);
            smtp.Port = 587;
            smtp.EnableSsl = true;
            smtp.Host = _config["Mail:Host"];
            await smtp.SendMailAsync(mail);


        }

        public async Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendMailAsync(new[] { to }, subject, body, isBodyHtml);
        }

        public async Task SendPasswordResetMailAsync(string to, string userId, string resetToken)
        {
            StringBuilder mail = new();
            mail.AppendLine("<html>");
            mail.AppendLine("<head>");
            mail.AppendLine("<style>");
            mail.AppendLine("body { font-family: Arial, sans-serif; background-color: #f2f2f2; }");
            mail.AppendLine(".container { max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f8f8f8; border: 1px solid #dddddd; }");
            mail.AppendLine("h1 { color: #ff6600; margin-top: 0; }");
            mail.AppendLine("p { color: #333; }");
            mail.AppendLine("a { color: #007bff; text-decoration: none; }");
            mail.AppendLine("a:hover { text-decoration: underline; }");
            mail.AppendLine(".button { display: inline-block; padding: 10px 20px; background-color: #007bff; color: #ffffff; text-decoration: none; border-radius: 4px; }");
            mail.AppendLine(".footer { font-size: 12px; color: #777; margin-top: 20px; }");
            mail.AppendLine("</style>");
            mail.AppendLine("</head>");
            mail.AppendLine("<body>");
            mail.AppendLine("<div class=\"container\">");
            mail.AppendLine("<h1>Şifre Yenileme Talebi</h1>");
            mail.AppendLine("<p>Merhaba,</p>");
            mail.AppendLine("<p>Şifre yenileme talebinde bulunduğunuz için bu e-postayı alıyorsunuz. Şifrenizi yenilemek için aşağıdaki bağlantıyı kullanabilirsiniz:</p>");
            mail.AppendLine("<p><a href=\"" + _config["AngularClientUrl"] + "/update-password/" + userId + "/" + resetToken + "\" target=\"_blank\" class=\"button\">Şifremi Yenile</a></p>");
            mail.AppendLine("<p>Eğer bu talebi siz gerçekleştirmediyseniz, endişelenmeyin. Bu e-postayı göz ardı edebilirsiniz.</p>");
            mail.AppendLine("<p class=\"footer\">Saygılarımızla,<br>Oğulcan Uçar</p>");
            mail.AppendLine("</div>");
            mail.AppendLine("</body>");
            mail.AppendLine("</html>");

            await SendMailAsync(to, "Şifre Yenileme Talebi", mail.ToString());
        }


        public async Task SendCompleteOrderMailAsync(string to, string userName, string orderCode, DateTime orderDate)
        {
            StringBuilder mail = new StringBuilder();
            mail.AppendLine("<html>");
            mail.AppendLine("<head>");
            mail.AppendLine("<style>");
            mail.AppendLine("body { font-family: Arial, sans-serif; background-color: #f1f1f1; }");
            mail.AppendLine(".container { max-width: 600px; margin: 0 auto; background-color: #fff; padding: 20px; border-radius: 10px; box-shadow: 0 0 10px rgba(0,0,0,0.1); }");
            mail.AppendLine(".header { background-color: #2196f3; padding: 20px; text-align: center; border-top-left-radius: 10px; border-top-right-radius: 10px; }");
            mail.AppendLine(".content { padding: 20px; color: #333; }");
            mail.AppendLine("h1 { color: #fff; margin-top: 0; }");
            mail.AppendLine("p { color: #666; }");
            mail.AppendLine("ul { color: #333; margin: 0; padding: 0 0 0 20px; }");
            mail.AppendLine("li { margin-bottom: 10px; }");
            mail.AppendLine("</style>");
            mail.AppendLine("</head>");
            mail.AppendLine("<body>");
            mail.AppendLine("<div class='container'>");
            mail.AppendLine("<div class='header'>");
            mail.AppendLine("<h1>Sipariş Tamamlandı</h1>");
            mail.AppendLine("</div>");
            mail.AppendLine("<div class='content'>");
            mail.AppendLine($"<p>Merhaba <span style='color: #2196f3;'>{userName},</span></p>");
            mail.AppendLine("<p>Siparişiniz başarıyla tamamlanmıştır.</p>");
            mail.AppendLine("<p>Detaylar:</p>");
            mail.AppendLine("<ul>");
            mail.AppendLine($"<li><strong>Sipariş Kodu:</strong> <span style='color: #2196f3;'>{orderCode}</span></li>");
            mail.AppendLine($"<li><strong>Sipariş Tarihi:</strong> <span style='color: #2196f3;'>{orderDate}</span></li>");
            mail.AppendLine("</ul>");
            mail.AppendLine("<p>Kargonuz kısa süre içinde gönderilecektir.</p>");
            mail.AppendLine("<p>Hayırlı kullanımlar...</p>");
            mail.AppendLine("</div>");
            mail.AppendLine("</div>");
            mail.AppendLine("</body>");
            mail.AppendLine("</html>");

            string mailBody = mail.ToString();

            await SendMailAsync(to, $"{orderCode} Sipariş Numaralı Siparişiniz Tamamlandı", mailBody);
        }





    }
}
