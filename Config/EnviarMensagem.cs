using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Serviços.Config
{
    public class EnviarMensagem
    {
        public EnviarMensagem() { }
        public async Task Enviar(string email, string assunto, string Remetente, string mensagem, string senha, string token)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Serviços", email));
            message.To.Add(MailboxAddress.Parse(email));

            var linkConfirmacao = $"https://seusite.com/confirmar?token={token}";

            var body = new TextPart("html")
            {
                Text = $@"
                <html>
                    <body style='font-family: Arial;'>
                        <h2>Confirmação de Conta</h2>
                            <p>Clique no botão abaixo para confirmar sua conta:</p>
                
                            <a href='{linkConfirmacao}' 
                            style='display:inline-block;
                            padding:10px 20px;
                            background-color:#28a745;
                            color:white;
                            text-decoration:none;
                            border-radius:5px;'>
                        Confirmar Conta
                            </a>

                        <p>Ou copie o link:</p>
                        <p>{linkConfirmacao}</p>
                    </body>
                </html>
    "
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(email, senha);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);

        }
    }
}
