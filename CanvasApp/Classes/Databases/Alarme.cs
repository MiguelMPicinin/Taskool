using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasApp.Classes.Databases
{
    public enum RepeticaoAlarme
    {
        N,
        D,
        S,
        M
    }
    public class Alarme
    {
        public int Codigo { get; set; }
        public int CodTarefa { get; set; }
        public int CodUsuario { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Hora { get; set; } // Alterado para TimeSpan para compatibilidade com SQL time
        public RepeticaoAlarme Repeticao { get; set; }
    }
}