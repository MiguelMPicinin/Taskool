using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasApp.Classes.Databases
{
    public class Tarefas_Comentarios
    {
        public int Codigo { get; set; }
        public int CodUsuario { get; set; } // Corrigido para int
        public int CodTarefa { get; set; }
        public string Comentario { get; set; }
        public DateTime Data { get; set; }
    }
}