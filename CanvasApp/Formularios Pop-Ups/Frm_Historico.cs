using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CanvasApp.Formularios_Pop_Ups
{
    public partial class Frm_Historico : Form
    {
        private List<string> historicoItens;

        public Frm_Historico(List<string> historico)
        {
            InitializeComponent();
            this.historicoItens = historico ?? new List<string>();
            ConfigurarFormulario();
            CarregarHistoricoNoFlowLayout();
        }

        private void ConfigurarFormulario()
        {
            this.Size = new Size(600, 450);
            this.Text = "Histórico da Tarefa";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void CarregarHistoricoNoFlowLayout()
        {
            // Limpa o FlowLayoutPanel
            Flw_LayoutHistorico.Controls.Clear();

            if (!historicoItens.Any())
            {
                var lblVazio = new Label
                {
                    Text = "Nenhum item no histórico.",
                    ForeColor = Color.Gray,
                    Font = new Font("Arial", 10, FontStyle.Italic),
                    AutoSize = true,
                    Margin = new Padding(10)
                };
                Flw_LayoutHistorico.Controls.Add(lblVazio);
                return;
            }

            // Adiciona cada item do histórico como um Label no FlowLayoutPanel
            for (int i = 0; i < historicoItens.Count; i++)
            {
                var item = historicoItens[i];

                var panelItem = new Panel
                {
                    Size = new Size(Flw_LayoutHistorico.Width - 25, 60),
                    Margin = new Padding(5),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = i % 2 == 0 ? Color.White : Color.LightGray
                };

                var lblNumero = new Label
                {
                    Text = $"{i + 1}.",
                    Location = new Point(5, 5),
                    Size = new Size(30, 20),
                    Font = new Font("Arial", 9, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleRight
                };

                var lblTexto = new Label
                {
                    Text = item,
                    Location = new Point(40, 5),
                    Size = new Size(panelItem.Width - 50, 50),
                    Font = new Font("Arial", 9),
                    TextAlign = ContentAlignment.MiddleLeft
                };

                // Quebra de texto automática
                lblTexto.AutoSize = false;
                lblTexto.MaximumSize = new Size(panelItem.Width - 50, 0);

                panelItem.Controls.Add(lblNumero);
                panelItem.Controls.Add(lblTexto);
                Flw_LayoutHistorico.Controls.Add(panelItem);
            }
        }

        private void Btu_Fechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCopiar_Click(object sender, EventArgs e)
        {
            if (!historicoItens.Any())
            {
                MessageBox.Show("Não há itens no histórico para copiar.", "Histórico Vazio",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                var textoCompleto = new StringBuilder();
                for (int i = 0; i < historicoItens.Count; i++)
                {
                    textoCompleto.AppendLine($"{i + 1}. {historicoItens[i]}");
                }

                Clipboard.SetText(textoCompleto.ToString());
                MessageBox.Show("Histórico copiado para a área de transferência!", "Copiado",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao copiar: {ex.Message}", "Erro",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            if (!historicoItens.Any())
            {
                MessageBox.Show("O histórico já está vazio.", "Histórico Vazio",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show(
                "Tem certeza que deseja limpar todo o histórico?\nEsta ação não pode ser desfeita.",
                "Confirmar Limpeza",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                historicoItens.Clear();
                CarregarHistoricoNoFlowLayout();
                MessageBox.Show("Histórico limpo com sucesso!", "Limpeza Concluída",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Evento para redimensionar os itens quando o formulário for redimensionado
        private void Frm_Historico_Resize(object sender, EventArgs e)
        {
            // Ajusta o tamanho dos panels existentes quando o formulário é redimensionado
            foreach (Control control in Flw_LayoutHistorico.Controls)
            {
                if (control is Panel panel)
                {
                    panel.Width = Flw_LayoutHistorico.Width - 25;
                    foreach (Control childControl in panel.Controls)
                    {
                        if (childControl is Label lbl && lbl.Name != "lblNumero")
                        {
                            lbl.MaximumSize = new Size(panel.Width - 50, 0);
                            lbl.Size = new Size(panel.Width - 50, lbl.Height);
                        }
                    }
                }
            }
        }
    }
}