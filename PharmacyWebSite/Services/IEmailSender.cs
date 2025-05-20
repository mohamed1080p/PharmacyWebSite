using PharmacyWebSite.Models;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Net.Sockets;

namespace PharmacyWebSite.Services
{
    public interface IEmailSender
    {
        Task SendOrderConfirmationEmail(string email, Order order);
        Task<bool> TestConnectionAsync();
    }

    public class EmailSender : IEmailSender, IDisposable
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailSender> _logger;
        private readonly SmtpClient _smtpClient;
        private readonly object _connectionLock = new();
        private bool _disposed;

        public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
        {
            _config = config;
            _logger = logger;
            _smtpClient = new SmtpClient
            {
                Timeout = 20000,
                SslProtocols = System.Security.Authentication.SslProtocols.Tls12
            };
        }

        public async Task SendOrderConfirmationEmail(string email, Order order)
        {
            if (_disposed)
                throw new ObjectDisposedException("EmailSender has been disposed");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _config["EmailSettings:SenderName"],
                _config["EmailSettings:SenderEmail"]));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = $"Order Confirmation #{order.OrderId} - Dawaya Pharmacy";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = BuildEmailBody(order)
            };
            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                await EnsureConnectedAsync();
                await _smtpClient.SendAsync(message);
                _logger.LogInformation($"Email successfully sent to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to send email: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                await EnsureConnectedAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task EnsureConnectedAsync()
        {
            lock (_connectionLock)
            {
                if (_smtpClient.IsConnected) return;
            }

            try
            {
                await _smtpClient.ConnectAsync(
                    _config["EmailSettings:MailServer"],
                    int.Parse(_config["EmailSettings:MailPort"]),
                    SecureSocketOptions.StartTls);

                await _smtpClient.AuthenticateAsync(
                    _config["EmailSettings:Username"],
                    _config["EmailSettings:Password"]);
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        private string BuildEmailBody(Order order)
        {
            return $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                <h2 style='color: #2a7fba;'>Thank you for your order!</h2>
                <p>Dear Valued Customer,</p>
                <p>Your order <strong>#{order.OrderId}</strong> has been successfully processed.</p>
                
                <h3 style='color: #3bb77e;'>Order Details</h3>
                <table style='width: 100%; border-collapse: collapse;'>
                    <tr style='background-color: #f8f9fa;'>
                        <th style='padding: 10px; border: 1px solid #dee2e6;'>Medicine</th>
                        <th style='padding: 10px; border: 1px solid #dee2e6;'>Quantity</th>
                        <th style='padding: 10px; border: 1px solid #dee2e6;'>Price</th>
                        <th style='padding: 10px; border: 1px solid #dee2e6;'>Total</th>
                    </tr>
                    {string.Join("", order.OrderItems.Select(oi =>
                        $@"<tr>
                            <td style='padding: 10px; border: 1px solid #dee2e6;'>{oi.Medicine.Name}</td>
                            <td style='padding: 10px; border: 1px solid #dee2e6; text-align: center;'>{oi.Quantity}</td>
                            <td style='padding: 10px; border: 1px solid #dee2e6;'>{oi.Price:C}</td>
                            <td style='padding: 10px; border: 1px solid #dee2e6; font-weight: bold;'>{(oi.Quantity * oi.Price):C}</td>
                        </tr>"))}
                </table>
                
                <div style='margin-top: 20px; font-size: 1.1em;'>
                    <p><strong>Grand Total: {order.OrderItems.Sum(oi => oi.Quantity * oi.Price):C}</strong></p>
                </div>
                
                <p style='margin-top: 30px;'>
                    Best regards,<br>
                    <strong>Dawaya Pharmacy Team</strong><br>
                    <a href='https://yourpharmacy.com' style='color: #2a7fba;'>www.dawaya-pharmacy.com</a>
                </p>
            </div>";
        }

        public void Dispose()
        {
            if (_disposed) return;

            lock (_connectionLock)
            {
                if (_smtpClient.IsConnected)
                {
                    _smtpClient.Disconnect(true);
                }
                _smtpClient.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}