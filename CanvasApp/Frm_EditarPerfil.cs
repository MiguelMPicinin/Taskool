using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace CanvasApp.Forms
{
    public partial class Frm_EditarPerfil : Form
    {
        private Usuario _usuario;
        private UsuarioDB _usuarioDB;
        private string _caminhoImagemTemp;

        private PictureBox Pic_FotoPerfil;
        private Button Btn_UploadImagem;
        private Button Btn_Salvar;
        private Button Btn_Cancelar;
        private TextBox Txt_Nome;
        private TextBox Txt_Email;
        private Label Lbl_Titulo;

        public Frm_EditarPerfil(Usuario usuario)
        {
            _usuario = usuario;
            _usuarioDB = new UsuarioDB();
            InitializeComponentCustom();
            ConfigurarInterface();
            CarregarDadosUsuario();
        }

        // CORREÇÃO: Renomear o método InitializeComponent para evitar conflito
        private void InitializeComponentCustom()
        {
            this.Lbl_Titulo = new Label();
            this.Pic_FotoPerfil = new PictureBox();
            this.Btn_UploadImagem = new Button();
            this.Txt_Nome = new TextBox();
            this.Txt_Email = new TextBox();
            this.Btn_Salvar = new Button();
            this.Btn_Cancelar = new Button();

            // Lbl_Titulo
            this.Lbl_Titulo.AutoSize = true;
            this.Lbl_Titulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.Lbl_Titulo.Location = new Point(20, 20);
            this.Lbl_Titulo.Name = "Lbl_Titulo";
            this.Lbl_Titulo.Size = new Size(120, 25);
            this.Lbl_Titulo.TabIndex = 0;
            this.Lbl_Titulo.Text = "Editar Perfil";

            // Pic_FotoPerfil
            this.Pic_FotoPerfil.Size = new Size(100, 100);
            this.Pic_FotoPerfil.Location = new Point(20, 60);
            this.Pic_FotoPerfil.SizeMode = PictureBoxSizeMode.Zoom;
            this.Pic_FotoPerfil.BorderStyle = BorderStyle.FixedSingle;
            this.Pic_FotoPerfil.Cursor = Cursors.Hand;

            // Btn_UploadImagem
            this.Btn_UploadImagem.Text = "Alterar Foto";
            this.Btn_UploadImagem.Location = new Point(20, 170);
            this.Btn_UploadImagem.Size = new Size(100, 25);
            this.Btn_UploadImagem.BackColor = Color.FromArgb(74, 124, 255);
            this.Btn_UploadImagem.ForeColor = Color.White;

            // Txt_Nome
            this.Txt_Nome.Location = new Point(140, 80);
            this.Txt_Nome.Size = new Size(200, 20);

            // Txt_Email
            this.Txt_Email.Location = new Point(140, 120);
            this.Txt_Email.Size = new Size(200, 20);

            // Btn_Salvar
            this.Btn_Salvar.Text = "Salvar";
            this.Btn_Salvar.Location = new Point(140, 170);
            this.Btn_Salvar.Size = new Size(80, 30);
            this.Btn_Salvar.BackColor = Color.FromArgb(50, 200, 100);
            this.Btn_Salvar.ForeColor = Color.White;

            // Btn_Cancelar
            this.Btn_Cancelar.Text = "Cancelar";
            this.Btn_Cancelar.Location = new Point(230, 170);
            this.Btn_Cancelar.Size = new Size(80, 30);
            this.Btn_Cancelar.BackColor = Color.FromArgb(255, 87, 87);
            this.Btn_Cancelar.ForeColor = Color.White;

            // Form
            this.BackColor = Color.White;
            this.ClientSize = new Size(400, 250);
            this.Controls.AddRange(new Control[] {
                Lbl_Titulo, Pic_FotoPerfil, Btn_UploadImagem,
                Txt_Nome, Txt_Email, Btn_Salvar, Btn_Cancelar
            });
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Frm_EditarPerfil";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Editar Perfil";
        }

        private void ConfigurarInterface()
        {
            // Eventos
            Btn_UploadImagem.Click += Btn_UploadImagem_Click;
            Btn_Salvar.Click += Btn_Salvar_Click;
            Btn_Cancelar.Click += Btn_Cancelar_Click;
            Pic_FotoPerfil.Click += Btn_UploadImagem_Click;

            // ToolTips
            var toolTip = new ToolTip();
            toolTip.SetToolTip(Pic_FotoPerfil, "Clique para alterar a foto");
            toolTip.SetToolTip(Btn_UploadImagem, "Selecionar imagem do computador");
        }

        private void CarregarDadosUsuario()
        {
            Txt_Nome.Text = _usuario.Nome;
            Txt_Email.Text = _usuario.Email;

            // Carregar imagem do usuário se existir
            if (_usuario.Foto != null && _usuario.Foto.Length > 0)
            {
                using (var ms = new MemoryStream(_usuario.Foto))
                {
                    Pic_FotoPerfil.Image = Image.FromStream(ms);
                }
            }
            else
            {
                // Imagem padrão
                Pic_FotoPerfil.Image = CriarImagemPadrao();
            }
        }

        private Image CriarImagemPadrao()
        {
            Bitmap bmp = new Bitmap(100, 100);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                using (Font font = new Font("Arial", 24, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.White))
                {
                    string inicial = _usuario.Nome.Length > 0 ? _usuario.Nome[0].ToString().ToUpper() : "?";
                    SizeF textSize = g.MeasureString(inicial, font);
                    g.DrawString(inicial, font, brush,
                        (bmp.Width - textSize.Width) / 2,
                        (bmp.Height - textSize.Height) / 2);
                }
            }
            return bmp;
        }

        private void Btn_UploadImagem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Title = "Selecionar imagem de perfil";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _caminhoImagemTemp = openFileDialog.FileName;

                        // Validar tamanho da imagem (máximo 2MB)
                        FileInfo fileInfo = new FileInfo(_caminhoImagemTemp);
                        if (fileInfo.Length > 2097152) // 2MB em bytes
                        {
                            MessageBox.Show("A imagem deve ter no máximo 2MB.", "Imagem muito grande",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Carregar e redimensionar imagem
                        Image imagemOriginal = Image.FromFile(_caminhoImagemTemp);
                        Pic_FotoPerfil.Image = RedimensionarImagem(imagemOriginal, 100, 100);

                        MessageBox.Show("Imagem carregada com sucesso! Clique em Salvar para confirmar.", "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao carregar imagem: {ex.Message}", "Erro",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private Image RedimensionarImagem(Image imagem, int largura, int altura)
        {
            Bitmap novaImagem = new Bitmap(largura, altura);
            using (Graphics g = Graphics.FromImage(novaImagem))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(imagem, 0, 0, largura, altura);
            }
            return novaImagem;
        }

        private void Btn_Salvar_Click(object sender, EventArgs e)
        {
            try
            {
                // Validar campos
                if (string.IsNullOrWhiteSpace(Txt_Nome.Text))
                {
                    MessageBox.Show("O nome é obrigatório.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(Txt_Email.Text))
                {
                    MessageBox.Show("O e-mail é obrigatório.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Atualizar dados do usuário
                _usuario.Nome = Txt_Nome.Text.Trim();
                _usuario.Email = Txt_Email.Text.Trim();

                // Converter imagem para byte[] se foi carregada uma nova
                if (!string.IsNullOrEmpty(_caminhoImagemTemp))
                {
                    _usuario.Foto = File.ReadAllBytes(_caminhoImagemTemp);
                }

                // CORREÇÃO: Usar o método correto do UsuarioDB
                if (_usuarioDB.AtualizarPerfilUsuario(_usuario))
                {
                    MessageBox.Show("Perfil atualizado com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show($"Erro ao atualizar perfil: {_usuarioDB.Mensagem}", "Erro",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar perfil: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Btn_Cancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}