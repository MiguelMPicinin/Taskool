using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using CanvasApp.Classes.Errors;
using CanvasApp.Classes.Databases;
using System.IO;
using CanvasApp.Classes.Tema;
using System.Linq;
using Home;
using System.Media;

namespace CanvasApp
{
    public partial class Frm_Autenticacao : Form
    {
        public Frm_Autenticacao()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.BackColor = Tema.corAtual;

            Lbl_Titulo.Text = "Bem-vindo ao Taskool";
            Lbl_User.Text = "Usuário";
            Lbl_Credencial.Text = "Credencial";
            Btn_Entrar.Text = "Entrar";
            Lnk_Cadastrar.Text = "Cadastrar-se";
            Lnk_Teclado.Text = "Teclado Virtual";
        }

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            using (OpenFileDialog FotoFile = new OpenFileDialog())
            {
                FotoFile.Title = "Escolher uma Imagem";
                FotoFile.Filter = "Arquivos de Imagem |*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                try
                {
                    if (FotoFile.ShowDialog() == DialogResult.OK)
                    {
                        pictureBox1.Image = Image.FromFile(FotoFile.FileName);
                        pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao carregar imagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Btn_Entrar_Click(object sender, EventArgs e)
        {
            try
            {
                string loginDigitado = Txt_Login.Text.Trim();

                // Validação de campos obrigatórios
                if (string.IsNullOrEmpty(loginDigitado) || pictureBox1.Image == null)
                {
                    SystemSounds.Beep.Play();
                    MessageBox.Show("Por favor, preencha o login e selecione uma imagem (clique duplo na imagem)", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (UsuarioDB dbUsuarios = new UsuarioDB())
                {
                    // Verifica se a conexão com o banco de dados foi bem-sucedida
                    if (!dbUsuarios.Status)
                    {
                        MessageBox.Show($"Erro de conexão com o banco: {dbUsuarios.Mensagem}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Busca o usuário no banco de dados
                    var usuario = dbUsuarios.BuscarPorLogin(loginDigitado);

                    // Verifica se o usuário foi encontrado
                    if (usuario == null)
                    {
                        MessageBox.Show("Usuário não encontrado.", "Erro!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        try
                        {
                            UsersErrors usersErrors = new UsersErrors(loginDigitado);
                            usersErrors.Incluir(loginDigitado);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erro ao catalogar erro de login: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        return;
                    }

                    // Verifica se a foto do usuário existe no banco
                    if (usuario.Foto == null)
                    {
                        MessageBox.Show("A imagem de credencial do usuário não foi cadastrada no banco de dados. Por favor, entre em contato com o suporte.", "Erro!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Converte a imagem selecionada para um array de bytes
                    byte[] imagemSelecionada = ImageToByteArray(pictureBox1.Image);

                    // Compara as imagens
                    bool imagensIguais = imagemSelecionada.SequenceEqual(usuario.Foto);

                    if (imagensIguais)
                    {
                        MessageBox.Show("Autenticação bem-sucedida.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Sessao.UsuarioLogado = usuario;
                        Frm_Home f = new Frm_Home();
                        f.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("A imagem fornecida não corresponde à imagem do usuário.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        try
                        {
                            UsersErrors usersErrors = new UsersErrors(loginDigitado);
                            usersErrors.Incluir(loginDigitado);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erro ao catalogar erro de autenticação: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void Lnk_Teclado_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(@"C:\Program Files\Common Files\microsoft shared\ink\TabTip.exe");

                Txt_Login.Focus();
                Txt_Login.Select();
                Txt_Login.SelectionStart = Txt_Login.Text.Length;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir teclado virtual: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Txt_Login_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Control.IsKeyLocked(Keys.CapsLock))
                {
                    // Mostrar aviso apenas uma vez
                    if (!Txt_Login.Tag?.Equals("CAPS_WARNED") == true)
                    {
                        MessageBox.Show("Atenção!: Você está com o CAPS LOCK ativado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Txt_Login.Tag = "CAPS_WARNED";
                    }
                }
                else
                {
                    Txt_Login.Tag = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Lnk_Cadastrar_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Frm_Cadastro f = new Frm_Cadastro();
            f.Show();
            this.Hide();
        }

        private void Txt_Login_Enter(object sender, EventArgs e)
        {
            // Limpar aviso do CAPS LOCK quando o campo recebe foco
            Txt_Login.Tag = null;
        }

        // Eventos vazios necessários
        private void Autenticacao_Load(object sender, EventArgs e) { }
        private void pictureBox1_Click(object sender, EventArgs e) { }
    }
}