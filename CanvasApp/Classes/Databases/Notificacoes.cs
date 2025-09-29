using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasApp.Classes.Databases
{
    public class Notificacoes
    {
        public int Codigo { get; set; }
        public string Texto { get; set; }
        public DateTime Data { get; set; }
        public int CodProjeto { get; set; }
        public int CodUsuario { get; set; }
        public bool isFechada { get; set; }

    }
}
