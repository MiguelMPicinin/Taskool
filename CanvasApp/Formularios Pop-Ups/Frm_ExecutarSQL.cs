using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace CanvasApp.Formularios_Pop_Ups
{
    public partial class Frm_ExecutarSQL : Form
    {
        public Frm_ExecutarSQL()
        {
            InitializeComponent();
            ConfigurarControlesIniciais();
        }

        private void ConfigurarControlesIniciais()
        {
            // Configuração inicial dos controles
            Lbl_Titulo.Text = "Autenticação SQL Server";
            Lbl_Usuario.Text = "Usuário:";
            Lbl_Senha.Text = "Senha:";

            Chk_AutenticacaoWindows.Checked = true;
            Txt_Usuario.Enabled = false;
            Txt_Senha.Enabled = false;

            Btn_Executar.Text = "Executar Script";
            Btn_Executar.BackColor = System.Drawing.Color.FromArgb(74, 124, 255);
            Btn_Executar.ForeColor = System.Drawing.Color.White;

            Txt_Resultado.Multiline = true;
            Txt_Resultado.ScrollBars = ScrollBars.Both;
            Txt_Resultado.ReadOnly = true;
        }

        private void Chk_AutenticacaoWindows_CheckedChanged(object sender, EventArgs e)
        {
            bool usarWindows = Chk_AutenticacaoWindows.Checked;
            Txt_Usuario.Enabled = !usarWindows;
            Txt_Senha.Enabled = !usarWindows;

            if (usarWindows)
            {
                Txt_Usuario.Clear();
                Txt_Senha.Clear();
            }
        }

        private void Btn_Executar_Click(object sender, EventArgs e)
        {
            try
            {
                Txt_Resultado.Clear();
                Txt_Resultado.AppendText("Iniciando execução do script..." + Environment.NewLine);

                // Aqui viria a lógica de conexão com o SQL Server
                string connectionString = ObterConnectionString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    Txt_Resultado.AppendText("Conectado ao SQL Server com sucesso!" + Environment.NewLine);

                    // Exemplo de execução (substitua pelo seu script)
                    string sql = "SELECT @@VERSION";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        var result = command.ExecuteScalar();
                        Txt_Resultado.AppendText("Resultado: " + result.ToString() + Environment.NewLine);
                    }

                    connection.Close();
                }

                Txt_Resultado.AppendText("Script executado com sucesso!" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Txt_Resultado.AppendText($"ERRO: {ex.Message}" + Environment.NewLine);
            }
        }

        private string ObterConnectionString()
        {
            // Substitua pelos seus dados de conexão
            string server = "localhost";
            string database = "master";

            if (Chk_AutenticacaoWindows.Checked)
            {
                return $"Server={server};Database={database};Integrated Security=True;";
            }
            else
            {
                return $"Server={server};Database={database};User Id={Txt_Usuario.Text};Password={Txt_Senha.Text};";
            }
        }

        // Conecte os eventos no construtor ou via designer
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Chk_AutenticacaoWindows.CheckedChanged += Chk_AutenticacaoWindows_CheckedChanged;
            Btn_Executar.Click += Btn_Executar_Click;
        }
    }
}