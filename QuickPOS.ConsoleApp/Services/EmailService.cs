using System;
using System.Net;
using System.Net.Mail;

namespace QuickPOS.Services
{
    public class EmailService
    {
        // --- CONFIGURACIÓN DE TU CORREO ---
        // NOTA: Si usas Gmail con verificación en 2 pasos, 
        // NO pongas tu contraseña normal. Debes crear una "Contraseña de Aplicación".
        private readonly string _senderEmail = "robertodavid2405@gmail.com";
        private readonly string _senderPassword = "kvod fkxb bzpm swir";

        public bool SendRecoveryCode(string recipientEmail, string recoveryCode)
        {
            try
            {
                // Configurar el cliente SMTP (El cartero)
                // Esto está configurado para GMAIL. Si usas Outlook/Hotmail cambia el host.
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(_senderEmail, _senderPassword),
                    EnableSsl = true,
                };

                // Crear el mensaje (La carta)
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_senderEmail, "Soporte QuickPOS"),
                    Subject = "Recuperación de Contraseña - QuickPOS",
                    Body = $@"
                        <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                            <h2 style='color: #0078D7;'>Código de Recuperación</h2>
                            <p>Hola,</p>
                            <p>Hemos recibido una solicitud para restablecer tu contraseña.</p>
                            <p>Tu código de seguridad es:</p>
                            <h1 style='background-color: #f0f0f0; padding: 10px; display: inline-block; letter-spacing: 5px;'>{recoveryCode}</h1>
                            <p>Si no fuiste tú, ignora este mensaje.</p>
                            <hr>
                            <small>Sistema QuickPOS</small>
                        </div>",
                    IsBodyHtml = true, // Importante para que se vea bonito
                };

                // Poner destinatario
                mailMessage.To.Add(recipientEmail);

                // Enviar
                smtpClient.Send(mailMessage);
                return true; // ¡Se envió!
            }
            catch (Exception ex)
            {
                // Si falla (ej. sin internet, clave mal), guardamos el error en consola
                Console.WriteLine("Error enviando correo: " + ex.Message);
                return false;
            }
        }
    }
}