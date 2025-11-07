using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CanvasApp.Classes.ManipulaçãoDados
{
    private void EnviarEmail(string nomeProjeto, string nomeDestinatario, string emailDestinatario, string nomeCartao, string acao)
    {
        try
        {
            using (SmtpClient smtpClient = new SmtpClient("127.0.0.1", 8087))
            {
                smtpClient.EnableSsl = false;
                smtpClient.UseDefaultCredentials = true;

                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress("worldskills2019@gmail.com");
                    mailMessage.To.Add(emailDestinatario);
                    mailMessage.Subject = $"Nova ação no projeto {nomeProjeto}";
                    mailMessage.Body = $"Olá, {nomeDestinatario}. Voce acabou de realizar uma nova ação no projeto {nomeProjeto}: - {acao} o cartão {nomeCartao} para feito.";
                    mailMessage.IsBodyHtml = false;

                    smtpClient.Send(mailMessage);
                }
            }

            MessageBox.Show("E-mail enviado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro ao enviar e-mail: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
