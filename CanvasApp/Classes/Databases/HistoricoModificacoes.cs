using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasApp.Classes.Databases.UsuarioCL
{
    public class HistoricoModificacoes
    {
        public int Codigo { get; set; }
        public int CodTarefa { get; set; }
        public int CodUsuario { get; set; }
        public DateTime Data { get; set; }
        public string Texto { get; set; }
        public string NomeUsuario { get; set; } // Adicionado para armazenar o nome do usuário
    }
}