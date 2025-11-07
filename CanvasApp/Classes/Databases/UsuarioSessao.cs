using System;

namespace CanvasApp
{
    public static class UsuarioSessao
    {
        public static int Codigo { get; set; } = 1;
        public static string Nome { get; set; } = "Usuário";
        public static string Email { get; set; } = "usuario@exemplo.com";

        // Método para definir a sessão do usuário
        public static void DefinirUsuario(int codigo, string nome, string email)
        {
            Codigo = codigo;
            Nome = nome;
            Email = email;
        }
    }
}