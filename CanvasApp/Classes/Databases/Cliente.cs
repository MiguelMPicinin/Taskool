using System;

namespace CanvasApp.Classes.Databases.UsuarioCL
{
    public class Usuario
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string NomeUsuario { get; set; }  // Adicionado para o nome de usuário
        public string Telefone { get; set; }
        public string DataNascimento { get; set; }
        public byte[] Foto { get; set; }

        public override string ToString()
        {
            return NomeUsuario; // ou Nome, conforme preferir
        }
    }
}
