using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CanvasApp.Classes.Databases.MensagemFun
{
    public class MensagemFun
    {
        public string diretorio;
        public string Mensagem;
        public bool status;

        public string Buscar(string mensagem)
        {
            
            try
            {
                string caminho = Path.Combine(diretorio, mensagem + ".json");

                if (!File.Exists(caminho))
                {
                    status = false;
                    Mensagem = $"Mensagem não existente: {mensagem}";
                    return null;
                }

                string conteudo = File.ReadAllText(caminho);
                status = true;
                Mensagem = $"Mensagem encontrada com sucesso: {mensagem}";
                return conteudo;
            }
            catch (Exception ex)
            {
                status = false;
                Mensagem = $"Erro ao buscar a mensagem: {ex.Message}";
                return null;
            }
        }

        
    }
}
