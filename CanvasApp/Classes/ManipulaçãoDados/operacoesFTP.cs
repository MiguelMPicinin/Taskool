using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace CanvasApp.Classes.ManipulaçãoDados.Prova_4_teste
{
    public class FTPManager
    {
        public bool EnviarArquivoParaFTP(string server, string username, string password,
                                       string filePath, string remoteFileName)
        {
            try
            {
                // Verificar se arquivo existe
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Arquivo não encontrado!", "Erro",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Verificar tamanho (5MB)
                FileInfo fileInfo = new FileInfo(filePath);
                if (fileInfo.Length > 5 * 1024 * 1024)
                {
                    MessageBox.Show("Arquivo muito grande! Limite: 5MB", "Erro",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Verificar se arquivo já existe no servidor
                if (VerificarArquivoExistenteFTP(server, username, password, remoteFileName))
                {
                    DialogResult result = MessageBox.Show(
                        $"O arquivo '{remoteFileName}' já existe no servidor. Deseja substituir?",
                        "Arquivo Existente",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result != DialogResult.Yes)
                        return false;
                }

                // Fazer upload
                FazerUploadFTP(server, username, password, filePath, remoteFileName);

                MessageBox.Show("Arquivo enviado para FTP com sucesso!", "Sucesso",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao enviar para FTP: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool VerificarArquivoExistenteFTP(string server, string username,
                                                string password, string fileName)
        {
            try
            {
                string fullUri = server.EndsWith("/") ? server + fileName : server + "/" + fileName;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(fullUri);
                request.Credentials = new NetworkCredential(username, password);
                request.Method = WebRequestMethods.Ftp.GetFileSize;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    return true;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }

        private void FazerUploadFTP(string server, string username, string password,
                                  string filePath, string remoteFileName)
        {
            string fullUri = server.EndsWith("/") ? server + remoteFileName : server + "/" + remoteFileName;

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(fullUri);
            request.Credentials = new NetworkCredential(username, password);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            request.UsePassive = true;

            using (FileStream fileStream = File.OpenRead(filePath))
            using (Stream requestStream = request.GetRequestStream())
            {
                fileStream.CopyTo(requestStream);
            }
        }
    }
}