using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Windows.Forms;

namespace CanvasApp.Classes.ManipulaçãoDados
{
    public class EmailService
    {
        public void EnviarEmail(string nomeProjeto, string nomeDestinatario, string emailDestinatario, string nomeCartao, string acao)
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
                        mailMessage.Body = $"Olá, {nomeDestinatario}. Você acabou de realizar uma nova ação no projeto {nomeProjeto}: - {acao} o cartão {nomeCartao}.";
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

        // Método sobrecarregado para uso mais simples
        public void EnviarEmailNotificacao(string acao, string nomeCartao, string nomeProjeto)
        {
            try
            {
                if (CanvasApp.Classes.Databases.UsuarioCL.Sessao.UsuarioLogado == null) return;

                var usuario = CanvasApp.Classes.Databases.UsuarioCL.Sessao.UsuarioLogado;
                EnviarEmail(nomeProjeto, usuario.Nome, usuario.Email, nomeCartao, acao);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar notificação: {ex.Message}");
            }
        }

        // Método para enviar e-mail para múltiplos destinatários
        public void EnviarEmailParaEquipe(string nomeProjeto, List<string> emailsDestinatarios, string nomeCartao, string acao, string nomeRemetente)
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

                        // Adicionar todos os destinatários
                        foreach (var email in emailsDestinatarios)
                        {
                            if (!string.IsNullOrEmpty(email))
                                mailMessage.To.Add(email);
                        }

                        mailMessage.Subject = $"Nova ação no projeto {nomeProjeto}";
                        mailMessage.Body = $"Olá, membros da equipe. {nomeRemetente} acabou de realizar uma nova ação no projeto {nomeProjeto}: - {acao} o cartão {nomeCartao}.";
                        mailMessage.IsBodyHtml = false;

                        if (mailMessage.To.Count > 0)
                        {
                            smtpClient.Send(mailMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar e-mail para equipe: {ex.Message}");
            }
        }
    }
}