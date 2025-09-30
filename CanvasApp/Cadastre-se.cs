using CanvasApp.Classes.ClsUteis;
using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using Home;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using CanvasApp.Classes.Tema;

namespace CanvasApp
{
    public partial class Frm_Cadastro : Form
    {
        public Frm_Cadastro()
        {
            this.BackColor = Tema.corAtual;
            InitializeComponent();
            Lbl_Credencial.Text = "Credencial";
            Lbl_Data.Text = "Data de nasc.";
            Lbl_Email.Text = "Email";
            Lbl_Nome.Text = "Nome";
            Lbl_Telefone.Text = "Telefone";
            Lbl_Titulo.Text = "Cadastre-se";
            Lbl_Usuario.Text = "Usuário";
            Btn_Aleatorio.Text = "Gerar Aleatorio";
            Btn_Salvar.Text = "Salvar";
            Btn_Selecionar.Text = "Selecionar";
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        { }
        private void Txt_Nome_StyleChanged(object sender, EventArgs e)
        { }
        private void Txt_Email_TextChanged(object sender, EventArgs e)
        { }
        private void Pic_Foto_Click(object sender, EventArgs e)
        { }
        private void Pic_Foto_DoubleClick(object sender, EventArgs e)
        {
            using (OpenFileDialog FotoFile = new OpenFileDialog())
            {
                FotoFile.Title = "Escolher uma Imagem";
                FotoFile.Filter = "Arquivos de Imagem |*.jpg;*.png;";
                try
                {
                    if (FotoFile.ShowDialog() == DialogResult.OK)
                    {
                        Pic_Foto.Image = Image.FromFile(FotoFile.FileName);
                        Pic_Foto.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        private byte[] ImageToByteArray(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }
        private void Btn_Selecionar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog FotoFile = new OpenFileDialog())
            {
                FotoFile.Title = "Escolher uma Imagem";
                FotoFile.Filter = "Arquivos de Imagem |*.jpg;*.png;";
                try
                {
                    if (FotoFile.ShowDialog() == DialogResult.OK)
                    {
                        Pic_Foto.Image = Image.FromFile(FotoFile.FileName);
                        Pic_Foto.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
        private void Pic_Foto_StyleChanged(object sender, EventArgs e)
        {
            System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
            gp.AddEllipse(0, 0, Pic_Foto.Width - 3, Pic_Foto.Height - 3);
            Region rg = new Region(gp);
            Pic_Foto.Region = rg;
        }

        private void Pic_Foto_VisibleChanged(object sender, EventArgs e)
        { }

        private void Btn_Aleatorio_Click(object sender, EventArgs e)
        {
            try
            {
                string nomeCompleto = Txt_Nome.Text.Trim();
                string dataNascimento = Msk_Data.Text.Trim();

                if (string.IsNullOrWhiteSpace(nomeCompleto) || string.IsNullOrWhiteSpace(dataNascimento))
                {
                    MessageBox.Show("Preencha o Nome e Data de Nascimento para gerar o usuário.",
                                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string[] partesNome = nomeCompleto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (partesNome.Length < 2)
                {
                    MessageBox.Show("Informe pelo menos nome e sobrenome para gerar o usuário.",
                                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string primeiroNome = RemoverAcentos(partesNome[0].ToLower());
                string ultimoSobrenome = RemoverAcentos(partesNome[partesNome.Length - 1].ToLower());
                string penultimoSobrenome = partesNome.Length > 2
                    ? RemoverAcentos(partesNome[partesNome.Length - 2].ToLower())
                    : null;

                if (!DateTime.TryParseExact(dataNascimento, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtNasc))
                {
                    MessageBox.Show("Data de nascimento inválida.",
                                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string ano = dtNasc.Year.ToString().Substring(2);
                string sugestao = $"{primeiroNome}.{ultimoSobrenome}{ano}";

                using (var db = new UsuarioDB()) // Corrigido: UsuarioDB em vez de DBUsuarios
                {
                    if (!db.VerificaUsuarioExistente(sugestao))
                    {
                        Txt_Usuario.Text = sugestao;
                        return;
                    }

                    if (!string.IsNullOrEmpty(penultimoSobrenome))
                    {
                        sugestao = $"{primeiroNome}.{penultimoSobrenome}{ano}";
                        if (!db.VerificaUsuarioExistente(sugestao))
                        {
                            Txt_Usuario.Text = sugestao;
                            return;
                        }
                    }

                    MessageBox.Show("Não foi possível gerar um usuário disponível.",
                                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao gerar usuário aleatório: " + ex.Message,
                                "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string RemoverAcentos(string texto)
        {
            var normalized = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) !=
                    System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private void Btn_Salvar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validação de campos obrigatórios, incluindo a foto
                if (string.IsNullOrWhiteSpace(Txt_Nome.Text) ||
                    string.IsNullOrWhiteSpace(Txt_Usuario.Text) ||
                    !Msk_Data.MaskCompleted ||
                    Pic_Foto.Image == null) // Adiciona a verificação da imagem aqui
                {
                    MessageBox.Show("Preencha todos os campos obrigatórios (Nome, Usuário, Data de Nascimento e a Imagem de Credencial).", "Atenção",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validação de data de nascimento
                if (!DateTime.TryParseExact(Msk_Data.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                            DateTimeStyles.None, out DateTime dataNascimento))
                {
                    MessageBox.Show("Data de nascimento inválida.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var db = new UsuarioDB()) // Corrigido: UsuarioDB em vez de DBUsuarios
                {
                    // Verifica se o usuário já existe
                    if (db.VerificaUsuarioExistente(Txt_Usuario.Text.Trim()))
                    {
                        MessageBox.Show("O nome de usuário já está em uso. Escolha outro.", "Atenção",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Txt_Usuario.Focus();
                        return;
                    }

                    // Cria objeto Usuario
                    Usuario novoUsuario = new Usuario
                    {
                        Nome = Txt_Nome.Text.Trim(),
                        NomeUsuario = Txt_Usuario.Text.Trim(),
                        DataNascimento = Msk_Data.Text.Trim(),
                        Email = Txt_Email.Text.Trim(),
                        Telefone = Txt_Telefone.Text.Trim(),
                        Foto = ImageToByteArray(Pic_Foto.Image)
                    };

                    // Insere no banco
                    bool sucesso = db.InserirUsuario(novoUsuario);
                    if (sucesso)
                    {
                        MessageBox.Show("Usuário cadastrado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                        Frm_Autenticacao fa = new Frm_Autenticacao();
                        fa.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show($"Erro ao cadastrar usuário: {db.Mensagem}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar usuário: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Txt_Email_Validated(object sender, EventArgs e)
        {
            string email = Txt_Email.Text;
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Coloque um email válido", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                bool valido = VerificaEmail.FormatoValidoEmail(email);
                if (!valido)
                {
                    MessageBox.Show("Email inválido", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao validar o email: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}