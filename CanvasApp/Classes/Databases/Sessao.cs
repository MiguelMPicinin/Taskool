using System;

namespace CanvasApp
{
    public static class Sessao
    {
        public static Usuario UsuarioLogado { get; set; }

        public class Usuario
        {
            public string Codigo { get; set; }
            public string Nome { get; set; }
            public string Email { get; set; }
            // Adicione outras propriedades conforme necessário
        }
    }
}