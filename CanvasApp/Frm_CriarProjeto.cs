using CanvasApp.Classes.Databases;
using CanvasApp.Classes.Databases.UsuarioCL;
using Home;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using CanvasApp.Classes.Tema;

namespace CanvasApp
{
    public partial class Frm_CriarProjeto : Form
    {
        private List<Usuario> membrosAdicionados = new List<Usuario>();
        private Usuario proprietario;
        private readonly UsuarioDB dbUsuario = new UsuarioDB();
        private readonly ProjetosDB dbProjetos = new ProjetosDB();
        private readonly NotificacoesDB dbNotificacoes = new NotificacoesDB();
        private readonly MembrosDB dbMembros;
        private int usuarioLogadoId;

        public Projetos ProjetoCriado { get; private set; }

        public Frm_CriarProjeto()
        {
            InitializeComponent();
            // Inicializar MembrosDB com as dependências necessárias
            dbMembros = new MembrosDB(dbNotificacoes, dbProjetos, dbUsuario);
            InitializeUI();
        }

        private void InitializeUI()
        {
            this.BackColor = Tema.corAtual;
            this.usuarioLogadoId = int.Parse(Sessao.UsuarioLogado.Codigo);

            Tbp_Membros.Text = "Membros da Lista";
            Tbp_Opcoes.Text = "Opções da lista";
            Lbl_Criar.Text = "Criar Projeto";

            if (string.IsNullOrEmpty(Txt_NomeProjeto.Text))
            {
                Lbl_Nome.Text = "Nome do Projeto";
            }
            else
            {
                Lbl_Nome.Hide();
                Pic_List.Hide();
            }

            Btn_Criar.Text = "Criar";
            Btn_Cancelar.Text = "Cancelar";

            if (string.IsNullOrEmpty(Txt_NomeEndereco.Text))
            {
                Lbl_NomeEndereco.Text = "Nome ou endereço de email";
            }
            else
            {
                Lbl_NomeEndereco.Hide();
                Pic_AddPerson.Hide();
            }

            ConfigurarToggleSwitch();
            Lst_Sugestoes.Hide();

            Lst_Sugestoes.View = View.Details;
            Lst_Sugestoes.Columns.Add("Usuário", 150);
            Lst_Sugestoes.Columns.Add("Email", 200);
            Lst_Sugestoes.FullRowSelect = true;
        }

        private void ConfigurarToggleSwitch()
        {
            Rbtn_ToggleSwitch.Appearance = Appearance.Button;
            Rbtn_ToggleSwitch.FlatStyle = FlatStyle.Flat;
            Rbtn_ToggleSwitch.FlatAppearance.BorderSize = 0;
            Rbtn_ToggleSwitch.Size = new Size(80, 30);
            Rbtn_ToggleSwitch.TextAlign = ContentAlignment.MiddleCenter;
            Rbtn_ToggleSwitch.ForeColor = Color.White;
            Rbtn_ToggleSwitch.Font = new Font("Arial", 9, FontStyle.Bold);

            if (Rbtn_ToggleSwitch.Checked)
            {
                Rbtn_ToggleSwitch.Text = "ATIVADO";
                Rbtn_ToggleSwitch.BackColor = Color.FromArgb(220, 80, 80);
            }
            else
            {
                Rbtn_ToggleSwitch.Text = "DESATIVADO";
                Rbtn_ToggleSwitch.BackColor = Color.FromArgb(100, 180, 100);
            }
        }

        private Control CriarItemMembro(Usuario usuario, bool isProprietario)
        {
            var panel = new Panel
            {
                Width = Lst_ListaMembros.Width - 25,
                Height = 40,
                Margin = new Padding(3),
                BackColor = Color.WhiteSmoke,
                BorderStyle = BorderStyle.FixedSingle
            };

            Color corBolinha = GetCorAleatoria();

            var lblInicial = new Label
            {
                Text = usuario.Nome.Length > 0 ? usuario.Nome[0].ToString().ToUpper() : "?",
                Width = 30,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = corBolinha,
                ForeColor = Color.White,
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(5, 5)
            };

            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, lblInicial.Width, lblInicial.Height);
            lblInicial.Region = new Region(gp);
            panel.Controls.Add(lblInicial);

            string[] nomes = usuario.Nome.Split(' ');
            string nomeParaExibir = nomes.Length >= 2 ? $"{nomes[0]} {nomes[1]}" : usuario.Nome;

            var lblNome = new Label
            {
                Text = nomeParaExibir,
                Location = new Point(45, 10),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(64, 64, 64)
            };
            panel.Controls.Add(lblNome);

            string titulo = isProprietario ? "Proprietário" : "Colaborador";
            var lblTitulo = new Label
            {
                Text = titulo,
                AutoSize = true,
                Location = new Point(lblNome.Location.X + lblNome.Width + 10, 10),
                Font = new Font("Arial", 8, FontStyle.Italic),
                ForeColor = isProprietario ? Color.FromArgb(0, 120, 215) : Color.Gray
            };
            panel.Controls.Add(lblTitulo);

            if (!isProprietario)
            {
                var btnRemover = new Button
                {
                    Text = "✕",
                    Size = new Size(25, 25),
                    Location = new Point(panel.Width - 35, 7),
                    Tag = usuario,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.Red,
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    BackColor = Color.Transparent
                };
                btnRemover.FlatAppearance.BorderSize = 0;
                btnRemover.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 200, 200);
                btnRemover.Click += BtnRemover_Click;
                panel.Controls.Add(btnRemover);
            }

            return panel;
        }

        private Color GetCorAleatoria()
        {
            Random rnd = new Random();
            Color[] cores = {
                Color.FromArgb(255, 140, 140),
                Color.FromArgb(100, 180, 255),
                Color.FromArgb(120, 220, 120),
                Color.FromArgb(180, 140, 255),
                Color.FromArgb(255, 180, 100)
            };
            return cores[rnd.Next(cores.Length)];
        }

        private void BtnRemover_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is Usuario usuario)
            {
                membrosAdicionados.RemoveAll(m => m.Codigo == usuario.Codigo);
                AtualizarListaMembros();
            }
        }

        private void Txt_NomeProjeto_TextChanged(object sender, EventArgs e)
        {
            string nome = Txt_NomeProjeto.Text.Trim();
            Btn_Criar.Enabled = nome.Replace(" ", "").Length >= 3;

            if (nome.Length > 0)
            {
                Lbl_Nome.Visible = false;
                Pic_List.Visible = false;
            }
            else
            {
                Lbl_Nome.Visible = true;
                Pic_List.Visible = true;
            }
        }

        private void Txt_NomeEndereco_TextChanged(object sender, EventArgs e)
        {
            string textoBusca = Txt_NomeEndereco.Text.Trim();

            if (string.IsNullOrEmpty(textoBusca))
            {
                Lst_Sugestoes.Visible = false;
                Pic_AddPerson.Visible = false;
                Lbl_NomeEndereco.Visible = true;
                return;
            }

            var resultados = dbUsuario.BuscarUsuariosPorTexto(textoBusca);
            Lst_Sugestoes.Items.Clear();

            foreach (var usuario in resultados)
            {
                var item = new ListViewItem(usuario.NomeUsuario);
                item.Tag = usuario;
                item.SubItems.Add(usuario.Email);
                Lst_Sugestoes.Items.Add(item);
            }

            Lst_Sugestoes.Visible = resultados.Any();
            Pic_AddPerson.Visible = false;
            Lbl_NomeEndereco.Visible = false;
        }

        private void AtualizarListaMembros()
        {
            Lst_ListaMembros.Controls.Clear();

            if (proprietario != null)
            {
                var itemProprietario = CriarItemMembro(proprietario, isProprietario: true);
                Lst_ListaMembros.Controls.Add(itemProprietario);
            }

            foreach (var membro in membrosAdicionados.Where(m => m.Codigo != proprietario?.Codigo))
            {
                var itemMembro = CriarItemMembro(membro, isProprietario: false);
                Lst_ListaMembros.Controls.Add(itemMembro);
            }
        }

        private void AdicionarMembro(Usuario usuario)
        {
            if (usuario == null) return;

            if (!membrosAdicionados.Any(m => m.Codigo == usuario.Codigo))
            {
                membrosAdicionados.Add(usuario);
                AtualizarListaMembros();
            }

            Txt_NomeEndereco.Clear();
            Lst_Sugestoes.Visible = false;
            Lbl_NomeEndereco.Visible = true;
            Pic_AddPerson.Visible = true;
        }

        private void Rbtn_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            if (Rbtn_ToggleSwitch.Checked)
            {
                Rbtn_ToggleSwitch.Text = "ATIVADO";
                Rbtn_ToggleSwitch.BackColor = Color.FromArgb(220, 80, 80);
                Rbtn_ToggleSwitch.ForeColor = Color.White;
            }
            else
            {
                Rbtn_ToggleSwitch.Text = "DESATIVADO";
                Rbtn_ToggleSwitch.BackColor = Color.FromArgb(100, 180, 100);
                Rbtn_ToggleSwitch.ForeColor = Color.White;
            }
        }

        private void Btn_Criar_Click(object sender, EventArgs e)
        {
            string nomeProjeto = Txt_NomeProjeto.Text.Trim();

            if (nomeProjeto.Replace(" ", "").Length < 3)
            {
                MessageBox.Show("O nome do projeto deve ter pelo menos 3 caracteres sem espaços.", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (proprietario == null)
            {
                MessageBox.Show("Proprietário não definido.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var projeto = new Projetos
            {
                Nome = nomeProjeto,
                CodUsuario = int.Parse(proprietario.Codigo),
                NaoPertube = Rbtn_ToggleSwitch.Checked
            };

            bool sucesso;

            if (membrosAdicionados.Count > 0)
            {
                sucesso = dbProjetos.CriarProjetoCompartilhado(projeto);
            }
            else
            {
                sucesso = dbProjetos.InserirProjeto(projeto);
            }

            if (!sucesso)
            {
                MessageBox.Show("Erro ao criar o projeto: " + dbProjetos.Mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var projetosUsuario = dbProjetos.ObterProjetosPorUsuario(int.Parse(proprietario.Codigo));
            var projetoCompleto = projetosUsuario?.FirstOrDefault(p => p.Nome == nomeProjeto);

            if (projetoCompleto == null)
            {
                MessageBox.Show("Erro ao recuperar o projeto criado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (membrosAdicionados.Count > 0)
            {
                foreach (var membro in membrosAdicionados)
                {
                    if (membro.Codigo == proprietario.Codigo)
                        continue;

                    if (!dbMembros.AdicionarMembroAoProjeto(projetoCompleto.Codigo, int.Parse(membro.Codigo)))
                    {
                        MessageBox.Show($"Erro ao adicionar membro {membro.Nome}: {dbMembros.Mensagem}", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            ProjetoCriado = projetoCompleto;

            MessageBox.Show("Projeto criado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void Txt_NomeEndereco_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Lst_Sugestoes.Visible && Lst_Sugestoes.SelectedItems.Count > 0)
            {
                Usuario usuarioSelecionado = (Usuario)Lst_Sugestoes.SelectedItems[0].Tag;
                AdicionarMembro(usuarioSelecionado);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void Lst_Sugestoes_ItemActivate(object sender, EventArgs e)
        {
            if (Lst_Sugestoes.SelectedItems.Count == 0) return;

            Usuario usuarioSelecionado = (Usuario)Lst_Sugestoes.SelectedItems[0].Tag;
            AdicionarMembro(usuarioSelecionado);
        }

        private void Frm_CriarProjeto_Load(object sender, EventArgs e)
        {
            proprietario = Sessao.UsuarioLogado;
            membrosAdicionados.Clear();
            AtualizarListaMembros();

            Btn_Criar.Enabled = false;
            Rbtn_ToggleSwitch.Checked = false;
            Rbtn_ToggleSwitch_CheckedChanged(Rbtn_ToggleSwitch, null);
        }

        private void Btn_Cancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void Pic_AddPerson_Click(object sender, EventArgs e)
        {
            if (Lst_Sugestoes.Visible && Lst_Sugestoes.SelectedItems.Count > 0)
            {
                Usuario usuarioSelecionado = (Usuario)Lst_Sugestoes.SelectedItems[0].Tag;
                AdicionarMembro(usuarioSelecionado);
            }
        }

        private void Txt_NomeEndereco_Enter(object sender, EventArgs e)
        {
            Lbl_NomeEndereco.Visible = false;
            Pic_AddPerson.Visible = false;
        }

        private void Txt_NomeEndereco_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Txt_NomeEndereco.Text))
            {
                Lbl_NomeEndereco.Visible = true;
                Pic_AddPerson.Visible = true;
            }
        }

        private void Lst_Sugestoes_SelectedIndexChanged(object sender, EventArgs e)
        {
            Pic_AddPerson.Visible = Lst_Sugestoes.SelectedItems.Count > 0;
        }

        // Eventos vazios
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) { }
        private void Rbtn_ToggleSwitch_CheckedChanged_1(object sender, EventArgs e) { }
    }
}