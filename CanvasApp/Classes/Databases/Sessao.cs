namespace CanvasApp.Classes.Databases.UsuarioCL
{
    public static class Sessao
    {
        public static Usuario UsuarioLogado { get; set; }

        public static bool EstaLogado()
        {
            return UsuarioLogado != null;
        }

        public static void Logout()
        {
            UsuarioLogado = null;
        }
    }
}