using System;

namespace CanvasApp.Classes.Databases.UsuarioCL
{
    public class Tarefas_Anexos
    {
        public int Codigo { get; set; }
        public int CodTarefa { get; set; }
        public string NomeArquivo { get; set; }
        public string Arquivo { get; set; } // Armazena em Base64
        public DateTime DataUpload { get; set; }
        public string Extensao { get; set; } // Nova propriedade para armazenar a extensão
    }
}