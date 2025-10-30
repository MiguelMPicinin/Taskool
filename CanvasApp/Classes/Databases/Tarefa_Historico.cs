using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasApp.Classes.Databases.UsuarioCL
{
    public class Tarefas_Historico
    {
        public int Codigo { get; set; }
        public int CodTarefa { get; set; }
        public int CodUsuario { get; set; }
        public string Acao { get; set; }
        public DateTime DataAcao { get; set; }
    }
}