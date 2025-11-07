using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasApp.Formularios_Pop_Ups
{
    public partial class Frm_AdicionarPostIt : Form
    {
        public string DescricaoTarefa { get; set; }
        public string CorSelecionada { get; set; }

        private Dictionary<Button, string> botaoCores = new Dictionary<Button, string>();

        public Frm_AdicionarPostIt()
        {
            InitializeComponent();
            InicializarForm();
        }

        private void InicializarForm()
        {
            // Configurar cores dos botões
            botaoCores[Btn_Cores1] = "#ffe079"; // Amarelo
            botaoCores[Btn_Cores2] = "#f097ca"; // Rosa  
            botaoCores[Btn_Cores3] = "#98d366"; // Verde
            botaoCores[Btn_Cores4] = "#82d3e5"; // Azul

            // Aplicar cores aos botões
            foreach (var botaoCor in botaoCores)
            {
                botaoCor.Key.BackColor = ColorTranslator.FromHtml(botaoCor.Value);
                botaoCor.Key.FlatStyle = FlatStyle.Flat;
                botaoCor.Key.FlatAppearance.BorderSize = 2;
                botaoCor.Key.FlatAppearance.BorderColor = Color.Gray;
                botaoCor.Key.Size = new Size(50, 50);
            }

            // Selecionar primeira cor por padrão
            SelecionarCor(Btn_Cores1);

            // Configurar texto
            Txt_Texto.Multiline = true;
            Txt_Texto.ScrollBars = ScrollBars.Vertical;
            Txt_Texto.Font = new Font("Arial", 10);
        }

        private void SelecionarCor(Button botaoSelecionado)
        {
            // Remover seleção de todos os botões
            foreach (var botao in botaoCores.Keys)
            {
                botao.FlatAppearance.BorderColor = Color.Gray;
                botao.FlatAppearance.BorderSize = 2;
            }

            // Destacar botão selecionado
            botaoSelecionado.FlatAppearance.BorderColor = Color.Black;
            botaoSelecionado.FlatAppearance.BorderSize = 3;

            // Definir cor selecionada
            CorSelecionada = botaoCores[botaoSelecionado];
        }

        private void Btn_Cores1_Click(object sender, EventArgs e)
        {
            SelecionarCor(Btn_Cores1);
        }

        private void Btn_Cores2_Click(object sender, EventArgs e)
        {
            SelecionarCor(Btn_Cores2);
        }

        private void Btn_Cores3_Click(object sender, EventArgs e)
        {
            SelecionarCor(Btn_Cores3);
        }

        private void Btn_Cores4_Click(object sender, EventArgs e)
        {
            SelecionarCor(Btn_Cores4);
        }

        private void Btn_Ok_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Txt_Texto.Text))
            {
                MessageBox.Show("Digite o texto do post-it!", "Aviso",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Txt_Texto.Focus();
                return;
            }

            DescricaoTarefa = Txt_Texto.Text.Trim();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Btn_Cancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void Frm_AdicionarPostIt_Load(object sender, EventArgs e)
        {
            Txt_Texto.Focus();
        }
    }
}