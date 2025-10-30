using Home;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Windows.Forms;
using CanvasApp.Classes.Tema;

namespace CanvasApp
{
    public partial class Frm_AlteraCor : Form
    {
        private Color corSelecionada;
        public Frm_AlteraCor()
        {
            InitializeComponent();

            Btn_Salvar.Text = "SALVAR";
            Btn_Selecionar.Text = "SELECIONAR COR";

            corSelecionada = Color.White;
            UpdateColorInfo(corSelecionada);
        }

        private void Btn_Selecionar_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    corSelecionada = colorDialog.Color;
                    UpdateColorInfo(corSelecionada);
                }
            }
        }

        private void Txt_Hexadecimal_BackColorChanged(object sender, EventArgs e)
        {

        }

        private void Txt_RGB_TextChanged(object sender, EventArgs e)
        {
            if (TryParseRgbColor(Txt_RGB.Text, out Color color))
            {
                corSelecionada = color;
                UpdateColorInfo(corSelecionada);
            }
        }

        private void Txt_RGB_BackColorChanged(object sender, EventArgs e)
        {

        }

        private void Btn_Salvar_Click(object sender, EventArgs e)
        {
            this.Close();
            Tema.corAtual = corSelecionada;

            foreach (Form form in Application.OpenForms)
            {
                form.BackColor = Tema.corAtual;
            }

            Frm_Home fh = new Frm_Home();
            fh.Show();
            this.Close();
        }

        private void UpdateColorInfo(Color color)
        {
            this.BackColor = color;
            Txt_Hexadecimal.Text = $"Hex: #{color.R:X2}{color.G:X2}{color.B:X2}";
            Txt_RGB.Text = $"RGB: {color.R}, {color.G}, {color.B}";
        }

        private void Frm_AlteraCor_Load(object sender, EventArgs e)
        {

        }

        private void Txt_Hexadecimal_TextChanged(object sender, EventArgs e)
        {
            if (TryParseHexColor(Txt_Hexadecimal.Text, out Color color))
            {
                corSelecionada = color;
                UpdateColorInfo(corSelecionada);
            }
        }

        private bool TryParseHexColor(string hex, out Color color)
        {
            color = Color.Empty;
            if (hex.StartsWith("#") && hex.Length == 7)
            {
                try
                {
                    color = ColorTranslator.FromHtml(hex);
                    return true;
                }
                catch(Exception ex )
                {
                    throw new Exception(ex.Message);
                    return false;
                }
            }
            return false;
        }

        private bool TryParseRgbColor(string rgb, out Color color)
        {
            color = Color.Empty;
            var parts = rgb.Split(',');
            if (parts.Length == 3 &&
                int.TryParse(parts[0], out int r) &&
                int.TryParse(parts[1], out int g) &&
                int.TryParse(parts[2], out int b) &&
                r >= 0 && r <= 255 && g>= 0 && g <= 255 && b>= 0 && b <= 255)
            {
                color = Color.FromArgb(r, g, b);
                return true;
            }
            return false;
        }

    }
}
