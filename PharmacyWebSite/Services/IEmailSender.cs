using PharmacyWebSite.Models;
using SendGrid.Helpers.Mail;
using SendGrid;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Net.Sockets;

namespace PharmacyWebSite.Services
{
    public interface IEmailSender
    {
        Task SendOrderConfirmationEmail(string email, Order order);
    }
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendOrderConfirmationEmail(string email, Order order)
        {
            using var client = new SmtpClient();
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(
                    _config["EmailSettings:SenderName"],
                    _config["EmailSettings:SenderEmail"]));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = $"Order Confirmation #{order.OrderId} - Dawaya Pharmacy";

                // Build email body
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
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
                </div>"
                };

                message.Body = bodyBuilder.ToMessageBody();

                // Configure connection settings
                client.Timeout = 15000; // 15 seconds timeout
                client.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;

                _logger.LogInformation($"Attempting to connect to SMTP server: {_config["EmailSettings:MailServer"]}:{_config["EmailSettings:MailPort"]}");

                // Establish connection with STARTTLS
                await client.ConnectAsync(
                    _config["EmailSettings:MailServer"],
                    int.Parse(_config["EmailSettings:MailPort"]),
                    SecureSocketOptions.StartTls);

                if (!client.IsConnected)
                {
                    _logger.LogError("Failed to connect to SMTP server");
                    throw new Exception("SMTP connection failed");
                }

                _logger.LogInformation("Authenticating with SMTP server...");

                await client.AuthenticateAsync(
                    _config["EmailSettings:Username"],
                    _config["EmailSettings:Password"]);

                if (!client.IsAuthenticated)
                {
                    _logger.LogError("SMTP authentication failed");
                    throw new Exception("SMTP authentication failed");
                }

                _logger.LogInformation("Sending confirmation email...");
                await client.SendAsync(message);

                _logger.LogInformation($"Email successfully sent to {email}");
            }
            catch (AuthenticationException authEx)
            {
                _logger.LogError($"Authentication failed: {authEx.Message}");
                throw new Exception("Email authentication failed. Please check your credentials.", authEx);
            }
            catch (SslHandshakeException sslEx)
            {
                _logger.LogError($"SSL Handshake failed: {sslEx.Message}");
                throw new Exception("Secure connection failed. Please verify SSL/TLS settings.", sslEx);
            }
            catch (SocketException sockEx)
            {
                _logger.LogError($"Network error: {sockEx.Message}");
                throw new Exception("Network connection failed. Please check your internet connection.", sockEx);
            }
            catch (Exception ex)
            {
                _logger.LogError($"General email error: {ex}");
                throw new Exception("Failed to send confirmation email. Please try again later.", ex);
            }
            finally
            {
                if (client.IsConnected)
                {
                    await client.DisconnectAsync(true);
                    _logger.LogInformation("Disconnected from SMTP server");
                }
            }
        }
    }

}
