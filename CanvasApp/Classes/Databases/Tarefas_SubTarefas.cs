using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasApp.Classes.Databases.UsuarioCL
{
    public class Tarefas_SubTarefas
    {
        public int Codigo { get; set; }
        public string Texto { get; set; }
        public int CodTarefa { get; set; }
        public bool isConcluida { get; set; }
    }
}
