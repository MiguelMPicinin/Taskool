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
        public int? CodUsuario { get; set; }
        public int? CodResponsavel { get; set; }
        public string Descricao { get; set; }
        public bool isConcluida { get; set; }
        public bool isFazendo { get; set; }
        public string Cor { get; set; }
        public DateTime dataConclusao { get; set; }
        public DateTime dataLimite { get; set; }

        // Removida a propriedade Posicao que não existe na tabela
    }
}