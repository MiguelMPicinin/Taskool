using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CanvasApp.Formularios_Pop_Ups
{
    public partial class Frm_HistoricoTarefas : Form
    {
        private List<string> historicoItens;

        public Frm_HistoricoTarefas(List<string> historico)
        {
            InitializeComponent();
            historicoItens = historico ?? new List<string>();
            CarregarHistoricoNoFlowLayout();
        }

        private void CarregarHistoricoNoFlowLayout()
        {
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

            foreach (var item in historicoItens)
            {
                var panelItem = new Panel
                {
                    Size = new Size(Flw_LayoutHistorico.Width - 30, 50),
                    Margin = new Padding(5),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.White,
                    Padding = new Padding(5)
                };

                var lblTexto = new Label
                {
                    Text = item,
                    Dock = DockStyle.Fill,
                    Font = new Font("Arial", 9),
                    TextAlign = ContentAlignment.MiddleLeft,
                    AutoSize = false
                };

                panelItem.Controls.Add(lblTexto);
                Flw_LayoutHistorico.Controls.Add(panelItem);
            }
        }

        private void Btn_Fechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Frm_Historico_Load(object sender, EventArgs e)
        {
            foreach (Control control in Flw_LayoutHistorico.Controls)
            {
                if (control is Panel panel)
                {
                    panel.Width = Flw_LayoutHistorico.Width - 30;
                }
            }
        }

        private void Flw_LayoutHistorico_Resize(object sender, EventArgs e)
        {
            foreach (Control control in Flw_LayoutHistorico.Controls)
            {
                if (control is Panel panel)
                {
                    panel.Width = Flw_LayoutHistorico.Width - 30;
                }
            }
        }
    }
}