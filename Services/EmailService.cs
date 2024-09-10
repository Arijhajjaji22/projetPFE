using App_plateforme_de_recurtement.Models;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using App_plateforme_de_recurtement.DTOs;
using EmailDto = App_plateforme_de_recurtement.DTOs.EmailDto; // Pour EmailDto
namespace App_plateforme_de_recurtement.Services
{
    public class EmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _senderEmail;
        private readonly UserService _userService;
        public EmailService(string smtpServer, int smtpPort, string smtpUsername, string smtpPassword, string senderEmail)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
            _senderEmail = senderEmail;
          

            // Configurer TLS 1.2 pour le client SMTP
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        
        }

        public async Task SendEmailVerification(string recipientEmail, string validationToken)
        {
            try
            {
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;

                    // Assurez-vous que l'URL de vérification est correcte
                    var verificationLink = $"https://localhost:7031/verify-email?token={validationToken}"; // Utiliser https pour SSL
                    var message = new MailMessage();
                    message.From = new MailAddress(_senderEmail);
                    message.To.Add(recipientEmail);
                    message.Subject = "Vérification d'adresse e-mail";
                    message.Body = $"Veuillez cliquer sur le lien suivant pour vérifier votre adresse e-mail : {verificationLink}";

                    await client.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur est survenue lors de l'envoi de l'email : {ex.Message}");
                throw;
            }
        }




        public async Task SendEmail(string recipientEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                client.EnableSsl = true;

                var message = new MailMessage();
                message.From = new MailAddress(_smtpUsername); // ou un autre email de l'expéditeur
                message.To.Add(recipientEmail);
                message.Subject = subject;
                message.Body = body;

                await client.SendMailAsync(message);
            }
        }
        public async Task SendEmailAsync(DTOs.EmailDto emailDto)
        {
            await SendEmail(emailDto.To, emailDto.Subject, emailDto.Body);
        }

    }
}
