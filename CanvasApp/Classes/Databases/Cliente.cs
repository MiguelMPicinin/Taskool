using System;

namespace CanvasApp.Classes.Databases.UsuarioCL
{
    public class Usuario
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string NomeUsuario { get; set; }
        public string Telefone { get; set; }
        public string DataNascimento { get; set; }
        public byte[] Foto { get; set; }

        public override string ToString()
        {
            return NomeUsuario;
        }
    }
}