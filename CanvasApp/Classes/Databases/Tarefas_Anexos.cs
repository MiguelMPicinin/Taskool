using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasApp.Classes.Databases.UsuarioCL
{
    public class Tarefas_Anexos
    {
        public int Codigo { get; set; }
        public int CodTarefa { get; set; }
        public string NomeArquivo { get; set; }
        public string Arquivo { get; set; }
        public DateTime DataUpload { get; set; }
    }
}