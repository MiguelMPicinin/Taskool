using System;
using System.IO;
using System.Net;
using System.Linq;

namespace CanvasApp.Classes.Errors
{
    public class UsersErrors
    {
        private readonly string _logDirectory = @"C:\USER_LOGS";
        private readonly string _fileName;

        public UsersErrors(string login)
        {
            _fileName = Path.Combine(_logDirectory, $"{login}.txt");
        }

        public void Incluir(string usuario)
        {
            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
            if (!File.Exists(_fileName))
            {
                File.WriteAllText(_fileName, "Data;Hora;Usuario;IP" + Environment.NewLine);
            }
            string ipAddress = GetLocalIPAddress();
            string logEntry = $"{DateTime.Now.ToString("dd/MM/yyyy")};{DateTime.Now.ToString("HH:mm")};{usuario};{ipAddress}";
            File.AppendAllText(_fileName, logEntry + Environment.NewLine);
        }

        private string GetLocalIPAddress()
        {
            try
            {
                string hostName = Dns.GetHostName();
                IPHostEntry ipEntry = Dns.GetHostEntry(hostName);
                var ipv4 = ipEntry.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                return ipv4?.ToString() ?? "N/A";
            }
            catch
            {
                return "N/A";
            }
        }
    }
}