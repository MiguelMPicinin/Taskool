using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasApp.Formularios_Pop_Ups
{
    public partial class Frm_AdicionarPostIt : Form
    {
        public string DescricaoTarefa { get; private set; }
        public string CorSelecionada { get; private set; }

        private readonly Dictionary<string, string> coresPostIt = new Dictionary<string, string>
        {
            { "Amarelo", "#ffe079" },
            { "Rosa", "#f097ca" },
            { "Verde", "#98d366" },
            { "Azul", "#82d3e5" }
        };

        public Frm_AdicionarPostIt()
        {
            InitializeComponent();
            ConfigurarFormulario();
        }

        private void ConfigurarFormulario()
        {
            this.Size = new Size(400, 300);
            this.Text = "Adicionar Post-It";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Label de instrução
            var lblInstrucao = new Label
            {
                Text = "Digite a descrição da tarefa:",
                Location = new Point(20, 20),
                Size = new Size(350, 20),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            // TextBox para descrição
            var txtDescricao = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(350, 100),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Arial", 9)
            };

            // Label para cores
            var lblCores = new Label
            {
                Text = "Selecione a cor do post-it:",
                Location = new Point(20, 160),
                Size = new Size(350, 20),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            // RadioButtons para cores
            int yPos = 185;
            var grupoCores = new GroupBox
            {
                Location = new Point(20, yPos),
                Size = new Size(350, 80)
            };

            var rdbAmarelo = new RadioButton
            {
                Text = "Amarelo",
                Location = new Point(10, 15),
                Checked = true,
                Tag = "#ffe079"
            };

            var rdbRosa = new RadioButton
            {
                Text = "Rosa",
                Location = new Point(100, 15),
                Tag = "#f097ca"
            };

            var rdbVerde = new RadioButton
            {
                Text = "Verde",
                Location = new Point(190, 15),
                Tag = "#98d366"
            };

            var rdbAzul = new RadioButton
            {
                Text = "Azul",
                Location = new Point(280, 15),
                Tag = "#82d3e5"
            };

            // Botões
            var btnAdicionar = new Button
            {
                Text = "Adicionar",
                Location = new Point(200, 270),
                Size = new Size(80, 25),
                DialogResult = DialogResult.OK
            };

            var btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(290, 270),
                Size = new Size(80, 25),
                DialogResult = DialogResult.Cancel
            };

            // Adicionar controles ao grupo
            grupoCores.Controls.AddRange(new Control[] { rdbAmarelo, rdbRosa, rdbVerde, rdbAzul });

            // Adicionar controles ao formulário
            this.Controls.AddRange(new Control[] {
                lblInstrucao, txtDescricao, lblCores, grupoCores, btnAdicionar, btnCancelar
            });

            // Eventos
            btnAdicionar.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtDescricao.Text))
                {
                    MessageBox.Show("Por favor, digite uma descrição para a tarefa.", "Aviso",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DescricaoTarefa = txtDescricao.Text.Trim();

                // Obter cor selecionada
                foreach (RadioButton rdb in grupoCores.Controls)
                {
                    if (rdb.Checked)
                    {
                        CorSelecionada = rdb.Tag.ToString();
                        break;
                    }
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            btnCancelar.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
        }
    }
}