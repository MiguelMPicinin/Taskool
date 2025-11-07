using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasApp.Classes.Databases.UsuarioCL
{
    public class JogadorCarta
    {
        public string NomeJogador { get; set; }
        public int ValorCarta { get; set; }
        public DateTime DataEscolha { get; set; }
    }

    public class Tarefa
    {
        public string Nome { get; set; }
        public bool Concluida { get; set; }
        public List<JogadorCarta> Estimativas { get; set; } = new List<JogadorCarta>();
    }
}