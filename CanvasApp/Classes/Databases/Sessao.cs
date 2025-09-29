using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CanvasApp.Classes.Databases.UsuarioCL;

namespace CanvasApp.Classes.Databases
{
    public static class Sessao
    {
        public static Usuario UsuarioLogado { get; set; }
    }
}
