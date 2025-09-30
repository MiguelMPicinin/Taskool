using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasApp.Classes.Databases.UsuarioCL
{
    public class Projeto_Tarefas
    {
        public int Codigo { get; set; }
        public int CodProjeto { get; set; }
        public string CodUsuario { get; set; }
        public string Descricao { get; set; }
        public bool isConcluida { get; set; }
    }
}