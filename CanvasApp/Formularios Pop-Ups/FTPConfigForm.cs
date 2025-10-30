using System;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasApp
{
    public partial class FTPConfigForm : Form
    {
        public string Servidor { get; private set; }
        public string Usuario { get; private set; }
        public string Senha { get; private set; }
        public string NomeArquivo { get; private set; }
        public string NomeArquivoSugerido { get; set; }

        public FTPConfigForm()
        {
            InitializeComponent();
        }

        // ✅ MÉTODO DE LOAD ADICIONADO
        private void FTPConfigForm_Load(object sender, EventArgs e)
        {
            // Preencher nome sugerido se disponível
            if (!string.IsNullOrEmpty(NomeArquivoSugerido))
                Txt_NomeArquivo.Text = NomeArquivoSugerido;

            // Configurar valores padrão
            if (string.IsNullOrEmpty(Txt_Servidor.Text))
                Txt_Servidor.Text = "ftp://";

            Console.WriteLine("✅ FTPConfigForm carregado com sucesso");
        }

        // ✅ MÉTODO DO BOTÃO OK ADICIONADO
        private void Btn_OK_Click(object sender, EventArgs e)
        {
            if (ValidarCampos())
            {
                Servidor = Txt_Servidor.Text;
                Usuario = Txt_Usuario.Text;
                Senha = Txt_Senha.Text;
                NomeArquivo = Txt_NomeArquivo.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(Txt_Servidor.Text))
            {
                MessageBox.Show("Preencha o servidor FTP.", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Servidor.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(Txt_Usuario.Text))
            {
                MessageBox.Show("Preencha o usuário.", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Usuario.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(Txt_Senha.Text))
            {
                MessageBox.Show("Preencha a senha.", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Senha.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(Txt_NomeArquivo.Text))
            {
                MessageBox.Show("Preencha o nome do arquivo.", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_NomeArquivo.Focus();
                return false;
            }

            return true;
        }

        // ✅ MÉTODO PARA CANCELAR COM ESC
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }
    }
}