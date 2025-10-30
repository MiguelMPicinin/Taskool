using System;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasApp
{
    public partial class FormOpcoesFTP : Form
    {
        public OpcaoFTP OpcaoSelecionada { get; private set; }

        private RadioButton radioDashboardAtual;
        private RadioButton radioArquivoExistente;

        public FormOpcoesFTP()
        {
            // REMOVA o arquivo FormOpcoesFTP.Designer.cs se existir
            InitializeComponentManual();
        }

        private void InitializeComponentManual()
        {
            this.Text = "Enviar para FTP";
            this.Size = new Size(400, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Label de instrução
            Label lblInstrucao = new Label()
            {
                Text = "Escolha o que deseja enviar para o servidor FTP:",
                Location = new Point(20, 20),
                Size = new Size(350, 20),
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(lblInstrucao);

            // Opção 1: Dashboard Atual
            radioDashboardAtual = new RadioButton()
            {
                Text = "Dashboard Atual (Gerar PDF automaticamente)",
                Location = new Point(30, 50),
                Size = new Size(350, 20),
                Checked = true
            };
            this.Controls.Add(radioDashboardAtual);

            // Opção 2: Arquivo Existente
            radioArquivoExistente = new RadioButton()
            {
                Text = "Arquivo Existente do Computador",
                Location = new Point(30, 80),
                Size = new Size(350, 20)
            };
            this.Controls.Add(radioArquivoExistente);

            // Botão Confirmar
            Button btnConfirmar = new Button()
            {
                Text = "Confirmar",
                Location = new Point(200, 120),
                Size = new Size(80, 30)
            };
            btnConfirmar.Click += BtnConfirmar_Click;
            this.Controls.Add(btnConfirmar);

            // Botão Cancelar
            Button btnCancelar = new Button()
            {
                Text = "Cancelar",
                Location = new Point(290, 120),
                Size = new Size(80, 30)
            };
            btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnCancelar);
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (radioDashboardAtual.Checked)
                OpcaoSelecionada = OpcaoFTP.DashboardAtual;
            else
                OpcaoSelecionada = OpcaoFTP.ArquivoExistente;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

    public enum OpcaoFTP
    {
        DashboardAtual,
        ArquivoExistente
    }
}